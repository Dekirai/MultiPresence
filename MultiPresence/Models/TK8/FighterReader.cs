namespace MultiPresence.Models.TK8
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadFighters(GameSnapshot snapshot, ulong world, ulong instance)
        {
            ulong manager = BattleManager("FighterKey");
            if (manager == 0) return snapshot;
            ulong list = Pointer(manager);
            if (list == 0) return snapshot;
            ulong begin = Pointer(list + 8);
            ulong end = Pointer(list + 16);
            if (begin == 0 || end < begin || end - begin is < 16 or > 4096 || (end - begin) % 8 != 0) return snapshot;

            ulong first = Pointer(begin), second = Pointer(begin + 8);
            if (first == 0 || second == 0 || first == second) return snapshot;
            // GetFighterIDByPlayerID reads the fighter state at +0x90, then its roster ID at +0x16C.
            int firstId = Hypervisor.Read<int>(first + 0x1FC, true);
            int secondId = Hypervisor.Read<int>(second + 0x1FC, true);
            int? local = snapshot.HasLocalFighter ? LocalFighterSide(firstId, secondId) : null;
            if (Pointer(begin) != first || Pointer(begin + 8) != second || Pointer(list + 8) != begin || Pointer(manager) != list)
            {
                return snapshot;
            }
            return snapshot with { LeftFighter = FighterName(firstId), RightFighter = FighterName(secondId), LocalSide = local };
        }

        private static int? LocalFighterSide(int firstId, int secondId)
        {
            ulong selection = Pointer(Address("PlayerSelection"));
            if (selection == 0) return null;
            ulong begin = Pointer(selection), end = Pointer(selection + 8);
            if (begin == 0 || end < begin || end - begin > 128 || (end - begin) % 8 != 0) return null;

            // The side selector looks up local user 1. Check its fighter against the live battle slots.
            int? ownSide = null, otherSide = null;
            for (ulong slot = begin; slot < end; slot += 8)
            {
                ulong profile = Pointer(slot);
                if (profile == 0) continue;
                int user = Hypervisor.Read<int>(profile + 8, true);
                int selected = Hypervisor.Read<int>(profile + 0x10, true);
                byte side = Hypervisor.Read<byte>(profile + 0x18, true);
                int? matchedSide = null;
                if (firstId != secondId)
                {
                    if (selected == firstId) matchedSide = 0;
                    else if (selected == secondId) matchedSide = 1;
                }
                else if (selected == firstId && side < 2) matchedSide = side;
                if (user == 1) ownSide = matchedSide;
                else if (user is 0 or 2 or 3 && matchedSide.HasValue) otherSide = matchedSide;
            }
            if (Pointer(Address("PlayerSelection")) != selection || Pointer(selection) != begin || Pointer(selection + 8) != end) return null;
            if (firstId == secondId && (!ownSide.HasValue || otherSide != 1 - ownSide.Value)) return null;
            return ownSide;
        }

        private static string FighterName(int id) => id switch
        {
            0 => "Paul", 1 => "Law", 2 => "King", 3 => "Yoshimitsu", 4 => "Hwoarang",
            5 => "Xiaoyu", 6 => "Jin", 7 => "Bryan", 8 => "Kazuya", 9 => "Steve",
            10 => "Jack-8", 11 => "Asuka", 12 => "Devil Jin", 13 => "Feng", 14 => "Lili",
            15 => "Dragunov", 16 => "Leo", 17 => "Lars", 18 => "Alisa", 19 => "Claudio",
            20 => "Shaheen", 21 => "Nina", 22 => "Lee", 23 => "Kuma", 24 => "Panda",
            25 => "Zafina", 26 => "Leroy", 27 => "Jun", 28 => "Reina", 29 => "Azucena",
            30 => "Victor", 31 => "Raven", 32 => "Azazel", 33 => "Eddy", 34 => "Lidia",
            35 => "Heihachi", 36 => "Clive", 37 => "Anna", 38 => "Fahkumram", 39 => "Armor King",
            40 => "Miary Zo", 41 => "Kunimitsu", 42 => "Bob",
            _ => ""
        };
    }
}
