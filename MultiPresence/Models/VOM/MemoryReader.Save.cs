namespace MultiPresence.Models.VOM
{
    internal static partial class MemoryReader
    {
        private const string GameDataThunkPattern = "40 53 48 83 EC 20 33 C0 49 8B D8 48 39 42 20 0F 95 C0 48 01 42 20 E8 ?? ?? ?? ?? 48 89 03";
        private const string GameDataGetterPattern = "48 89 5C 24 08 57 48 83 EC 20 48 8B 81 ?? ?? ?? ?? 48 8B F9 48 85 C0";
        private static ulong saveInstance, saveSubsystem;
        private static int saveOffset;

        private static void ResetSave()
        {
            saveInstance = saveSubsystem = 0;
            saveOffset = 0;
        }

        private static ulong ReadSaveGame(ulong instance)
        {
            if (instance != saveInstance || saveSubsystem == 0 || Pointer(saveSubsystem + 0x20) != instance)
            {
                saveSubsystem = FindSubsystem(instance, "ActSaveGameSubsystem");
                saveInstance = instance;
            }
            if (saveSubsystem == 0) return 0;
            if (saveOffset == 0) saveOffset = GameDataOffset(saveSubsystem);
            return Pointer(saveSubsystem + (ulong)saveOffset);
        }

        private static ulong FindSubsystem(ulong instance, string name)
        {
            // UGameInstance's subsystem collection maps each subsystem class to its instance.
            int size = StructSize(Pointer(instance + 0x10));
            if (size is < 0x100 or > 0x10000) throw Invalid("game instance size");
            var bytes = Hypervisor.ReadArray(instance, size, true);
            for (int offset = 0x28; offset + 0x30 <= size; offset += 8)
            {
                ulong entries = BitConverter.ToUInt64(bytes, offset);
                int slots = BitConverter.ToInt32(bytes, offset + 8);
                int capacity = BitConverter.ToInt32(bytes, offset + 12);
                ulong flags = BitConverter.ToUInt64(bytes, offset + 0x20);
                if (entries == 0 || slots is < 1 or > 256 || slots > capacity || BitConverter.ToInt32(bytes, offset + 0x28) != slots ||
                    (flags == 0 && slots > 128) || !Hypervisor.TryRead<ulong>(entries, out _, true) ||
                    (flags != 0 && !Hypervisor.TryRead<byte>(flags, out _, true)))
                {
                    continue;
                }

                var occupied = flags == 0 ? bytes.AsSpan(offset + 0x10, 16).ToArray() : Hypervisor.ReadArray(flags, (slots + 7) / 8, true);
                var pairs = Hypervisor.ReadArray(entries, slots * 24, true);
                for (int index = 0; index < slots; index++)
                {
                    if ((occupied[index / 8] & (1 << (index % 8))) == 0) continue;
                    ulong type = BitConverter.ToUInt64(pairs, index * 24);
                    ulong subsystem = BitConverter.ToUInt64(pairs, index * 24 + 8);
                    if (type == 0 || !Hypervisor.TryRead<ulong>(subsystem + 0x10, out var owner, true) || owner != type ||
                        !Hypervisor.TryRead<ulong>(subsystem + 0x20, out var outer, true) || outer != instance)
                    {
                        break;
                    }
                    if (Name(type) == name) return subsystem;
                }
            }
            return 0;
        }

        private static int GameDataOffset(ulong subsystem)
        {
            // GetCurrentGameData's native thunk calls the getter that returns the current ActSaveGame field.
            ulong type = Pointer(subsystem + 0x10);
            ulong function = Pointer(type + 0x48);
            for (int i = 0; function != 0 && i < 512 && Name(function) != "GetCurrentGameData"; i++) function = Pointer(function + 0x28);
            if (function == 0 || ClassName(function) != "Function") throw Invalid("GetCurrentGameData");

            ulong native = Pointer(function + 0xD8);
            var thunk = Hypervisor.ReadArray(native, 32, true);
            if (SignaturePattern.Parse(GameDataThunkPattern).IndexOf(thunk) != 0) throw Invalid("GetCurrentGameData thunk");
            ulong getter = (ulong)((long)native + 0x1B + BitConverter.ToInt32(thunk, 0x17));
            var code = Hypervisor.ReadArray(getter, 24, true);
            if (SignaturePattern.Parse(GameDataGetterPattern).IndexOf(code) != 0) throw Invalid("GetCurrentGameData getter");

            int offset = BitConverter.ToInt32(code, 13);
            if (offset < 0x30 || offset > StructSize(type) - 8) throw Invalid("current game data offset");
            return offset;
        }

        private static string LoadedMap(ulong world, ulong save)
        {
            // MapName follows the level instance only after streaming finished, so it must match a loaded level.
            int field = Field(save, "MapName"), levelsField = Field(world, "Levels");
            if (field == 0 || levelsField == 0) return "";
            string map = FString(save + (ulong)field);
            ulong levels = Pointer(world + (ulong)levelsField);
            int count = Hypervisor.Read<int>(world + (ulong)levelsField + 8, true);
            if (map.Length == 0 || levels == 0 || count is < 1 or > 1024) return "";
            foreach (ulong level in Hypervisor.Read<ulong>(levels, count, true))
            {
                if (level != 0 && Name(Pointer(level + 0x20)) == map) return map;
            }
            return "";
        }

        private static GameSnapshot ReadProgress(GameSnapshot snapshot, ulong save, string character, string map)
        {
            ulong type = Pointer(save + 0x10);
            int difficulty = Field(save, "Difficulty");
            if (difficulty != 0) snapshot = snapshot with { Difficulty = GameText.Difficulty(Hypervisor.Read<int>(save + (ulong)difficulty, true)) };

            var (location, region) = Locations.Get(map);
            snapshot = snapshot with { Location = location, Region = region };

            var (events, eventsProperty) = Member(type, "EventInformation");
            int chapter = Member(Struct(eventsProperty), "Chapter").Offset;
            if (events >= 0x28 && chapter >= 0)
            {
                int value = Hypervisor.Read<int>(save + (ulong)(events + chapter), true);
                snapshot = snapshot with { Chapter = GameText.Chapter(value), ChapterTitle = GameText.ChapterTitle(value) };
            }

            snapshot = ReadItems(snapshot, save, type);
            return ReadParty(snapshot, save, type, character);
        }

        private static GameSnapshot ReadItems(GameSnapshot snapshot, ulong save, ulong type)
        {
            var (field, property) = Member(type, "ItemInventoryData");
            ulong item = Struct(Struct(property));
            int id = Member(item, "ItemId").Offset, amount = Member(item, "ItemCount").Offset, size = StructSize(item);
            if (field < 0x28 || id < 0 || amount < 0 || size is < 8 or > 64) return snapshot;

            ulong data = Pointer(save + (ulong)field);
            int count = Hypervisor.Read<int>(save + (ulong)field + 8, true);
            if (count is < 0 or > 4096 || (count > 0 && data == 0)) return snapshot;
            var bytes = count == 0 ? Array.Empty<byte>() : Hypervisor.ReadArray(data, count * size, true);
            int lucre = 0, syrup = 0, coins = 0, clovers = 0;
            for (int offset = 0; offset < bytes.Length; offset += size)
            {
                int value = BitConverter.ToInt32(bytes, offset + amount);
                switch (NameOf(BitConverter.ToUInt64(bytes, offset + id)))
                {
                    case "Money": lucre = value; break;
                    case "IT1022": syrup = value; break;
                    case "IT1029": coins = value; break;
                    case "AP2033": clovers = value; break;
                }
            }
            return snapshot with { Lucre = lucre, GrizzlySyrup = syrup, NiccoloCoins = coins, GoldClovers = clovers };
        }

        private static GameSnapshot ReadParty(GameSnapshot snapshot, ulong save, ulong type, string character)
        {
            var (field, property) = Member(type, "PartyCharacterData");
            ulong member = Struct(Struct(property));
            int id = Member(member, "CharacterId").Offset, role = Member(member, "PartyRole").Offset, size = StructSize(member);
            if (field < 0x28 || id < 0 || role < 0 || size is < 12 or > 1024) return snapshot;

            ulong data = Pointer(save + (ulong)field);
            int count = Hypervisor.Read<int>(save + (ulong)field + 8, true);
            if (count is < 1 or > 16 || data == 0) return snapshot;
            var bytes = Hypervisor.ReadArray(data, count * size, true);
            var party = new List<string>();
            for (int offset = 0; offset < bytes.Length; offset += size)
            {
                if (bytes[offset + role] > 2) continue;
                string hero = NameOf(BitConverter.ToUInt64(bytes, offset + id));
                string name = hero.Length < 4 ? "" : GameText.Hero(hero[..4]);
                if (name.Length > 0 && name != character && !party.Contains(name)) party.Add(name);
            }
            return snapshot with { Party = string.Join(", ", party) };
        }
    }
}
