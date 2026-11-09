using System.Text;

namespace MultiPresence.Models.SH2
{
    internal static partial class MemoryReader
    {
        private const string WorldPattern = "48 8B 15 ?? ?? ?? ?? 48 39 82 20 02 00 00 75 ?? 33 D2 48 8B CF E8 ?? ?? ?? ?? 44 8B 45 07";
        private const string NamesPattern = "48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? 01 8B 50 08 48 8D 98 40 00 01 00";
        private static readonly Dictionary<uint, string> names = new();
        private static readonly Dictionary<(ulong, string), int> fields = new();
        private static ulong worldSlot, namesPool, currentWorld;
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            worldSlot = namesPool = currentWorld = 0;
            processId = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            fields.Clear();
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            ulong world = Pointer(worldSlot);
            if (world != currentWorld)
            {
                fields.Clear();
                currentWorld = world;
            }
            if (world == 0 || ClassName(world) != "World") return new GameSnapshot { State = "Loading" };

            ulong instance = ObjectProperty(world, "OwningGameInstance");
            int menuField = Field(instance, "MainMenuLevel");
            if (menuField != 0 && Name(world).Equals(FName(instance + (ulong)menuField), StringComparison.OrdinalIgnoreCase))
            {
                return new GameSnapshot();
            }
            var playersField = Field(instance, "LocalPlayers");
            if (playersField == 0) return new GameSnapshot { State = "Loading" };
            ulong players = instance + (ulong)playersField;
            int count = Hypervisor.Read<int>(players + 8, true);
            ulong data = Pointer(players);
            if (count is < 1 or > 4 || data == 0) return new GameSnapshot { State = "Loading" };

            ulong local = Pointer(data);
            ulong controller = ObjectProperty(local, "PlayerController");
            ulong pawn = ObjectProperty(controller, "Pawn");
            if (!IsA(pawn, "SHCharacterPlay")) return new GameSnapshot { State = "Loading" };

            var snapshot = ReadPlayer(new GameSnapshot { State = "Ingame" }, pawn);
            snapshot = ReadLocation(snapshot, pawn);
            snapshot = ReadDifficulties(snapshot, instance);
            if (Pointer(worldSlot) != world || ObjectProperty(world, "OwningGameInstance") != instance ||
                Pointer(players) != data || Pointer(data) != local || ObjectProperty(local, "PlayerController") != controller ||
                ObjectProperty(controller, "Pawn") != pawn)
            {
                return new GameSnapshot { State = "Loading" };
            }
            return snapshot;
        }

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

        private static InvalidDataException Invalid(string field) => new("SILENT HILL 2: invalid " + field + ".");
    }
}
