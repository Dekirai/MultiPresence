namespace MultiPresence.Models.RE4
{
    internal static partial class MemoryReader
    {
        private static readonly (string Name, string Pattern, int Operand)[] ContextPatterns =
        [
            ("Global", "A1 ?? ?? ?? ?? B9 FF FF FF 7F 21 48 ?? A1", 1),
            ("Player", "A1 ?? ?? ?? ?? D8 CC D8 C9 D8 CA D9 5D ?? D9 45 ??", 1),
            ("Score", "01 15 ?? ?? ?? ?? FF 05 ?? ?? ?? ?? FF 05 ?? ?? ?? ?? 5E B0 01 5D C3", 2),
            ("RoomLayout", "0F B7 81 AC 4F 00 00 BA 00 04 00 00 66 3B C2 75 ?? 33 C0 5D C3 B9 03 04 00 00 66 3B C1", -1),
            ("ChapterLayout", "0F B6 88 9A 4F 00 00 83 C4 04 51 6A 0A E8 ?? ?? ?? ?? 50 E8 ?? ?? ?? ?? 83 C4 0C C3", -1),
            ("CharacterLayout", "0F B6 81 C8 4F 00 00 83 C4 20 68 ?? ?? ?? ?? 83 EC 0C 83 E8 02 D9 5C 24 08 D9 EE D9 54 24 04", -1),
            ("DifficultyLayout", "0F B6 80 7C 84 00 00 48 74 ?? 83 E8 02 C7 45 F4 C8 00 00 00 74 ?? BE 46 00 00 00 C7 45 F8 19 00 00 00", -1),
        ];
    }
}
