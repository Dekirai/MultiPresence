namespace MultiPresence.Models.FFRS
{
    internal static class GameText
    {
        private static readonly Dictionary<int, string> Units = new()
        {
            { 1001, "Rain" },
            { 1002, "Lasswell" },
            { 1003, "Fina" },
            { 1004, "Lid" },
            { 1005, "Nichol" },
            { 1006, "Archwitch Fina" },
            { 1007, "Jake" },
            { 1008, "Sakura" },
            { 1013, "Ignis" },
            { 1014, "Alluvion" },
            { 1016, "Charlotte" },
        };

        private static readonly Dictionary<int, string> ChapterTitles = new()
        {
            { 1, "Tailing the Truth" },
            { 2, "Eternal Blue" },
            { 3, "Wardens of the Aquapolis" },
            { 4, "Banner Oath" },
            { 5, "Scars of Time" },
            { 6, "Tranquility" },
            { 7, "Reckoning" },
            { 8, "Beyond Hope" },
            { 9, "Epilogue" },
            { 10, "Prologue" },
        };

        private static readonly string[] Difficulties =
        {
            "Casual",
            "Normal",
            "Expert"
        };

        public static string Unit(int id) => Units.GetValueOrDefault(id, "");

        public static string Chapter(int chapter) => chapter is >= 1 and <= 8 ? $"Chapter {chapter}" : "";

        public static string ChapterTitle(int chapter) => ChapterTitles.GetValueOrDefault(chapter, "");

        public static string Difficulty(int value) => value >= 0 && value < Difficulties.Length ? Difficulties[value] : "";
    }
}
