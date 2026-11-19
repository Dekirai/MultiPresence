using System.Text;

namespace MultiPresence.Models.FFRS
{
    internal static partial class MemoryReader
    {
        private const string WorldPattern = "48 8B 05 ?? ?? ?? ?? 4C 8B C3 0F 1F 40 00 49 8B 08 48 39 81 C0 02 00 00 74 ?? FF C2 49 83 C0 08";
        private const string NamesPattern = "48 8D 05 ?? ?? ?? ?? EB 13 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? C6 05 ?? ?? ?? ?? 01 0F 10 07 4C 8D 44 24 20";
        private const string TextKeysPattern = "48 8D 0D ?? ?? ?? ?? 48 8B 5C 24 30 48 83 C4 20 5F E9 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 3D ?? ?? ?? ?? FF 75 ?? 48 8D 0D ?? ?? ?? ?? 48 89 3D";
        private static readonly Dictionary<uint, string> names = new();
        private static readonly Dictionary<int, string> textKeys = new();
        private static readonly Dictionary<(ulong, string), (int Offset, ulong Property)> members = new();
        private static ulong worldSlot, namesPool, textKeyState, currentWorld;
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            worldSlot = namesPool = textKeyState = currentWorld = 0;
            processId = 0;
            nextScan = DateTime.MinValue;
            names.Clear();
            textKeys.Clear();
            members.Clear();
            ResetSubsystems();
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            ulong world = Pointer(worldSlot);
            if (world != currentWorld)
            {
                members.Clear();
                ResetSubsystems();
                currentWorld = world;
            }
            if (world == 0 || ClassName(world) != "World") return new GameSnapshot { State = "Loading" };

            ulong mode = ObjectProperty(world, "AuthorityGameMode");
            ulong instance = ObjectProperty(world, "OwningGameInstance");
            if (Name(world) == "MUCHA_Title" || IsA(mode, "CPP_MuchaGameModeBoot")) return new GameSnapshot();
            if (!(IsA(mode, "CPP_MuchaGameModeField") || IsA(mode, "CPP_MuchaGameModeCardgame")) || !IsA(instance, "CPP_MuchaGameInstance"))
            {
                return new GameSnapshot { State = "Loading" };
            }

            ulong manager = Subsystem(instance, "CPP_MuchaSaveDataManager");
            ulong save = ObjectProperty(manager, "m_pGameSaveData");
            if (!IsA(save, "CPP_MuchaGameSaveData")) return new GameSnapshot { State = "Loading" };

            var snapshot = ReadProgress(new GameSnapshot { State = "Ingame" }, save, out var party);
            if (party.Count == 0) return new GameSnapshot { State = "Loading" };
            snapshot = ReadUnits(snapshot, instance, party);
            snapshot = ReadQuest(snapshot, instance);
            if (Pointer(worldSlot) != world || ObjectProperty(world, "AuthorityGameMode") != mode ||
                ObjectProperty(world, "OwningGameInstance") != instance || ObjectProperty(manager, "m_pGameSaveData") != save)
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
            ulong cls = type;
            for (int depth = 0; cls != 0 && depth < 48; depth++, cls = Pointer(cls + 0x40))
            {
                var field = Pointer(cls + 0x50);
                for (int i = 0; field != 0 && i < 2048; i++, field = Pointer(field + 0x18))
                {
                    if (FName(field + 0x20) != name) continue;
                    int offset = Hypervisor.Read<int>(field + 0x44, true);
                    int size = Hypervisor.Read<int>(cls + 0x58, true);
                    if (offset < 0 || offset >= size) throw Invalid("property offset for " + name);
                    return members[(type, name)] = (offset, field);
                }
            }
            return members[(type, name)] = (-1, 0);
        }

        private static ulong Struct(ulong property) => property == 0 ? 0 : Pointer(property + 0x70);
        private static ulong Inner(ulong property) => property == 0 ? 0 : Pointer(property + 0x78);
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

        private static InvalidDataException Invalid(string field) => new("FINAL FANTASY RESONANCE: invalid " + field + ".");
    }
}
