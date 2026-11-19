using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace MultiPresence;

/// <summary>
/// Thread-safe, read-only facade for reading game process memory.
/// A single session is sufficient because MultiPresence runs one game integration at a time.
/// </summary>
public static class Hypervisor
{
    private static readonly ReaderWriterLockSlim SessionLock = new(LockRecursionPolicy.NoRecursion);
    private static readonly AsyncLocal<HypervisorSession?> ScopedSession = new();
    private static HypervisorSession? _session;

    public static Process? Process => WithSession(static session => session.CurrentProcess);
    public static ulong PureAddress => WithSession(static session => session.MainModuleAddress);
    public static bool IsAttached => WithOptionalSession(static session => session.IsAlive, false);

    public static void AttachProcess(Process input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var replacement = HypervisorSession.Attach(input);
        HypervisorSession? previous;

        SessionLock.EnterWriteLock();
        try
        {
            previous = _session;
            _session = replacement;
        }
        finally
        {
            SessionLock.ExitWriteLock();
        }

        previous?.Dispose();
    }

    public static void DetachProcess()
    {
        HypervisorSession? previous;

        SessionLock.EnterWriteLock();
        try
        {
            previous = _session;
            _session = null;
        }
        finally
        {
            SessionLock.ExitWriteLock();
        }

        previous?.Dispose();
    }

    public static T Read<T>(ulong address, bool absolute = false, string? moduleName = null)
        where T : struct =>
        WithSession(session => session.Read<T>(address, absolute, moduleName));

    public static bool TryRead<T>(ulong address, out T value, bool absolute = false, string? moduleName = null)
        where T : struct
    {
        T local = default;
        var success = WithOptionalSession(
            session => session.TryRead(address, out local, absolute, moduleName),
            false);
        value = local;
        return success;
    }

    public static T[] Read<T>(ulong address, int size, bool absolute = false, string? moduleName = null)
        where T : struct =>
        WithSession(session => session.ReadArray<T>(address, size, absolute, moduleName));

    public static string ReadString(
        ulong address,
        int length,
        bool absolute = false,
        string? moduleName = null,
        bool isUnicode = false) =>
        WithSession(session => session.ReadString(address, length, absolute, moduleName, isUnicode));

    public static byte[] ReadArray(ulong address, int length, bool absolute = false) =>
        WithSession(session => session.ReadBytes(address, length, absolute));

    public static ulong GetPointer64(
        ulong address,
        uint[]? offsets = null,
        bool absolute = false,
        string? moduleName = null) =>
        WithSession(session => session.GetPointer64(address, offsets, absolute, moduleName));

    public static uint GetPointer32(
        ulong address,
        uint[]? offsets = null,
        bool absolute = false,
        string? moduleName = null) =>
        WithSession(session => session.GetPointer32(address, offsets, absolute, moduleName));

    internal static int FindPattern(ReadOnlySpan<byte> data, string signature) =>
        SignaturePattern.Parse(signature).IndexOf(data);

    internal static IDisposable UseProcessScope(Process process)
    {
        var previous = ScopedSession.Value;
        var session = HypervisorSession.Attach(process);
        ScopedSession.Value = session;
        return new ProcessScope(session, previous);
    }

    private sealed class ProcessScope(HypervisorSession session, HypervisorSession? previous) : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            ScopedSession.Value = previous;
            session.Dispose();
        }
    }

    private static T WithSession<T>(Func<HypervisorSession, T> action)
    {
        if (ScopedSession.Value is { } scoped)
        {
            if (!scoped.IsAlive)
            {
                throw new InvalidOperationException("The scoped process is no longer running.");
            }
            return action(scoped);
        }

        SessionLock.EnterReadLock();
        try
        {
            var session = _session;
            if (session is null || !session.IsAlive)
            {
                throw new InvalidOperationException("Hypervisor is not attached to a running process.");
            }

            return action(session);
        }
        finally
        {
            SessionLock.ExitReadLock();
        }
    }

    private static T WithOptionalSession<T>(Func<HypervisorSession, T> action, T fallback)
    {
        if (ScopedSession.Value is { } scoped)
        {
            return scoped.IsAlive ? action(scoped) : fallback;
        }

        SessionLock.EnterReadLock();
        try
        {
            return _session is { } session ? action(session) : fallback;
        }
        catch (InvalidOperationException)
        {
            return fallback;
        }
        finally
        {
            SessionLock.ExitReadLock();
        }
    }
}

internal sealed class HypervisorSession : IDisposable
{
    private static readonly TimeSpan ModuleRefreshInterval = TimeSpan.FromSeconds(5);
    private readonly SafeProcessHandle _handle;
    private readonly object _moduleSync = new();
    private Dictionary<string, ulong> _moduleBaseAddresses;
    private DateTime _nextProcessRefresh;
    private DateTime _nextModuleScan;
    private bool _disposed;

    private HypervisorSession(Process process, SafeProcessHandle handle)
    {
        Process = process;
        _handle = handle;

        var mainModule = process.MainModule
            ?? throw new InvalidOperationException($"Unable to resolve the main module for process {process.Id}.");

        MainModuleAddress = unchecked((ulong)mainModule.BaseAddress.ToInt64());
        _moduleBaseAddresses = LoadModules(process);
        _nextProcessRefresh = DateTime.UtcNow + ModuleRefreshInterval;
        _nextModuleScan = _nextProcessRefresh;
    }

    public Process Process { get; }
    public ulong MainModuleAddress { get; }

    /// <summary>
    /// The attached process with its cached information (for example <see cref="Process.Modules"/>) refreshed
    /// every few seconds, so DLLs that a game loads after the attach are found by later polls.
    /// </summary>
    public Process CurrentProcess
    {
        get
        {
            lock (_moduleSync)
            {
                if (DateTime.UtcNow >= _nextProcessRefresh)
                {
                    _nextProcessRefresh = DateTime.UtcNow + ModuleRefreshInterval;
                    Process.Refresh();
                }
            }

            return Process;
        }
    }

    public bool IsAlive
    {
        get
        {
            if (_disposed || _handle.IsClosed || _handle.IsInvalid)
            {
                return false;
            }

            try
            {
                return !Process.HasExited;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }
    }

    public static HypervisorSession Attach(Process input)
    {
        int processId;
        try
        {
            processId = input.Id;
        }
        catch (InvalidOperationException exception)
        {
            throw new InvalidOperationException("The target process is no longer available.", exception);
        }

        var handle = NativeMethods.OpenProcess(
            ProcessAccess.QueryInformation | ProcessAccess.VirtualMemoryRead,
            false,
            processId);

        if (handle.IsInvalid)
        {
            var error = Marshal.GetLastWin32Error();
            handle.Dispose();
            throw new MemoryAccessException(
                $"Could not open process {processId}. Try running MultiPresence with the same privilege level as the game.",
                error);
        }

        try
        {
            return new HypervisorSession(Process.GetProcessById(processId), handle);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }

    public T Read<T>(ulong address, bool absolute, string? moduleName)
        where T : struct
    {
        var byteCount = TypeSize<T>();
        Span<byte> buffer = byteCount <= 256 ? stackalloc byte[byteCount] : new byte[byteCount];
        ReadExact(ResolveAddress(address, absolute, moduleName), buffer);
        return MemoryMarshal.Read<T>(buffer);
    }

    public bool TryRead<T>(ulong address, out T value, bool absolute, string? moduleName)
        where T : struct
    {
        try
        {
            value = Read<T>(address, absolute, moduleName);
            return true;
        }
        catch (Exception exception) when (exception is MemoryAccessException or InvalidOperationException or OverflowException)
        {
            value = default;
            return false;
        }
    }

    public T[] ReadArray<T>(ulong address, int count, bool absolute, string? moduleName)
        where T : struct
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (count == 0)
        {
            return [];
        }

        var elementSize = TypeSize<T>();
        var bytes = new byte[checked(elementSize * count)];
        ReadExact(ResolveAddress(address, absolute, moduleName), bytes);

        var result = new T[count];
        for (var index = 0; index < count; index++)
        {
            result[index] = MemoryMarshal.Read<T>(bytes.AsSpan(index * elementSize, elementSize));
        }

        return result;
    }

    public string ReadString(ulong address, int length, bool absolute, string? moduleName, bool isUnicode)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        var bytes = new byte[length];
        ReadExact(ResolveAddress(address, absolute, moduleName), bytes);

        var usedLength = isUnicode
            ? FindUnicodeTerminator(bytes)
            : Array.IndexOf(bytes, (byte)0) is var index && index >= 0 ? index : bytes.Length;

        return (isUnicode ? Encoding.Unicode : Encoding.UTF8)
            .GetString(bytes, 0, usedLength)
            .TrimEnd('\0');
    }

    public byte[] ReadBytes(ulong address, int length, bool absolute)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        var bytes = new byte[length];
        if (length > 0)
        {
            ReadExact(ResolveAddress(address, absolute, null), bytes);
        }

        return bytes;
    }

    public ulong GetPointer64(ulong address, uint[]? offsets, bool absolute, string? moduleName)
    {
        var pointer = Read<ulong>(address, absolute, moduleName);
        if (pointer == 0)
        {
            throw new MemoryAccessException("The pointer chain starts with a null pointer.");
        }

        if (offsets is null || offsets.Length == 0)
        {
            return pointer;
        }

        for (var index = 0; index < offsets.Length - 1; index++)
        {
            pointer = Read<ulong>(checked(pointer + offsets[index]), true, null);
            if (pointer == 0)
            {
                throw new MemoryAccessException($"The pointer chain contains a null pointer at offset index {index}.");
            }
        }

        return checked(pointer + offsets[^1]);
    }

    public uint GetPointer32(ulong address, uint[]? offsets, bool absolute, string? moduleName)
    {
        var pointer = Read<uint>(address, absolute, moduleName);
        if (pointer == 0)
        {
            throw new MemoryAccessException("The pointer chain starts with a null pointer.");
        }

        if (offsets is null || offsets.Length == 0)
        {
            return pointer;
        }

        for (var index = 0; index < offsets.Length - 1; index++)
        {
            pointer = Read<uint>(checked(pointer + offsets[index]), true, null);
            if (pointer == 0)
            {
                throw new MemoryAccessException($"The pointer chain contains a null pointer at offset index {index}.");
            }
        }

        return checked(pointer + offsets[^1]);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _handle.Dispose();
        Process.Dispose();
    }

    private void ReadExact(ulong address, Span<byte> destination)
    {
        ThrowIfUnavailable();
        if (destination.Length == 0)
        {
            return;
        }

        unsafe
        {
            fixed (byte* buffer = destination)
            {
                var success = NativeMethods.ReadProcessMemory(
                    _handle,
                    ToIntPtr(address),
                    buffer,
                    (nuint)destination.Length,
                    out var bytesRead);

                if (!success || bytesRead != (nuint)destination.Length)
                {
                    throw MemoryAccessException.FromLastWin32Error(
                        $"Expected {destination.Length} bytes but read {bytesRead}",
                        address);
                }
            }
        }
    }

    private ulong ResolveAddress(ulong address, bool absolute, string? moduleName)
    {
        if (absolute)
        {
            return address;
        }

        var baseAddress = MainModuleAddress;
        if (!string.IsNullOrWhiteSpace(moduleName) && !TryGetModuleBase(moduleName, out baseAddress))
        {
            throw new KeyNotFoundException(
                $"Module '{moduleName}' is not loaded in process {Process.ProcessName} ({Process.Id}).");
        }

        return checked(baseAddress + address);
    }

    private bool TryGetModuleBase(string moduleName, out ulong baseAddress)
    {
        lock (_moduleSync)
        {
            if (_moduleBaseAddresses.TryGetValue(moduleName, out baseAddress))
            {
                return true;
            }

            if (DateTime.UtcNow < _nextModuleScan)
            {
                return false;
            }

            // Games such as Unity titles load their DLLs after the process starts; look again before giving up.
            _nextModuleScan = DateTime.UtcNow + ModuleRefreshInterval;
            Process.Refresh();
            _moduleBaseAddresses = LoadModules(Process);
            return _moduleBaseAddresses.TryGetValue(moduleName, out baseAddress);
        }
    }

    private static Dictionary<string, ulong> LoadModules(Process process)
    {
        var modules = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);
        foreach (ProcessModule module in process.Modules)
        {
            if (!string.IsNullOrWhiteSpace(module.ModuleName))
            {
                modules[module.ModuleName] = unchecked((ulong)module.BaseAddress.ToInt64());
            }
        }

        return modules;
    }

    private void ThrowIfUnavailable()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsAlive)
        {
            throw new InvalidOperationException($"Process {Process.Id} is no longer running.");
        }
    }

    private static int FindUnicodeTerminator(ReadOnlySpan<byte> bytes)
    {
        for (var index = 0; index + 1 < bytes.Length; index += 2)
        {
            if (bytes[index] == 0 && bytes[index + 1] == 0)
            {
                return index;
            }
        }

        return bytes.Length - bytes.Length % 2;
    }

    private static int TypeSize<T>() where T : struct
    {
        var type = typeof(T);
        return Marshal.SizeOf(type.IsEnum ? Enum.GetUnderlyingType(type) : type);
    }

    private static IntPtr ToIntPtr(ulong address) => unchecked((IntPtr)(long)address);
}

internal readonly record struct SignaturePattern(byte?[] Bytes)
{
    public int Length => Bytes.Length;

    public static SignaturePattern Parse(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);
        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            throw new FormatException("A signature must contain at least one byte.");
        }

        var bytes = new byte?[tokens.Length];
        for (var index = 0; index < tokens.Length; index++)
        {
            var token = tokens[index];
            if (token is "?" or "??")
            {
                continue;
            }

            if (token.Length != 2 ||
                !byte.TryParse(token, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var value))
            {
                throw new FormatException($"Invalid signature token '{token}' at index {index}.");
            }

            bytes[index] = value;
        }

        return new SignaturePattern(bytes);
    }

    public int IndexOf(ReadOnlySpan<byte> data)
    {
        if (data.Length < Bytes.Length)
        {
            return -1;
        }

        for (var start = 0; start <= data.Length - Bytes.Length; start++)
        {
            var matched = true;
            for (var offset = 0; offset < Bytes.Length; offset++)
            {
                if (Bytes[offset] is { } expected && data[start + offset] != expected)
                {
                    matched = false;
                    break;
                }
            }

            if (matched)
            {
                return start;
            }
        }

        return -1;
    }
}

public sealed class MemoryAccessException : IOException
{
    public MemoryAccessException(string message, int nativeErrorCode = 0)
        : base(nativeErrorCode == 0
            ? message
            : $"{message} (Win32 {nativeErrorCode}: {new Win32Exception(nativeErrorCode).Message})")
    {
        NativeErrorCode = nativeErrorCode;
    }

    public int NativeErrorCode { get; }

    internal static MemoryAccessException FromLastWin32Error(string operation, ulong address)
    {
        var error = Marshal.GetLastWin32Error();
        return new MemoryAccessException($"{operation} at 0x{address:X}.", error);
    }
}

[Flags]
internal enum ProcessAccess : uint
{
    VirtualMemoryRead = 0x0010,
    QueryInformation = 0x0400
}

internal static partial class NativeMethods
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial SafeProcessHandle OpenProcess(
        ProcessAccess desiredAccess,
        [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
        int processId);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static unsafe partial bool ReadProcessMemory(
        SafeProcessHandle process,
        IntPtr baseAddress,
        void* buffer,
        nuint size,
        out nuint bytesRead);
}
