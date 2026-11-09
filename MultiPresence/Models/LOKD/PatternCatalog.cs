namespace MultiPresence.Models.LOKD
{
    internal static partial class MemoryReader
    {
        private sealed record Pattern(string Name, string Bytes, int Operand, int End, int Addend = 0);

        private static readonly Pattern[] Patterns =
        {
            new("GameState", "8B 05 ?? ?? ?? ?? 89 05 ?? ?? ?? ?? FF C8 83 F8 0A 0F 87 ?? ?? ?? ?? 48 8D 15", 2, 6),
            new("PlayerData", "4C 8B 05 ?? ?? ?? ?? 0F B6 DA 41 8B 90 00 0B 00 00 03 D1 41 89 90 00 0B 00 00", 3, 7),
            new("Actor", "48 8B 05 ?? ?? ?? ?? 48 8B 80 10 03 00 00 8B 80 18 01 00 00", 3, 7),
            new("ChapterIndex", "3B 1D ?? ?? ?? ?? 74 ?? 8D 43 FF 3C 0B 77 ??", 2, 6),
            new("CurrentLevel", "48 89 0D ?? ?? ?? ?? 41 8B C8 89 05 ?? ?? ?? ?? 48 83 C4 28 E9", 3, 7),
            new("StoryData", "48 8B 05 ?? ?? ?? ?? 89 88 44 07 00 00 C3 CC CC 48 8B 05 ?? ?? ?? ?? 89 88 48 07 00 00 C3", 3, 7)
        };
    }
}
