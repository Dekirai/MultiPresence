using MultiPresence.Core;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace MultiPresence
{
    public static class Hypervisor
    {
        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_NOACCESS = 0x01;
        private const uint PAGE_READONLY = 0x02;
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_EXECUTE_READ = 0x20;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint PAGE_GUARD = 0x100;
        private const int SignatureChunkSize = 4 * 1024 * 1024;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, ref int lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern nuint VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, nuint dwLength);

        private sealed record ProcessContext(
            IntPtr Handle,
            System.Diagnostics.Process Process,
            ulong BaseAddress,
            ulong MemoryOffset,
            IReadOnlyDictionary<string, ulong> Modules);

        private static ProcessContext? _context;

        public static System.Diagnostics.Process? Process;
        public static ulong PureAddress;
        public static ulong MemoryOffset;

        public static void AttachProcess(System.Diagnostics.Process input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var handle = input.Handle;
            var mainModule = input.MainModule
                ?? throw new InvalidOperationException($"Process '{input.ProcessName}' has no accessible main module.");
            var baseAddress = (ulong)mainModule.BaseAddress.ToInt64();
            var memoryOffset = baseAddress & 0x7FFF00000000;
            var modules = new Dictionary<string, ulong>(StringComparer.OrdinalIgnoreCase);

            foreach (ProcessModule module in input.Modules)
                modules[module.ModuleName] = (ulong)module.BaseAddress.ToInt64();

            var context = new ProcessContext(handle, input, baseAddress, memoryOffset, modules);
            Interlocked.Exchange(ref _context, context);

            Process = input;
            PureAddress = baseAddress;
            MemoryOffset = memoryOffset;
        }

        public static T Read<T>(ulong Address, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            var context = GetContext();
            return Read<T>(context, Address, Absolute, ModuleName);
        }

        public static bool TryRead<T>(ulong Address, out T value, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            try
            {
                value = Read<T>(Address, Absolute, ModuleName);
                return true;
            }
            catch (Exception ex) when (ex is MemoryReadException or Win32Exception or InvalidOperationException or ArgumentException)
            {
                RateLimitedLogger.Error($"memory-read:{typeof(T).FullName}:{Address:X}", ex);
                value = default;
                return false;
            }
        }

        public static T[] Read<T>(ulong Address, int Size, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            ArgumentOutOfRangeException.ThrowIfNegative(Size);
            if (Size == 0)
                return [];

            var context = GetContext();
            var elementSize = Marshal.SizeOf<T>();
            var byteLength = checked(elementSize * Size);
            var buffer = ReadBytes(context, ResolveAddress(context, Address, Absolute, ModuleName), byteLength, requireExact: true);
            var result = new T[Size];

            for (var i = 0; i < Size; i++)
                result[i] = MemoryMarshal.Read<T>(buffer.AsSpan(i * elementSize, elementSize));

            return result;
        }

        public static void Write<T>(ulong Address, T Value, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            var context = GetContext();
            var buffer = new byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(buffer.AsSpan(), in Value);
            WriteBytes(context, ResolveAddress(context, Address, Absolute, ModuleName), buffer);
        }

        public static void Write<T>(ulong Address, T[] Value, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            ArgumentNullException.ThrowIfNull(Value);
            if (Value.Length == 0)
                return;

            var context = GetContext();
            var elementSize = Marshal.SizeOf<T>();
            var buffer = new byte[checked(elementSize * Value.Length)];

            for (var i = 0; i < Value.Length; i++)
                MemoryMarshal.Write(buffer.AsSpan(i * elementSize, elementSize), in Value[i]);

            WriteBytes(context, ResolveAddress(context, Address, Absolute, ModuleName), buffer);
        }

        public static string ReadString(ulong Address, int length, bool Absolute = false, string? ModuleName = null, bool IsUnicode = false)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            var context = GetContext();
            var address = ResolveAddress(context, Address, Absolute, ModuleName);
            var buffer = ReadBytes(context, address, length, requireExact: false);
            if (buffer.Length == 0)
                return string.Empty;

            if (IsUnicode)
            {
                var usableLength = buffer.Length - buffer.Length % 2;
                var terminator = usableLength;
                for (var i = 0; i < usableLength - 1; i += 2)
                {
                    if (buffer[i] == 0 && buffer[i + 1] == 0)
                    {
                        terminator = i;
                        break;
                    }
                }

                return Encoding.Unicode.GetString(buffer, 0, terminator);
            }

            var terminatorIndex = Array.IndexOf(buffer, (byte)0);
            var byteCount = terminatorIndex >= 0 ? terminatorIndex : buffer.Length;
            return Encoding.UTF8.GetString(buffer, 0, byteCount);
        }

        public static void WriteString(ulong Address, string value, bool Absolute = false, string? ModuleName = null, bool IsUnicode = false)
        {
            ArgumentNullException.ThrowIfNull(value);

            var context = GetContext();
            var address = ResolveAddress(context, Address, Absolute, ModuleName);
            var buffer = IsUnicode
                ? Encoding.Unicode.GetBytes(value + '\0')
                : Encoding.UTF8.GetBytes(value + '\0');
            WriteBytes(context, address, buffer);
        }

        public static byte[] ReadArray(ulong Address, int Length, bool Absolute = false)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(Length);
            if (Length == 0)
                return [];

            var context = GetContext();
            return ReadBytes(context, ResolveAddress(context, Address, Absolute, null), Length, requireExact: true);
        }

        public static void WriteArray(ulong Address, byte[] Value, bool Absolute = false)
        {
            ArgumentNullException.ThrowIfNull(Value);
            if (Value.Length == 0)
                return;

            var context = GetContext();
            WriteBytes(context, ResolveAddress(context, Address, Absolute, null), Value);
        }

        public static ulong GetPointer64(ulong Address, uint[]? Offsets = null, bool Absolute = false, string? ModuleName = null)
        {
            var context = GetContext();
            var returnPoint = Read<ulong>(context, Address, Absolute, ModuleName);
            if (Offsets is null || Offsets.Length == 0)
                return returnPoint;

            for (var i = 0; i < Offsets.Length - 1; i++)
                returnPoint = Read<ulong>(context, returnPoint + Offsets[i], true, null);

            return returnPoint + Offsets[^1];
        }

        public static uint GetPointer32(ulong Address, uint[]? Offsets = null, bool Absolute = false, string? ModuleName = null)
        {
            var context = GetContext();
            var returnPoint = Read<uint>(context, Address, Absolute, ModuleName);
            if (Offsets is null || Offsets.Length == 0)
                return returnPoint;

            for (var i = 0; i < Offsets.Length - 1; i++)
                returnPoint = Read<uint>(context, returnPoint + Offsets[i], true, null);

            return returnPoint + Offsets[^1];
        }

        public static void RedirectInstruction(ulong Address, uint Destination, bool Absolute = false)
        {
            var instructionEnding = checked((uint)Address + 0x07);
            var instructionOffset = Destination - instructionEnding;
            Write(Address + 0x03, BitConverter.GetBytes(instructionOffset), Absolute);
        }

        public static void DeleteInstruction(ulong Address, int Length, bool Absolute = false)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(Length);
            Write(Address, Enumerable.Repeat((byte)0x90, Length).ToArray(), Absolute);
        }

        public static IntPtr FindSignature(string Input)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Input);

            var context = GetContext();
            var pattern = Input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(static value => value == "??" ? -1 : int.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
                .ToArray();

            if (pattern.Length == 0)
                throw new ArgumentException("Signature pattern cannot be empty.", nameof(Input));

            var currentAddress = IntPtr.Zero;
            var mbiSize = (nuint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>();

            while (VirtualQueryEx(context.Handle, currentAddress, out var mbi, mbiSize) != 0)
            {
                if (mbi.State == MEM_COMMIT && IsReadable(mbi.Protect))
                {
                    var result = ScanRegion(context, mbi.BaseAddress, mbi.RegionSize, pattern);
                    if (result != IntPtr.Zero)
                        return result;
                }

                var current = currentAddress.ToInt64();
                var next = checked(mbi.BaseAddress.ToInt64() + (long)mbi.RegionSize);
                if (next <= current)
                    break;

                currentAddress = new IntPtr(next);
            }

            throw new InvalidDataException("ERROR: Signature scan error -- No results found!");
        }

        public static void UnlockBlock(ulong Address, bool Absolute = false, string? ModuleName = null)
        {
            var context = GetContext();
            var address = ResolveAddress(context, Address, Absolute, ModuleName);
            var oldProtect = 0;
            if (!VirtualProtectEx(context.Handle, address, 0x100000, PAGE_EXECUTE_READWRITE, ref oldProtect))
                throw new Win32Exception(Marshal.GetLastWin32Error(), $"VirtualProtectEx failed at 0x{address.ToInt64():X}.");
        }

        private static T Read<T>(ProcessContext context, ulong address, bool absolute, string? moduleName) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var buffer = ReadBytes(context, ResolveAddress(context, address, absolute, moduleName), size, requireExact: true);
            return MemoryMarshal.Read<T>(buffer);
        }

        private static byte[] ReadBytes(ProcessContext context, IntPtr address, int length, bool requireExact)
        {
            var buffer = new byte[length];
            var bytesRead = 0;
            var success = ReadProcessMemory(context.Handle, address, buffer, length, ref bytesRead);

            if (bytesRead <= 0 || (requireExact && (!success || bytesRead != length)))
            {
                throw new MemoryReadException(
                    context.Process.ProcessName,
                    address,
                    length,
                    bytesRead,
                    Marshal.GetLastWin32Error());
            }

            if (bytesRead == buffer.Length)
                return buffer;

            return buffer.AsSpan(0, bytesRead).ToArray();
        }

        private static void WriteBytes(ProcessContext context, IntPtr address, byte[] buffer)
        {
            var bytesWritten = 0;
            var success = WriteProcessMemory(context.Handle, address, buffer, buffer.Length, ref bytesWritten);
            if (!success || bytesWritten != buffer.Length)
            {
                throw new Win32Exception(
                    Marshal.GetLastWin32Error(),
                    $"WriteProcessMemory failed for process '{context.Process.ProcessName}' at 0x{address.ToInt64():X}. Requested {buffer.Length} bytes, wrote {bytesWritten}.");
            }
        }

        private static IntPtr ResolveAddress(ProcessContext context, ulong address, bool absolute, string? moduleName)
        {
            if (absolute)
                return new IntPtr(unchecked((long)address));

            var baseAddress = moduleName is not null && context.Modules.TryGetValue(moduleName, out var moduleBase)
                ? moduleBase
                : context.BaseAddress;
            return new IntPtr(unchecked((long)(baseAddress + address)));
        }

        private static ProcessContext GetContext()
            => Volatile.Read(ref _context)
                ?? throw new InvalidOperationException("Hypervisor is not attached to a process.");

        private static bool IsReadable(uint protection)
        {
            if ((protection & PAGE_GUARD) != 0 || (protection & PAGE_NOACCESS) != 0)
                return false;

            var baseProtection = protection & 0xFF;
            return baseProtection is PAGE_READONLY or PAGE_READWRITE or PAGE_EXECUTE_READ or PAGE_EXECUTE_READWRITE;
        }

        private static IntPtr ScanRegion(ProcessContext context, IntPtr baseAddress, nuint regionSize, int[] pattern)
        {
            ulong offset = 0;
            var totalSize = (ulong)regionSize;
            var carry = Array.Empty<byte>();

            while (offset < totalSize)
            {
                var requested = (int)Math.Min((ulong)SignatureChunkSize, totalSize - offset);
                var chunk = new byte[requested];
                var bytesRead = 0;
                var address = new IntPtr(checked(baseAddress.ToInt64() + (long)offset));

                if (!ReadProcessMemory(context.Handle, address, chunk, requested, ref bytesRead) || bytesRead <= 0)
                {
                    offset += (ulong)requested;
                    carry = Array.Empty<byte>();
                    continue;
                }

                var scanBuffer = new byte[carry.Length + bytesRead];
                carry.CopyTo(scanBuffer, 0);
                Buffer.BlockCopy(chunk, 0, scanBuffer, carry.Length, bytesRead);

                var index = FindPattern(scanBuffer, pattern);
                if (index >= 0)
                {
                    var resultOffset = checked((long)offset - carry.Length + index);
                    return new IntPtr(checked(baseAddress.ToInt64() + resultOffset));
                }

                var carryLength = Math.Min(pattern.Length - 1, scanBuffer.Length);
                carry = carryLength == 0 ? Array.Empty<byte>() : scanBuffer.AsSpan(scanBuffer.Length - carryLength, carryLength).ToArray();
                offset += (ulong)requested;
            }

            return IntPtr.Zero;
        }

        private static int FindPattern(ReadOnlySpan<byte> buffer, int[] pattern)
        {
            if (pattern.Length > buffer.Length)
                return -1;

            for (var offset = 0; offset <= buffer.Length - pattern.Length; offset++)
            {
                var matched = true;
                for (var index = 0; index < pattern.Length; index++)
                {
                    if (pattern[index] != -1 && pattern[index] != buffer[offset + index])
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                    return offset;
            }

            return -1;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public nuint RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
    }

    public sealed class MemoryReadException : IOException
    {
        public MemoryReadException(string processName, IntPtr address, int requestedBytes, int actualBytes, int nativeError)
            : base($"ReadProcessMemory failed for process '{processName}' at 0x{address.ToInt64():X}. Requested {requestedBytes} bytes, read {actualBytes}. Win32 error: {nativeError}.")
        {
            ProcessName = processName;
            Address = address;
            RequestedBytes = requestedBytes;
            ActualBytes = actualBytes;
            NativeError = nativeError;
        }

        public string ProcessName { get; }
        public IntPtr Address { get; }
        public int RequestedBytes { get; }
        public int ActualBytes { get; }
        public int NativeError { get; }
    }
}
