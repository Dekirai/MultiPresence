namespace MultiPresence.Models.FFRS
{
    internal static partial class MemoryReader
    {
        private static ulong subsystemInstance;
        private static int subsystemMap;

        private static void ResetSubsystems()
        {
            subsystemInstance = 0;
            subsystemMap = 0;
        }

        private static ulong Subsystem(ulong instance, string name)
        {
            // UGameInstance's subsystem collection maps each subsystem class to its instance.
            if (instance != subsystemInstance || subsystemMap == 0)
            {
                subsystemInstance = instance;
                subsystemMap = FindSubsystemMap(instance);
            }
            if (subsystemMap == 0) return 0;
            foreach (int entry in Entries(instance + (ulong)subsystemMap, 24, out var pairs))
            {
                ulong type = BitConverter.ToUInt64(pairs, entry);
                ulong subsystem = BitConverter.ToUInt64(pairs, entry + 8);
                if (type != 0 && Name(type) == name && Pointer(subsystem + 0x10) == type && Pointer(subsystem + 0x20) == instance) return subsystem;
            }
            return 0;
        }

        private static int FindSubsystemMap(ulong instance)
        {
            int size = StructSize(Pointer(instance + 0x10));
            if (size is < 0x100 or > 0x10000) throw Invalid("game instance size");
            var bytes = Hypervisor.ReadArray(instance, size, true);
            for (int offset = 0x28; offset + 0x30 <= size; offset += 8)
            {
                ulong data = BitConverter.ToUInt64(bytes, offset);
                int slots = BitConverter.ToInt32(bytes, offset + 8);
                int capacity = BitConverter.ToInt32(bytes, offset + 12);
                ulong flags = BitConverter.ToUInt64(bytes, offset + 0x20);
                if (data == 0 || slots is < 1 or > 256 || slots > capacity || BitConverter.ToInt32(bytes, offset + 0x28) != slots ||
                    (flags == 0 && slots > 128) || !Hypervisor.TryRead<ulong>(data, out _, true)) continue;

                bool valid = false;
                foreach (int entry in Entries(instance + (ulong)offset, 24, out var pairs))
                {
                    ulong type = BitConverter.ToUInt64(pairs, entry);
                    ulong subsystem = BitConverter.ToUInt64(pairs, entry + 8);
                    valid = type != 0 && Hypervisor.TryRead<ulong>(subsystem + 0x10, out var owner, true) && owner == type &&
                        Hypervisor.TryRead<ulong>(subsystem + 0x20, out var outer, true) && outer == instance;
                    if (!valid) break;
                }
                if (valid) return offset;
            }
            return 0;
        }

        private static List<int> Entries(ulong set, int stride, out byte[] elements)
        {
            // TSet/TMap: element array at +0, allocation bits inline at +0x10 or allocated at +0x20, bit count at +0x28.
            ulong data = Pointer(set);
            int count = Hypervisor.Read<int>(set + 8, true);
            var result = new List<int>();
            elements = Array.Empty<byte>();
            if (data == 0 || count is < 1 or > 4096 || Hypervisor.Read<int>(set + 0x28, true) != count) return result;
            ulong flags = Pointer(set + 0x20);
            if (flags == 0 && count > 128) return result;
            var occupied = Hypervisor.ReadArray(flags == 0 ? set + 0x10 : flags, (count + 7) / 8, true);
            elements = Hypervisor.ReadArray(data, count * stride, true);
            for (int index = 0; index < count; index++)
            {
                if ((occupied[index / 8] & (1 << (index % 8))) != 0) result.Add(index * stride);
            }
            return result;
        }

        private static int Point(ulong viewModel, string name)
        {
            int field = Field(viewModel, name);
            return field == 0 ? -1 : Hypervisor.Read<int>(viewModel + (ulong)field, true);
        }
    }
}
