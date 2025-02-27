using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace MultiPresence
{
    public static class Hypervisor
    {
        //Hypervisor was originally created by TopazTK
        //It got modified by Dekirai to make it properly work with MultiPresence

        //Things changed/added:
        //- Read/Write to a module of a process, for example "GameAssembly.dll"
        //- Read/Write Strings (Only Read has been tested)
        //- Changed how "FindSignature" works, to find pattern across all memory regions of the process

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, ref int lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        static IntPtr Handle;
        public static Process? Process;
        public static ulong PureAddress;
        public static ulong MemoryOffset;

        static byte[]? _patternBuffer = null;

        static Dictionary<string, ulong> ModuleBaseAddresses = new Dictionary<string, ulong>();

        /// <summary>
        /// Initialize the Hypervisor on a process.
        /// </summary>
        /// <param name="Input">The input process.</param>
        public static void AttachProcess(Process Input)
        {
            Process = Input;
            Handle = Input.Handle;
            PureAddress = (ulong)Input.MainModule.BaseAddress;
            MemoryOffset = PureAddress & 0x7FFF00000000;

            // Store the base address of all modules
            foreach (ProcessModule module in Input.Modules)
            {
                ModuleBaseAddresses[module.ModuleName] = (ulong)module.BaseAddress;
            }
        }

        /// <summary>
        /// Reads a value with the type of T from an address.
        /// Unsafe, must be used with caution.
        /// </summary>
        /// <typeparam name="T">Type of the value to read.</typeparam>
        /// <param name="Address">The address of the value to read.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        /// <returns>The value as it is read from memory.</returns>
        public static T Read<T>(ulong Address, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            var _dynoMethod = new DynamicMethod("SizeOfType", typeof(int), []);
            ILGenerator _ilGen = _dynoMethod.GetILGenerator();

            _ilGen.Emit(OpCodes.Sizeof, typeof(T));
            _ilGen.Emit(OpCodes.Ret);

            var _outSize = (int)_dynoMethod.Invoke(null, null);

            var _outArray = new byte[_outSize];
            int _outRead = 0;

            ReadProcessMemory(Handle, _address, _outArray, _outSize, ref _outRead);

            var _outType = typeof(T);

            if (_outType.IsEnum)
            {
                var _gcHandle = GCHandle.Alloc(_outArray, GCHandleType.Pinned);
                var _retData = (T)Marshal.PtrToStructure(_gcHandle.AddrOfPinnedObject(), Enum.GetUnderlyingType(_outType));

                _gcHandle.Free();

                return _retData;
            }

            else
            {
                var _gcHandle = GCHandle.Alloc(_outArray, GCHandleType.Pinned);
                var _retData = (T)Marshal.PtrToStructure(_gcHandle.AddrOfPinnedObject(), typeof(T));

                _gcHandle.Free();

                return _retData;
            }
        }

        /// <summary>
        /// Reads an array with the type of T[] from an address.
        /// Unsafe, must be used with caution.
        /// </summary>
        /// <typeparam name="T">Type of the array to read.</typeparam>
        /// <param name="Address">The address of the value to read.</param>
        /// <param name="Size">The size of the array to read.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        /// <returns>The array as it is read from memory.</returns>
        public static T[] Read<T>(ulong Address, int Size, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            var _dynoMethod = new DynamicMethod("SizeOfType", typeof(int), Type.EmptyTypes);
            ILGenerator _ilGen = _dynoMethod.GetILGenerator();

            _ilGen.Emit(OpCodes.Sizeof, typeof(T));
            _ilGen.Emit(OpCodes.Ret);

            var _outSize = (int)_dynoMethod.Invoke(null, null);
            var _outArray = new byte[Size * _outSize];
            int _outRead = 0;

            ReadProcessMemory(Handle, _address, _outArray, Size * _outSize, ref _outRead);

            var _outType = typeof(T);

            if (_outType.IsEnum)
            {
                var _enumType = Enum.GetUnderlyingType(_outType);
                var _retArray = Array.CreateInstance(_enumType, Size);

                for (int i = 0; i < Size; i++)
                {
                    var _pickArray = _outArray.Skip(i * _outSize).Take(_outSize).ToArray();

                    var _gcHandle = GCHandle.Alloc(_pickArray, GCHandleType.Pinned);
                    var _convData = (T)Marshal.PtrToStructure(_gcHandle.AddrOfPinnedObject(), _enumType);

                    _retArray.SetValue(_convData, i);
                    _gcHandle.Free();
                }

                var _convArray = new T[Size];
                _retArray.CopyTo(_convArray, 0);

                return _convArray;
            }
            else
            {
                var _retArray = new T[Size];

                for (int i = 0; i < Size; i++)
                {
                    var _pickArray = _outArray.Skip(i * _outSize).Take(_outSize).ToArray();

                    var _gcHandle = GCHandle.Alloc(_pickArray, GCHandleType.Pinned);
                    var _convData = (T)Marshal.PtrToStructure(_gcHandle.AddrOfPinnedObject(), typeof(T));

                    _retArray[i] = _convData;
                    _gcHandle.Free();
                }

                return _retArray;
            }
        }

        /// <summary>
        /// Writes a value with the type of T to an address.
        /// Unsafe, must be used with caution.
        /// </summary>
        /// <typeparam name="T">Type of the value to write. Must have a size.</typeparam>
        /// <param name="Address">The address which the value will be written to.</param>
        /// <param name="Value">The value to write.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        public static void Write<T>(ulong Address, T Value, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            var _dynoMethod = new DynamicMethod("SizeOfType", typeof(int), []);
            ILGenerator _ilGen = _dynoMethod.GetILGenerator();

            _ilGen.Emit(OpCodes.Sizeof, typeof(T));
            _ilGen.Emit(OpCodes.Ret);

            var _inSize = (int)_dynoMethod.Invoke(null, null);
            int _inWrite = 0;

            if (_inSize > 1)
            {
                var _inArray = (byte[])typeof(BitConverter).GetMethod("GetBytes", new[] { typeof(T) }).Invoke(null, new object[] { Value });
                WriteProcessMemory(Handle, _address, _inArray, _inArray.Length, ref _inWrite);
            }

            else
            {
                var _inArray = new byte[] { (byte)Convert.ChangeType(Value, typeof(byte)) };
                WriteProcessMemory(Handle, _address, _inArray, _inArray.Length, ref _inWrite);
            }
        }

        /// <summary>
        /// Writes an array with the type of T to an address.
        /// Unsafe, must be used with caution.
        /// </summary>
        /// <typeparam name="T">Type of the array to write. Must have a size.</typeparam>
        /// <param name="Address">The address which the Array will be written to.</param>
        /// <param name="Value">The array to write.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        public static void Write<T>(ulong Address, T[] Value, bool Absolute = false, string? ModuleName = null) where T : struct
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            var _dynoMethod = new DynamicMethod("SizeOfType", typeof(int), []);
            ILGenerator _ilGen = _dynoMethod.GetILGenerator();

            _ilGen.Emit(OpCodes.Sizeof, typeof(T));
            _ilGen.Emit(OpCodes.Ret);

            var _inSize = (int)_dynoMethod.Invoke(null, null);
            int _inWrite = 0;

            if (_inSize > 1)
            {
                for (int i = 0; i < Value.Length; i++)
                {
                    var _inArray = (byte[])typeof(BitConverter).GetMethod("GetBytes", [typeof(T)]).Invoke(null, [Value[i]]);
                    WriteProcessMemory(Handle, _address + _inSize * i, _inArray, _inArray.Length, ref _inWrite);
                }
            }

            else
                WriteProcessMemory(Handle, _address, Value as byte[], Value.Length, ref _inWrite);
        }

        /// <summary>
        /// Reads a string from a memory address, with optional Unicode support.
        /// </summary>
        /// <param name="Address">The address of the string to read.</param>
        /// <param name="length">The maximum length of the string to read.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        /// <param name="IsUnicode">If true, reads the string as Unicode (UTF-16). Default is false (UTF-8).</param>
        /// <returns>The string read from memory.</returns>
        public static string ReadString(ulong Address, int length, bool Absolute = false, string? ModuleName = null, bool IsUnicode = false)
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            var buffer = new byte[length];
            int bytesRead = 0;

            ReadProcessMemory(Handle, _address, buffer, length, ref bytesRead);

            if (bytesRead == 0)
                return string.Empty;

            if (IsUnicode)
            {
                for (int i = 0; i < bytesRead - 1; i += 2)
                {
                    if (buffer[i] == 0 && buffer[i + 1] == 0)
                    {
                        bytesRead = i;
                        break;
                    }
                }

                return Encoding.Unicode.GetString(buffer, 0, bytesRead);
            }
            else
            {
                // Find the first single-byte null terminator
                int terminatorIndex = Array.IndexOf(buffer, (byte)0);
                if (terminatorIndex >= 0)
                    bytesRead = terminatorIndex;

                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Writes a string to a memory address, with optional Unicode support.
        /// </summary>
        /// <param name="Address">The address to write the string to.</param>
        /// <param name="value">The string to write.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to write to. Defaults to main executable.</param>
        /// <param name="IsUnicode">If true, writes the string as Unicode (UTF-16). Default is false (UTF-8).</param>
        public static void WriteString(ulong Address, string value, bool Absolute = false, string? ModuleName = null, bool IsUnicode = false)
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            byte[] buffer;

            if (IsUnicode)
            {
                buffer = Encoding.Unicode.GetBytes(value + '\0');
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes(value + '\0');
            }

            int bytesWritten = 0;
            WriteProcessMemory(Handle, _address, buffer, buffer.Length, ref bytesWritten);
        }

        /// <summary>
        /// Reads a byte array from an address.
        /// </summary>
        /// <param name="Address">The address which the value will be read from.</param>
        /// <param name="Length">The length of the array to read.</param>
        /// <param name="Absolute">Whether the address is an absolute address or not. Defaults to false.</param>
        /// <returns></returns>
        public static byte[] ReadArray(ulong Address, int Length, bool Absolute = false)
        {
            IntPtr _address = (IntPtr)Address;

            if (Absolute)
                _address = (IntPtr)(Address);

            var _outArray = new byte[Length];
            int _outRead = 0;

            ReadProcessMemory(Handle, _address, _outArray, Length, ref _outRead);

            return _outArray;
        }

        /// <summary>
        /// Writes a byte array to an address.
        /// </summary>
        /// <param name="Address">The address which the value will be written to.</param>
        /// <param name="Value">The array to write.</param>
        /// <param name="Absolute">Whether the address is an absolute address or not. Defaults to false.</param>
        public static void WriteArray(ulong Address, byte[] Value, bool Absolute = false)
        {
            IntPtr _address = (IntPtr)Address;

            if (Absolute)
                _address = (IntPtr)(Address);

            int _inWrite = 0;

            WriteProcessMemory(Handle, _address, Value, Value.Length, ref _inWrite);
        }

        /// <summary>
        /// Calculated a 64-bit pointer with the given offsets.
        /// All offsets are added and the resulting address is read.
        /// </summary>
        /// <param name="Address">The starting point to the pointer.</param>
        /// <param name="Offsets">All the offsets of the pointer, null by default.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        /// <returns>The final calculated pointer.</returns>
        public static ulong GetPointer64(ulong Address, uint[]? Offsets = null, bool Absolute = false, string? ModuleName = null)
        {
            var _returnPoint = Read<ulong>(Address, Absolute, ModuleName);

            if (Offsets == null)
                return _returnPoint;

            for (int i = 0; i < Offsets.Length - 1; i++)
            {
                _returnPoint = Read<ulong>(_returnPoint + Offsets[i], true);
            }

            return _returnPoint + Offsets.Last();
        }

        /// <summary>
        /// Calculated a 32-bit pointer with the given offsets.
        /// All offsets are added and the resulting address is read.
        /// </summary>
        /// <param name="Address">The starting point to the pointer.</param>
        /// <param name="Offsets">All the offsets of the pointer, null by default.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        /// <returns>The final calculated pointer.</returns>
        public static uint GetPointer32(ulong Address, uint[]? Offsets = null, bool Absolute = false, string? ModuleName = null)
        {
            var _returnPoint = Read<uint>(Address, Absolute, ModuleName);

            if (Offsets == null)
                return _returnPoint;

            for (int i = 0; i < Offsets.Length - 1; i++)
            {
                _returnPoint = Read<uint>(_returnPoint + Offsets[i], true);
            }

            return _returnPoint + Offsets.Last();
        }

        public static void RedirectInstruction(ulong Address, uint Destination, bool Absolute = false)
        {
            var _instEnding = (uint)Address + 0x07;
            var _instMath = Destination - _instEnding;
            Write(Address + 0x03, BitConverter.GetBytes(_instMath), Absolute);
        }

        public static void DeleteInstruction(ulong Address, int Length, bool Absolute = false)
        {
            Write(Address, Enumerable.Repeat<byte>(0x90, Length).ToArray(), Absolute);
        }

        public static IntPtr FindSignature(string Input)
        {
            var _sigBytes = Input.Split(' ').Select(s => s == "??" ? -1 : int.Parse(s, NumberStyles.HexNumber)).ToArray();

            IntPtr currentAddress = IntPtr.Zero;
            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();
            const int ChunkSize = 0x2000000; // 20 MB
            IntPtr firstResult = IntPtr.Zero;

            while (VirtualQueryEx(Handle, currentAddress, out mbi, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))) != 0)
            {
                if (mbi.Protect == PAGE_READWRITE || mbi.Protect == PAGE_READONLY || mbi.Protect == PAGE_EXECUTE_READ || mbi.Protect == PAGE_EXECUTE_READWRITE)
                {
                    ulong regionSize = (ulong)mbi.RegionSize;
                    if (regionSize > int.MaxValue)
                        regionSize = int.MaxValue;

                    ulong totalBytesRead = 0;
                    var regionBuffer = new byte[ChunkSize];

                    while (totalBytesRead < regionSize)
                    {
                        ulong bytesToRead = Math.Min((ulong)ChunkSize, regionSize - totalBytesRead);
                        int bytesRead = 0;

                        if (ReadProcessMemory(Handle, IntPtr.Add(mbi.BaseAddress, (int)totalBytesRead), regionBuffer, (int)bytesToRead, ref bytesRead))
                        {
                            Parallel.For(0, bytesRead, (a, state) =>
                            {
                                if (a + _sigBytes.Length > bytesRead) return;

                                bool matchFound = true;
                                for (int b = 0; b < _sigBytes.Length; b++)
                                {
                                    if (_sigBytes[b] != -1 && _sigBytes[b] != regionBuffer[a + b])
                                    {
                                        matchFound = false;
                                        break;
                                    }
                                }

                                object lockObject = new object();

                                if (matchFound)
                                {
                                    lock (lockObject)
                                    {
                                        if (firstResult == IntPtr.Zero)
                                        {
                                            firstResult = IntPtr.Add(mbi.BaseAddress, (int)(totalBytesRead + (ulong)a));
                                            state.Stop();
                                        }
                                    }
                                }
                            });
                        }

                        totalBytesRead += bytesToRead;
                        if (firstResult != IntPtr.Zero)
                            break;
                    }
                }

                currentAddress = new IntPtr((long)mbi.BaseAddress + (long)mbi.RegionSize);
                if (firstResult != IntPtr.Zero)
                    break;
            }

            if (firstResult == IntPtr.Zero)
                throw new InvalidDataException("ERROR: Signature scan error -- No results found!");

            return firstResult;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        private const uint PAGE_READONLY = 0x02;
        private const uint PAGE_READWRITE = 0x04;
        private const uint PAGE_EXECUTE_READ = 0x20;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        /// <summary>
        /// Unlocks a particular block to be written.
        /// </summary>
        /// <param name="Address">The address of the subject block.</param>
        /// <param name="Absolute">If the address is absolute, false by default.</param>
        /// <param name="ModuleName">The name of the module to read from. Defaults to main executable.</param>
        public static void UnlockBlock(ulong Address, bool Absolute = false, string? ModuleName = null)
        {
            var _address = (IntPtr)Address;

            if (!Absolute)
            {
                ulong baseAddress = ModuleName != null && ModuleBaseAddresses.ContainsKey(ModuleName)
                    ? ModuleBaseAddresses[ModuleName]
                    : PureAddress;

                _address = (IntPtr)(baseAddress + Address);
            }

            int _oldProtect = 0;
            VirtualProtectEx(Handle, _address, 0x100000, 0x40, ref _oldProtect);
        }
    }
}