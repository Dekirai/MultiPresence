using System.Text;

namespace MultiPresence.Models.TK8
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<uint, string> names = new();
        private static readonly Dictionary<(ulong, string), int> fields = new();
        private static readonly Dictionary<string, ulong> addresses = new();
        private static ulong currentWorld;
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            currentWorld = 0;
            processId = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            fields.Clear();
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            ulong world = Pointer(Address("World"));
            if (world != currentWorld)
            {
                fields.Clear();
                currentWorld = world;
            }
            if (world == 0 || ClassName(world) != "World") return new GameSnapshot { State = "Loading" };

            ulong instance = ObjectProperty(world, "OwningGameInstance");
            if (instance == 0) return new GameSnapshot { State = "Loading" };
            var snapshot = ReadMode(new GameSnapshot(), world, instance);
            if (snapshot.State == "Ingame")
            {
                snapshot = ReadFighters(snapshot, world, instance);
                snapshot = ReadRank(snapshot, world, instance);
                snapshot = ReadStage(snapshot);
                if (!snapshot.IsMatchmaking) snapshot = ReadScore(snapshot, world, instance);
            }
            if (Pointer(Address("World")) != world || ObjectProperty(world, "OwningGameInstance") != instance)
                return new GameSnapshot { State = "Loading" };
            return snapshot;
        }

        private static ulong Address(string name) => addresses[name];
        private static ulong Pointer(ulong address) => Hypervisor.Read<ulong>(address, true);
        private static string Name(ulong obj) => obj == 0 ? "" : FName(obj + 0x18);
        private static string ClassName(ulong obj) => obj == 0 ? "" : Name(Pointer(obj + 0x10));
        private static ulong ObjectProperty(ulong obj, string name)
        {
            int offset = Field(obj, name);
            return offset == 0 ? 0 : Pointer(obj + (ulong)offset);
        }

        private static bool IsA(ulong obj, string expected)
        {
            ulong type = obj == 0 ? 0 : Pointer(obj + 0x10);
            for (int depth = 0; type != 0 && depth < 48; depth++, type = Pointer(type + 0x40))
            {
                if (Name(type) == expected) return true;
            }
            return false;
        }

        private static int Field(ulong obj, string name)
        {
            if (obj == 0) return 0;
            var type = Pointer(obj + 0x10);
            if (fields.TryGetValue((type, name), out int cached)) return cached;
            var cls = type;
            for (int depth = 0; cls != 0 && depth < 48; depth++, cls = Pointer(cls + 0x40))
            {
                var field = Pointer(cls + 0x50);
                for (int i = 0; field != 0 && i < 2048; i++, field = Pointer(field + 0x20))
                {
                    if (FName(field + 0x28) != name) continue;
                    int offset = Hypervisor.Read<int>(field + 0x4C, true);
                    int size = Hypervisor.Read<int>(cls + 0x58, true);
                    if (offset <= 0 || offset >= size) throw Invalid("property offset for " + name);
                    return fields[(type, name)] = offset;
                }
            }
            return fields[(type, name)] = 0;
        }

        private static string FName(ulong address)
        {
            ulong value = Pointer(address);
            uint id = (uint)value;
            if (!names.TryGetValue(id, out var name))
            {
                ulong namesPool = Address("Names");
                uint block = id >> 16;
                uint last = Hypervisor.Read<uint>(namesPool + 8, true);
                if (block > last || last >= 8192) throw Invalid("name block");
                uint offset = (id & 0xffff) * 2;
                ulong chunk = Pointer(namesPool + 0x10 + block * 8);
                if (chunk == 0) throw Invalid("name pool");
                ushort header = Hypervisor.Read<ushort>(chunk + offset, true);
                bool wide = (header & 1) != 0;
                int length = (header >> 6) * (wide ? 2 : 1);
                uint limit = block == last ? Hypervisor.Read<uint>(namesPool + 12, true) : 131072;
                if (length == 0 || offset + 2 + length > limit || limit > 131072) throw Invalid("name length");
                var bytes = Hypervisor.ReadArray(chunk + offset + 2, length, true);
                names[id] = name = (wide ? Encoding.Unicode : Encoding.UTF8).GetString(bytes);
            }
            uint number = (uint)(value >> 32);
            return number == 0 ? name : $"{name}_{number - 1}";
        }

        private static InvalidDataException Invalid(string field) => new("TEKKEN 8: invalid " + field + ".");
    }
}
