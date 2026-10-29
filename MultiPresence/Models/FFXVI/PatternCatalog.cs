namespace MultiPresence.Models.FFXVI
{
    internal static class PatternCatalog
    {
        public const string Profile = "48 8B 3D ?? ?? ?? ?? 49 8B 48 58 48 85 C9 74 ?? 48 39 19 7C ?? E8 ?? ?? ?? ??";
        public const string Currency = "48 8B 0D ?? ?? ?? ?? BA 03 00 00 00 E8 ?? ?? ?? ?? 39 43 2C 7F ?? 45 8B C5";
        public const string Player = "48 8B 0D ?? ?? ?? ?? 48 83 B9 00 8E 00 00 00 75 ?? 48 81 C1 E0 8D 00 00";
        public const string Scenario = "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? F6 D8 B8 20 00 00 00 48 1B C9 81 E1 08 A9 00 00 48 03 C1";

        public static readonly string[] Layouts =
        {
            "C5 FB 10 88 18 8B 00 00 C5 FC 10 80 F8 8A 00 00 C5 FB 11 4C 24 40 80 7C 24 40 00 C5 FC 11 44 24 20",
            "49 8D 80 50 CB 00 00 48 0F 45 D0 48 63 2A BA C4 01 00 00",
            "49 8D 0C 02 48 8B 01 4C 8D 41 08 48 C1 E0 04 48 8D 51 08 4C 03 C0 EB ?? 39 3A 74 ?? 48 83 C2 10",
            "4C 8B 49 38 45 33 C0 4D 85 C9 74 ?? 4D 39 01 7C ?? 49 8B C9 E8 ?? ?? ?? ?? 48 85 C0 74 ?? 4D 39 01 7C ?? 49 8B C9 E8 ?? ?? ?? ?? EB ?? 48 8D 05 ?? ?? ?? ?? 44 8A 40 35"
        };
    }
}
