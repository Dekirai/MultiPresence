namespace MultiPresence.Models.VOM
{
    internal static class GameText
    {
        private static readonly Dictionary<string, string> Heroes = new(StringComparer.Ordinal)
        {
            { "P001", "Val" },
            { "P002", "Careena" },
            { "P003", "Morley" },
            { "P004", "Julei" },
            { "P005", "Palamena" },
            { "P006", "Aesh" },
            { "P007", "Eoren" },
            { "P011", "Lyza" },
            { "P012", "Hinna" },
            { "P013", "Ramcoh" },
            { "P021", "Vuscav" },
            { "P022", "Flammie" },
            { "P027", "Pikul" },
        };

        private static readonly Dictionary<string, string> Classes = new(StringComparer.Ordinal)
        {
            { "P001", "Guard" },
            { "P001A", "Liege" },
            { "P001B", "Duelist" },
            { "P001C", "Rune Knight" },
            { "P001D", "Aegis" },
            { "P001E", "Paladin" },
            { "P001F", "Edelfrei" },
            { "P001G", "Berserker" },
            { "P001H", "Weapon Master" },
            { "P002", "Oracle" },
            { "P002A", "Dancer" },
            { "P002B", "Divine Fist" },
            { "P002C", "Conjurer" },
            { "P002D", "Maverick" },
            { "P002E", "Warrior Monk" },
            { "P002F", "Starlancer" },
            { "P002G", "Dragon Master" },
            { "P002H", "Moon Charterer" },
            { "P003", "Ranger" },
            { "P003A", "Ascetic" },
            { "P003B", "Samurai" },
            { "P003C", "Ninja Master" },
            { "P003D", "Rogue" },
            { "P003E", "Soothsayer" },
            { "P003F", "Nomad" },
            { "P003G", "Nightblade" },
            { "P003H", "Sovereign" },
            { "P004", "Custodian" },
            { "P004A", "Sage" },
            { "P004B", "Gatekeeper" },
            { "P004C", "Occultist" },
            { "P004D", "Hunter" },
            { "P004E", "High Cleric" },
            { "P004F", "Scholar" },
            { "P004G", "Necromancer" },
            { "P004H", "Piper" },
            { "P005", "Monarch" },
            { "P005A", "Grand Diviner" },
            { "P005B", "Reaper" },
            { "P005C", "Archmage" },
            { "P005D", "Rune Seer" },
            { "P005E", "Majesty" },
            { "P005F", "Masquer" },
            { "P005G", "Magus" },
            { "P005H", "Beastleader" },
        };

        private static readonly string[] Difficulties =
        {
            "Beginner",
            "Easy",
            "Normal",
            "Hard",
            "Expert"
        };

        private static readonly string[] ChapterTitles =
        {
            "Daybreak",
            "Fate",
            "Initiation",
            "Confessions",
            "Hope",
            "Miscalculations",
            "Revelations",
            "Upheaval",
            "Reflections",
            "Visions",
            "Ambitions"
        };

        public static string Hero(string id) => Heroes.GetValueOrDefault(id, "");

        public static string Class(string id) => Classes.GetValueOrDefault(id, "");

        public static string Difficulty(int value) => value >= 0 && value < Difficulties.Length ? Difficulties[value] : "";

        public static string Chapter(int value) => value == 0 ? "Prologue" : value > 0 && value < ChapterTitles.Length ? $"Chapter {value}" : "";

        public static string ChapterTitle(int value) => value >= 0 && value < ChapterTitles.Length ? ChapterTitles[value] : "";
    }
}
