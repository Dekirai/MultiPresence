namespace MultiPresence.Models.BXP
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadCharacterProgress(GameSnapshot snapshot, ulong meta, ulong battle)
        {
            if (snapshot.State != "Ingame" || meta == 0 || battle == 0) return snapshot;
            ulong active = Ptr(battle + 0x10);
            if (active == 0 || ClassName(active) != "CharBattleInst") return snapshot;

            int id = Value<int>(active + 0x10);
            ulong record = CharacterArrayItem(Ptr(meta + 0xA8), id, "CharMetaInst");
            if (record == 0 || Value<int>(record + 0x10) != id) return snapshot;
            int level = Value<int>(record + 0x28);
            if (level is < 0 or > 99 || Ptr(battle + 0x10) != active) return snapshot;
            snapshot = snapshot with { CharacterLevel = level + 1 };
            if (level == 99) return snapshot;

            int experience = Value<int>(record + 0x2C);
            if (experience < 0) return snapshot;
            snapshot = snapshot with { Experience = experience };
            ulong database = Singleton("InfoDB");
            ulong info = database == 0 ? 0 : CharacterArrayItem(Ptr(database + 0xD8), id, "CharInfo");
            if (info == 0 || Value<int>(info + 0x58) != id) return snapshot;

            // CharMetaInst.GetTgtXP uses the zero-based level and the character's DLC version.
            int target = (level + 8) * 50 + (int)MathF.Round((level - 0.85f) * level * 50);
            if (Value<int>(info + 0x64) > 0) target = target * 3 / 2;
            return snapshot with { MaxExperience = target };
        }

        private static ulong CharacterArrayItem(ulong array, int id, string type)
        {
            if (array == 0 || id < 0 || (ulong)id >= Value<ulong>(array + 0x18)) return 0;
            ulong element = Ptr(array + 0x20 + (ulong)id * 8);
            return element != 0 && ClassName(element) == type ? element : 0;
        }
    }
}
