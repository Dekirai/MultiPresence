namespace MultiPresence.Models.TK8
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadStage(GameSnapshot snapshot)
        {
            ulong manager = BattleManager("StageKey");
            if (manager == 0 || !Hypervisor.TryRead<int>(manager + 0x84, out int id, true) || id < 0 || id >= 255)
                return snapshot;
            return snapshot with { StageId = id, Stage = StageName(id) };
        }

        private static string StageName(int id) => id switch
        {
            0 or 3 => "Arena",
            1 or 2 => "Arena (Underground)",
            4 => "Urban Square (Evening)",
            5 or 6 or 16 or 17 or 18 or 19 or 20 => "Urban Square",
            7 or 8 => "Yakushima",
            9 or 10 => "Coliseum of Fate",
            11 or 12 or 36 => "Rebel Hangar",
            13 or 14 or 40 or 41 or 42 or 43 or 44 => "Fallen Destiny",
            21 or 22 => "Descent into Subconscious",
            23 or 24 => "Sanctum",
            25 or 26 => "Into the Stratosphere",
            27 => "Ortiz Farm",
            28 => "Celebration on the Seine",
            29 or 37 or 38 => "Secluded Training Ground",
            30 => "Elegant Palace",
            31 or 39 => "Midnight Siege",
            32 => "Beachside",
            33 => "Seaside Resort",
            34 => "Genmaji Temple",
            35 => "Genmaji Temple (Daytime)",
            45 => "Phoenix Gate",
            46 => "PAC-PIXELS",
            47 => "Baobab Horizon",
            _ => ""
        };
    }
}
