namespace MultiPresence.Models.TK8
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadMode(GameSnapshot snapshot, ulong world, ulong instance)
        {
            ulong gameMode = ObjectProperty(world, "AuthorityGameMode");
            int typeOffset = Field(gameMode, "GameModeType");
            if (typeOffset == 0) return snapshot with { State = "Loading" };

            byte type = Hypervisor.Read<byte>(gameMode + (ulong)typeOffset, true);
            if (type != 1)
            {
                var (mode, session) = type switch
                {
                    2 => ("Character Select", ""),
                    3 => ("Character Customization", ""),
                    4 => (IsSuperGhostBattle() ? "Super Ghost Battle" : "Arcade Quest", "Offline"),
                    5 => ("TEKKEN Fight Lounge", "Online"),
                    6 => ("Avatar Customization", ""),
                    9 or 11 => ("Player Match", "Online"),
                    10 => ("Player Profile", ""),
                    _ => ("", "")
                };
                if (mode.Length != 0) return snapshot with { State = "Menu", Mode = mode, Session = session };
                return type is 7 or 8 ? snapshot : snapshot with { State = "Loading" };
            }

            if (!IsA(gameMode, "PolarisBattleGameMode")) return snapshot with { State = "Loading" };
            ulong context = CurrentBattleContext();
            ulong config = context == 0 ? 0 : Pointer(context + 8);
            ulong stage = BattleManager("StageKey");
            ulong battle = BattleManager("ScoreKey");
            if (config == 0 || stage == 0 || battle == 0) return snapshot with { State = "Loading" };

            int modeId = Hypervisor.Read<int>(config, true);
            int phase = Hypervisor.Read<int>(battle + 0x28, true);
            if (phase == 38 || modeId == 0) return snapshot with { State = "Loading" };
            ulong replay = Pointer(context + 0x28);
            if (replay != 0 && Hypervisor.Read<int>(replay + 0x28, true) == 2)
                return snapshot with { State = "Ingame", Mode = "Replay" };

            if (modeId == 1)
            {
                int rules = Hypervisor.Read<int>(Address("NetworkRules"), true);
                if (rules is 1 or 2)
                    return snapshot with { State = "Ingame", Mode = rules == 2 ? "Ranked Matchmaking" : "Quick Matchmaking", Session = "Online", IsMatchmaking = true, HasLocalFighter = true };
                return snapshot with { State = "Ingame", Mode = "Practice", Session = "Offline", HasLocalFighter = true };
            }
            if (modeId == 4)
            {
                int rules = Hypervisor.Read<int>(Address("NetworkRules"), true);
                string mode = rules switch
                {
                    0 => "Online Practice",
                    1 => "Quick Match",
                    2 => "Ranked Match",
                    3 => "Group Match",
                    6 => "Tekken Ball",
                    7 => "Ghost vs. Ghost",
                    _ => "Online Battle"
                };
                ulong watch = Pointer(Address("WatchSession"));
                if (watch != 0)
                {
                    if (mode == "Online Battle" && Hypervisor.Read<byte>(watch + 0x1a8, true) == 1) mode = "Player Match";
                    return snapshot with { State = "Ingame", Mode = $"Spectating ({mode})", Session = "Online" };
                }
                return snapshot with { State = "Ingame", Mode = mode, Session = "Online", HasLocalFighter = rules is >= 0 and <= 6 };
            }
            ulong modeRoot = Pointer(Address("ModeRoot"));
            ulong arcadeQuest = modeRoot == 0 ? 0 : Pointer(modeRoot + 0x28);
            if (arcadeQuest != 0 && Hypervisor.Read<int>(arcadeQuest + 0xf8, true) is 2 or 8)
            {
                ulong active = Pointer(modeRoot + 0x110);
                if (active != 0 && Hypervisor.Read<uint>(active + 0x60, true) == 0x2b570179)
                    return snapshot with { State = "Ingame", Mode = IsSuperGhostBattle() ? "Super Ghost Battle" : "Arcade Quest", Session = "Offline", HasLocalFighter = true };
            }
            string offline = modeId switch
            {
                5 or 18 => "Story",
                6 => "Versus",
                8 => "Character Episodes",
                12 or 19 => "Tekken Ball",
                20 or 21 => "Ghost vs. Ghost",
                _ => ""
            };
            if (offline.Length == 0)
            {
                ulong arcade = modeRoot == 0 ? 0 : Pointer(modeRoot + 0x40);
                if (arcade != 0 && Hypervisor.Read<byte>(arcade, true) == 1) offline = "Arcade Battle";
            }
            if (offline.Length == 0) return snapshot with { State = "Ingame", Mode = "Battle" };
            return snapshot with { State = "Ingame", Mode = offline, Session = "Offline", HasLocalFighter = modeId is not (19 or 20 or 21) };
        }

        private static ulong CurrentBattleContext()
        {
            ulong root = Pointer(Address("ModeRoot"));
            if (root == 0) return 0;
            ulong end = Pointer(root + 0x50);
            ulong buckets = Pointer(root + 0x60);
            ulong mask = Pointer(root + 0x78);
            uint key = Hypervisor.Read<uint>(root + 0x88, true);
            if (buckets == 0 || end == 0 || mask > 0xffff) return 0;

            ulong bucket = buckets + (key & mask) * 16;
            ulong first = Pointer(bucket);
            ulong entry = Pointer(bucket + 8);
            for (int i = 0; i < 256 && entry != end && entry != 0; i++)
            {
                if (Hypervisor.Read<uint>(entry + 0x10, true) == key) return Pointer(entry + 0x18);
                if (entry == first) break;
                entry = Pointer(entry + 8);
            }
            return 0;
        }

        private static bool IsSuperGhostBattle()
        {
            ulong root = Pointer(Address("ModeRoot"));
            ulong system = root == 0 ? 0 : Pointer(root + 0x28);
            ulong sequence = system == 0 ? 0 : Pointer(system + 0xe8);
            return sequence != 0 && Hypervisor.Read<byte>(sequence + 0x2b0, true) == 1;
        }
    }
}
