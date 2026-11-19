namespace MultiPresence.Models.FFRS
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadProgress(GameSnapshot snapshot, ulong save, out List<int> party)
        {
            party = new List<int>();
            ulong type = Pointer(save + 0x10);
            var (player, playerProperty) = Member(type, "Player");
            ulong playerType = Struct(playerProperty);
            int money = Member(playerType, "currentMoney").Offset, difficulty = Member(playerType, "GameDifficulty").Offset;
            if (player >= 0x28 && money >= 0)
            {
                int gil = Hypervisor.Read<int>(save + (ulong)(player + money), true);
                if (gil >= 0) snapshot = snapshot with { Gil = gil };
            }
            if (player >= 0x28 && difficulty >= 0)
            {
                snapshot = snapshot with { Difficulty = GameText.Difficulty(Hypervisor.Read<byte>(save + (ulong)(player + difficulty), true)) };
            }

            var (partyField, partyProperty) = Member(type, "Party");
            int list = Member(Struct(partyProperty), "partyList").Offset;
            if (partyField >= 0x28 && list >= 0)
            {
                ulong array = save + (ulong)(partyField + list);
                ulong data = Pointer(array);
                int count = Hypervisor.Read<int>(array + 8, true);
                if (data != 0 && count is >= 1 and <= 8)
                {
                    party.AddRange(Hypervisor.Read<int>(data, count, true).Where(id => id > 0));
                }
            }

            var (mapField, mapProperty) = Member(type, "Map");
            int id = Member(Struct(mapProperty), "mapId").Offset;
            if (mapField < 0x28 || id < 0) return snapshot;
            return snapshot with { Location = Locations.Get(Hypervisor.Read<int>(save + (ulong)(mapField + id), true)) };
        }

        private static GameSnapshot ReadQuest(GameSnapshot snapshot, ulong instance)
        {
            ulong quests = Subsystem(instance, "CPP_QuestSubsystem");
            int mainField = Field(quests, "MainQuestId");
            if (mainField == 0) return snapshot;
            int main = Hypervisor.Read<int>(quests + (ulong)mainField, true);
            int chapter = main - 1000;
            snapshot = snapshot with { Chapter = GameText.Chapter(chapter), ChapterTitle = GameText.ChapterTitle(chapter) };

            var (mapField, mapProperty) = Member(Pointer(quests + 0x10), "QuestDataMap");
            ulong questType = Struct(Inner(mapProperty));
            int id = Member(questType, "ID").Offset, subtitle = Member(questType, "Subtitle").Offset, size = StructSize(questType);
            if (mapField < 0x28 || id < 0 || subtitle < 0 || size is < 0x20 or > 0x1000) return snapshot;

            // TMap<int32, FQuestData>: key at +0, value aligned to +8, then the hash link and bucket indices.
            int stride = (8 + size + 7) / 8 * 8 + 8;
            ulong set = quests + (ulong)mapField;
            foreach (int entry in Entries(set, stride, out var elements))
            {
                if (BitConverter.ToInt32(elements, entry) != main || BitConverter.ToInt32(elements, entry + 8 + id) != main) continue;
                var (table, key) = TextKey(Pointer(set) + (ulong)(entry + 8 + subtitle));
                return snapshot with { Objective = table == "ST_MainProgress" ? Objectives.Get(key) : "" };
            }
            return snapshot;
        }
    }
}
