using System.Text;

namespace MultiPresence.Models.VOM
{
    internal static partial class MemoryReader
    {
        private const string WorldPattern = "48 8B 05 ?? ?? ?? ?? 49 8B D2 4D 8B C5 48 8B 0A 48 39 81 80 02 00 00 74 ?? 44 03 CE 4C 03 C6 48 83 C2 08";
        private const string NamesPattern = "48 8D 15 ?? ?? ?? ?? 48 8B 44 24 30 4C 8B CB 48 C1 E8 20 44 8D 04 00 4C 03 44 FA 10 41 0F B7 10";
        private static readonly Dictionary<uint, string> names = new();
        private static readonly Dictionary<(ulong, string), (int Offset, ulong Property)> members = new();
        private static ulong worldSlot, namesPool, currentWorld;
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            worldSlot = namesPool = currentWorld = 0;
            processId = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            members.Clear();
            ResetSave();
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            ulong world = Pointer(worldSlot);
            if (world != currentWorld)
            {
                members.Clear();
                ResetSave();
                currentWorld = world;
            }
            if (world == 0 || ClassName(world) != "World") return new GameSnapshot { State = "Loading" };

            ulong instance = ObjectProperty(world, "OwningGameInstance");
            ulong mode = ObjectProperty(world, "AuthorityGameMode");
            if (Name(world) == "MAP_GameStart") return new GameSnapshot();
            if (!IsA(instance, "ActGameInstance") || !IsA(mode, "PyX21GameMode")) return new GameSnapshot { State = "Loading" };

            var playersField = Field(instance, "LocalPlayers");
            if (playersField == 0) return new GameSnapshot { State = "Loading" };
            ulong players = instance + (ulong)playersField;
            int count = Hypervisor.Read<int>(players + 8, true);
            ulong data = Pointer(players);
            if (count is < 1 or > 4 || data == 0) return new GameSnapshot { State = "Loading" };

            ulong local = Pointer(data);
            ulong controller = ObjectProperty(local, "PlayerController");
            ulong pawn = ObjectProperty(controller, "Pawn");
            ulong save = ReadSaveGame(instance);
            if (!IsA(pawn, "PyCharBase") || !IsA(save, "ActSaveGame")) return new GameSnapshot { State = "Loading" };
            string map = LoadedMap(world, save);
            if (map.Length == 0) return new GameSnapshot { State = "Loading" };

            var snapshot = ReadPlayer(new GameSnapshot { State = "Ingame" }, pawn);
            snapshot = ReadProgress(snapshot, save, snapshot.Character, map);
            snapshot = ReadObjective(snapshot, mode, save);
            if (Pointer(worldSlot) != world || ObjectProperty(world, "OwningGameInstance") != instance ||
                ObjectProperty(world, "AuthorityGameMode") != mode || Pointer(players) != data || Pointer(data) != local ||
                ObjectProperty(local, "PlayerController") != controller || ObjectProperty(controller, "Pawn") != pawn ||
                ReadSaveGame(instance) != save)
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
            int offset = Member(Pointer(obj + 0x10), name).Offset;
            return offset >= 0x28 ? offset : 0;
        }

        private static (int Offset, ulong Property) Member(ulong type, string name)
        {
            if (type == 0) return (-1, 0);
            if (members.TryGetValue((type, name), out var cached)) return cached;
            for (ulong cls = type; cls != 0; cls = Pointer(cls + 0x40))
            {
                var field = Pointer(cls + 0x50);
                for (int i = 0; field != 0 && i < 2048; i++, field = Pointer(field + 0x20))
                {
                    if (FName(field + 0x28) != name) continue;
                    int offset = Hypervisor.Read<int>(field + 0x4C, true);
                    int size = Hypervisor.Read<int>(cls + 0x58, true);
                    if (offset < 0 || offset >= size) throw Invalid("property offset for " + name);
                    return members[(type, name)] = (offset, field);
                }
            }
            return members[(type, name)] = (-1, 0);
        }

        private static ulong Struct(ulong property) => property == 0 ? 0 : Pointer(property + 0x78);
        private static int StructSize(ulong type) => type == 0 ? 0 : Hypervisor.Read<int>(type + 0x58, true);

        private static string FName(ulong address) => NameOf(Pointer(address));

        private static string NameOf(ulong value)
        {
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

        private static string FString(ulong address)
        {
            ulong data = Pointer(address);
            int length = Hypervisor.Read<int>(address + 8, true);
            if (data == 0 || length is <= 1 or > 256) return "";
            return Encoding.Unicode.GetString(Hypervisor.ReadArray(data, (length - 1) * 2, true));
        }

        private static InvalidDataException Invalid(string field) => new("Visions of Mana: invalid " + field + ".");
    }
}
