using Microsoft.Win32.SafeHandles;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace MultiPresence;

/// <summary>
/// Thread-safe compatibility facade for the Hypervisor process-memory library.
/// A single session is sufficient because MultiPresence runs one game integration at a time.
/// </summary>
public static class Hypervisor
{
    private static readonly ReaderWriterLockSlim SessionLock = new(LockRecursionPolicy.NoRecursion);
    private static HypervisorSession? _session;

    public static Process? Process => WithSession(static session => session.Process);
    public static ulong PureAddress => WithSession(static session => session.MainModuleAddress);
    public static ulong MemoryOffset => PureAddress & 0x7FFF00000000;
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

    public static void Write<T>(ulong address, T value, bool absolute = false, string? moduleName = null)
        where T : struct =>
        WithSession(session =>
        {
            session.Write(address, value, absolute, moduleName);
            return true;
        });

    public static void Write<T>(ulong address, T[] value, bool absolute = false, string? moduleName = null)
        where T : struct =>
        WithSession(session =>
        {
            session.WriteArray(address, value, absolute, moduleName);
            return true;
        });

    public static string ReadString(
        ulong address,
        int length,
        bool absolute = false,
        string? moduleName = null,
        bool isUnicode = false) =>
        WithSession(session => session.ReadString(address, length, absolute, moduleName, isUnicode));

    public static void WriteString(
        ulong address,
        string value,
        bool absolute = false,
        string? moduleName = null,
        bool isUnicode = false) =>
        WithSession(session =>
        {
            session.WriteString(address, value, absolute, moduleName, isUnicode);
            return true;
        });

    public static byte[] ReadArray(ulong address, int length, bool absolute = false) =>
        WithSession(session => session.ReadBytes(address, length, absolute));

    public static void WriteArray(ulong address, byte[] value, bool absolute = false) =>
        WithSession(session =>
        {
            session.WriteBytes(address, value, absolute);
            return true;
        });

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

    public static IntPtr FindSignature(string input) =>
        WithSession(session => session.FindSignature(input));

    public static void RedirectInstruction(ulong address, uint destination, bool absolute = false)
    {
        var instructionEnd = checked((uint)address + 0x07);
        var displacement = destination - instructionEnd;
        WriteArray(address + 0x03, BitConverter.GetBytes(displacement), absolute);
    }

    public static void DeleteInstruction(ulong address, int length, bool absolute = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        WriteArray(address, Enumerable.Repeat((byte)0x90, length).ToArray(), absolute);
    }

    public static void UnlockBlock(ulong address, bool absolute = false, string? moduleName = null) =>
        WithSession(session =>
        {
            session.UnlockBlock(address, absolute, moduleName);
            return true;
        });

    internal static int FindPattern(ReadOnlySpan<byte> data, string signature) =>
        SignaturePattern.Parse(signature).IndexOf(data);

    private static T WithSession<T>(Func<HypervisorSession, T> action)
    {
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
    private const int ScanChunkSize = 1024 * 1024;
    private readonly SafeProcessHandle _handle;
    private readonly Dictionary<string, ulong> _moduleBaseAddresses;
    private bool _disposed;

    private HypervisorSession(Process process, SafeProcessHandle handle, bool canWrite)
    {
        Process = process;
        _handle = handle;
        CanWrite = canWrite;

        var mainModule = process.MainModule
            ?? throw new InvalidOperationException($"Unable to resolve the main module for process {process.Id}.");

        MainModuleAddress = unchecked((ulong)mainModule.BaseAddress.ToInt64());
        _moduleBaseAddresses = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);

        foreach (ProcessModule module in process.Modules)
        {
            if (!string.IsNullOrWhiteSpace(module.ModuleName))
            {
                _moduleBaseAddresses[module.ModuleName] = unchecked((ulong)module.BaseAddress.ToInt64());
            }
        }
    }

    public Process Process { get; }
    public ulong MainModuleAddress { get; }
    public bool CanWrite { get; }

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

        var access = ProcessAccess.QueryInformation |
                     ProcessAccess.VirtualMemoryRead |
                     ProcessAccess.VirtualMemoryWrite |
                     ProcessAccess.VirtualMemoryOperation;
        var handle = NativeMethods.OpenProcess(access, false, processId);
        var canWrite = true;

        if (handle.IsInvalid)
        {
            handle.Dispose();
            canWrite = false;
            handle = NativeMethods.OpenProcess(
                ProcessAccess.QueryInformation | ProcessAccess.VirtualMemoryRead,
                false,
                processId);
        }

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
            return new HypervisorSession(Process.GetProcessById(processId), handle, canWrite);
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

    public void Write<T>(ulong address, T value, bool absolute, string? moduleName)
        where T : struct
    {
        var bytes = new byte[TypeSize<T>()];
        MemoryMarshal.Write(bytes.AsSpan(), in value);
        WriteExact(ResolveAddress(address, absolute, moduleName), bytes);
    }

    public void WriteArray<T>(ulong address, T[] values, bool absolute, string? moduleName)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(values);
        var elementSize = TypeSize<T>();
        var bytes = new byte[checked(elementSize * values.Length)];

        for (var index = 0; index < values.Length; index++)
        {
            var value = values[index];
            MemoryMarshal.Write(bytes.AsSpan(index * elementSize, elementSize), in value);
        }

        WriteExact(ResolveAddress(address, absolute, moduleName), bytes);
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

    public void WriteString(ulong address, string value, bool absolute, string? moduleName, bool isUnicode)
    {
        ArgumentNullException.ThrowIfNull(value);
        var encoding = isUnicode ? Encoding.Unicode : Encoding.UTF8;
        var bytes = encoding.GetBytes(value + '\0');
        WriteExact(ResolveAddress(address, absolute, moduleName), bytes);
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

    public void WriteBytes(ulong address, byte[] value, bool absolute)
    {
        ArgumentNullException.ThrowIfNull(value);
        WriteExact(ResolveAddress(address, absolute, null), value);
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

    public IntPtr FindSignature(string signature)
    {
        var pattern = SignaturePattern.Parse(signature);
        ulong currentAddress = 0;
        var informationSize = (nuint)Marshal.SizeOf<MemoryBasicInformation>();

        while (NativeMethods.VirtualQueryEx(
                   _handle,
                   ToIntPtr(currentAddress),
                   out var information,
                   informationSize) != 0)
        {
            var baseAddress = unchecked((ulong)information.BaseAddress.ToInt64());
            var regionSize = information.RegionSize.ToUInt64();

            if (information.State == MemoryState.Commit && IsReadable(information.Protect))
            {
                var result = ScanRegion(baseAddress, regionSize, pattern);
                if (result is { } match)
                {
                    return ToIntPtr(match);
                }
            }

            var nextAddress = baseAddress + regionSize;
            if (nextAddress <= currentAddress)
            {
                break;
            }

            currentAddress = nextAddress;
        }

        throw new InvalidDataException($"Signature was not found in process {Process.Id}: {signature}");
    }

    public void UnlockBlock(ulong address, bool absolute, string? moduleName)
    {
        EnsureWritable();
        var resolved = ResolveAddress(address, absolute, moduleName);
        if (!NativeMethods.VirtualProtectEx(
                _handle,
                ToIntPtr(resolved),
                0x100000,
                MemoryProtection.ExecuteReadWrite,
                out _))
        {
            throw MemoryAccessException.FromLastWin32Error("Could not change memory protection", resolved);
        }
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

    private ulong? ScanRegion(ulong baseAddress, ulong regionSize, SignaturePattern pattern)
    {
        var overlap = Math.Max(0, pattern.Length - 1);
        var rented = ArrayPool<byte>.Shared.Rent(ScanChunkSize + overlap);
        var carried = 0;
        ulong offset = 0;

        try
        {
            while (offset < regionSize)
            {
                var requested = (int)Math.Min((ulong)ScanChunkSize, regionSize - offset);
                var destination = rented.AsSpan(carried, requested);
                var bytesRead = ReadAvailable(checked(baseAddress + offset), destination);
                if (bytesRead <= 0)
                {
                    carried = 0;
                    offset += (ulong)requested;
                    continue;
                }

                var searchable = rented.AsSpan(0, carried + bytesRead);
                var index = pattern.IndexOf(searchable);
                if (index >= 0)
                {
                    return checked(baseAddress + offset - (ulong)carried + (ulong)index);
                }

                carried = Math.Min(overlap, searchable.Length);
                if (carried > 0)
                {
                    searchable[^carried..].CopyTo(rented);
                }

                offset += (ulong)requested;
            }

            return null;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private int ReadAvailable(ulong address, Span<byte> destination)
    {
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

                if (!success && bytesRead == 0)
                {
                    return 0;
                }

                return checked((int)bytesRead);
            }
        }
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

    private void WriteExact(ulong address, ReadOnlySpan<byte> source)
    {
        ThrowIfUnavailable();
        EnsureWritable();
        if (source.Length == 0)
        {
            return;
        }

        unsafe
        {
            fixed (byte* buffer = source)
            {
                var success = NativeMethods.WriteProcessMemory(
                    _handle,
                    ToIntPtr(address),
                    buffer,
                    (nuint)source.Length,
                    out var bytesWritten);

                if (!success || bytesWritten != (nuint)source.Length)
                {
                    throw MemoryAccessException.FromLastWin32Error(
                        $"Expected to write {source.Length} bytes but wrote {bytesWritten}",
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
        if (!string.IsNullOrWhiteSpace(moduleName) &&
            !_moduleBaseAddresses.TryGetValue(moduleName, out baseAddress))
        {
            throw new KeyNotFoundException(
                $"Module '{moduleName}' is not loaded in process {Process.ProcessName} ({Process.Id}).");
        }

        return checked(baseAddress + address);
    }

    private void ThrowIfUnavailable()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!IsAlive)
        {
            throw new InvalidOperationException($"Process {Process.Id} is no longer running.");
        }
    }

    private void EnsureWritable()
    {
        if (!CanWrite)
        {
            throw new UnauthorizedAccessException(
                "The process was attached in read-only mode; writing memory is not permitted.");
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

    private static bool IsReadable(MemoryProtection protection)
    {
        if ((protection & (MemoryProtection.Guard | MemoryProtection.NoAccess)) != 0)
        {
            return false;
        }

        const MemoryProtection readable =
            MemoryProtection.ReadOnly |
            MemoryProtection.ReadWrite |
            MemoryProtection.WriteCopy |
            MemoryProtection.ExecuteRead |
            MemoryProtection.ExecuteReadWrite |
            MemoryProtection.ExecuteWriteCopy;

        return (protection & readable) != 0;
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
    VirtualMemoryOperation = 0x0008,
    VirtualMemoryRead = 0x0010,
    VirtualMemoryWrite = 0x0020,
    QueryInformation = 0x0400
}

internal enum MemoryState : uint
{
    Commit = 0x1000
}

[Flags]
internal enum MemoryProtection : uint
{
    NoAccess = 0x01,
    ReadOnly = 0x02,
    ReadWrite = 0x04,
    WriteCopy = 0x08,
    ExecuteRead = 0x20,
    ExecuteReadWrite = 0x40,
    ExecuteWriteCopy = 0x80,
    Guard = 0x100
}

[StructLayout(LayoutKind.Sequential)]
internal struct MemoryBasicInformation
{
    public IntPtr BaseAddress;
    public IntPtr AllocationBase;
    public MemoryProtection AllocationProtect;
    public ushort PartitionId;
    public UIntPtr RegionSize;
    public MemoryState State;
    public MemoryProtection Protect;
    public uint Type;
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

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static unsafe partial bool WriteProcessMemory(
        SafeProcessHandle process,
        IntPtr baseAddress,
        void* buffer,
        nuint size,
        out nuint bytesWritten);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nuint VirtualQueryEx(
        SafeProcessHandle process,
        IntPtr address,
        out MemoryBasicInformation information,
        nuint informationLength);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualProtectEx(
        SafeProcessHandle process,
        IntPtr address,
        nuint size,
        MemoryProtection newProtection,
        out MemoryProtection oldProtection);
}
