using System.Numerics;

namespace MultiPresence.Models.TK8
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadRank(GameSnapshot snapshot, ulong world, ulong instance)
        {
            if (!snapshot.HasLocalFighter || snapshot.LocalSide is not (0 or 1)) return snapshot;
            string fighter = snapshot.LocalSide == 0 ? snapshot.LeftFighter : snapshot.RightFighter;
            if (fighter.Length == 0) return snapshot;

            ulong selection = Pointer(Address("PlayerSelection"));
            if (selection == 0) return snapshot;
            ulong begin = Pointer(selection), end = Pointer(selection + 8);
            if (begin == 0 || end < begin || end - begin > 128 || (end - begin) % 8 != 0) return snapshot;

            for (ulong slot = begin; slot < end; slot += 8)
            {
                ulong profile = Pointer(slot);
                if (profile == 0 || Hypervisor.Read<int>(profile + 8, true) != 1) continue;
                if (!Hypervisor.TryRead<int>(profile + 0x10, out int id, true) || id is < 0 or > 254 || FighterName(id) != fighter)
                    return snapshot;
                if (snapshot.LeftFighter == snapshot.RightFighter && Hypervisor.Read<byte>(profile + 0x18, true) != snapshot.LocalSide)
                    return snapshot;

                // Character select stores the online grade in each fighter's own record.
                ulong data = Pointer(profile + 0x20);
                if (data == 0) return snapshot;
                ulong record = data + 0x164 + (ulong)id * 0x2B0;
                if (!Hypervisor.TryRead<int>(record, out int recordId, true) || recordId != id ||
                    !Hypervisor.TryRead<uint>(record + 0x44, out uint grade, true) || DecodeRank(grade) is not int rank ||
                    Pointer(profile + 0x20) != data || Hypervisor.Read<int>(profile + 0x10, true) != id ||
                    Pointer(slot) != profile || Pointer(selection) != begin || Pointer(selection + 8) != end ||
                    Pointer(Address("PlayerSelection")) != selection) return snapshot;
                return snapshot with { RankId = rank, Rank = RankNames[rank] };
            }
            return snapshot;
        }

        private static int? DecodeRank(uint grade)
        {
            const ulong key = 0xEDCCFB96DCA40FBA;
            uint checksum = 0;
            for (int shift = 0; shift < 24; shift += 8)
                checksum ^= (byte)(BitOperations.RotateLeft(key, shift + 8) ^ (grade >> shift));
            if ((checksum == 0 ? 1 : checksum) != grade >> 24) return null;

            uint value = grade ^ 0x1D;
            uint mask = (uint)BitOperations.RotateLeft(key, (int)(value & 31)) & 0xFFFFFFE0;
            int rank = (int)((value ^ mask) & 0xFFFFFF);
            return rank < RankNames.Length ? rank : null;
        }

        private static readonly string[] RankNames =
        [
            "Beginner", "1st dan", "2nd dan", "Fighter", "Strategist", "Combatant", "Brawler", "Ranger",
            "Cavalry", "Warrior", "Assailant", "Dominator", "Vanquisher", "Destroyer", "Eliminator",
            "Garyu", "Shinryu", "Tenryu", "Mighty Ruler", "Flame Ruler", "Battle Ruler", "Fujin",
            "Raijin", "Kishin", "Bushin", "Tekken King", "Tekken Emperor", "Tekken God", "Tekken God Supreme",
            "God of Destruction", "God of Destruction I", "God of Destruction II", "God of Destruction III",
            "God of Destruction IV", "God of Destruction V", "God of Destruction VI", "God of Destruction VII",
            "God of Destruction ∞"
        ];
    }
}
