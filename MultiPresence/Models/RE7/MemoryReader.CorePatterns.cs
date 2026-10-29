namespace MultiPresence.Models.RE7
{
    internal static partial class MemoryReader
    {
        private static readonly (string Name, string Pattern, int Operand, int Length)[] CorePatterns =
        [
            ("ObjectManager", "48 8B 15 ?? ?? ?? ?? 48 83 78 18 00 75 ?? 48 85 D2 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 48 8B 82 88 00 00 00 48 83 C4 28 C3 CC CC CC CC CC CC 48 8B 82 98 04 00 00", 3, 7),
            ("GameManager", "48 8B 3D ?? ?? ?? ?? 48 83 78 18 00 0F 85 ?? ?? ?? ?? 48 8B 92 E0 00 00 00", 3, 7),
            ("CoreDifficultyLayout", "8B 82 E8 00 00 00 C3 CC CC CC CC CC CC CC CC CC 4D 85 C0", 0, 0),
            ("CoreSceneLoadingLayout", "80 BA A1 01 00 00 00 0F 95 C0 C3", 0, 0),
            ("CoreTitleLayout", "41 83 F8 15 7E ?? 41 83 F8 1C 74 ?? 41 83 F8 21 75 ?? B0 01 C3 41 83 F8 03 74 ?? 41 83 F8 15 74 ?? 32 C0 C3", 0, 0),
            ("CoreHealthLayout", "48 83 EC 28 4C 8B 42 70 4D 85 C0 75 ?? BA 38 00 00 00 E8 ?? ?? ?? ?? 0F 57 C0 48 83 C4 28 C3 48 8B 41 50 48 83 78 18 00 75 ?? F3 41 0F 10 40 14 48 83 C4 28 C3 CC CC CC CC CC CC CC CC CC CC CC", 0, 0),
            ("CoreEquipmentLayout", "48 83 EC 28 4C 8B 82 28 01 00 00 4C 8B C9 4D 85 C0 74 ?? 49 83 78 10 00 74 ?? 32 C0 EB ?? B0 01 0F B6 D0 48 8B 41 50 48 8B 48 18 48 85 C9 74 ?? 32 C0 EB ?? 85 D2 0F 94 C0 48 85 C9 75 ?? 84 C0 74 ?? 4D 85 C0 75 ?? 8D 51 38 49 8B C9 E8 ?? ?? ?? ?? 33 C0 48 83 C4 28 C3 49 8B 40 60 48 83 C4 28 C3", 0, 0),
            ("CoreAmmoLayout", "4C 8B 82 90 01 00 00 4D 85 C0 74 ?? 48 8B 41 50 48 83 78 18 00 75 ?? 41 8B 40 14 C3 33 C0 C3", 0, 0),
            ("CoreReserveLayout", "48 8B CB 48 85 ED 0F 84 ?? ?? ?? ?? 41 B0 01 48 8B D5 E8 ?? ?? ?? ?? 48 8B D0 48 8B 43 50 48 83 78 18 00 75 ?? 4D 8B C6 48 8B CB E8 ?? ?? ?? ?? 0F B6 C8 48 8B 43 50 48 83 78 18 00 75 ?? 85 C9 75 ?? 03 B5 88 00 00 00", 0, 0),
            ("CorePassiveInfinityLayout", "48 83 EC 28 4C 8B 82 10 01 00 00 4C 8B C9 4D 85 C0 74 ?? 49 83 78 10 00 74 ?? 32 C0 EB ?? B0 01 0F B6 D0 48 8B 41 50 48 8B 48 18 48 85 C9 74 ?? 32 C0 EB ?? 85 D2 0F 94 C0 48 85 C9 75 ?? 84 C0 74 ?? 4D 85 C0 75 ?? 8D 51 38 49 8B C9 E8 ?? ?? ?? ?? 32 C0 48 83 C4 28 C3 41 83 B8 A0 00 00 00 00 0F 9F C0 48 83 C4 28 C3", 0, 0),
        ];
    }
}
