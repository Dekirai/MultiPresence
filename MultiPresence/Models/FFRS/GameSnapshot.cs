namespace MultiPresence.Models.FFRS
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Character { get; init; } = "";
        public string Party { get; init; } = "";
        public string Difficulty { get; init; } = "";
        public string Chapter { get; init; } = "";
        public string ChapterTitle { get; init; } = "";
        public string Objective { get; init; } = "";
        public string Location { get; init; } = "";
        public int? Level { get; init; }
        public int? NextLevel { get; init; }
        public int? Health { get; init; }
        public int? MaxHealth { get; init; }
        public int? Mana { get; init; }
        public int? MaxMana { get; init; }
        public int? LimitBurst { get; init; }
        public int? Gil { get; init; }
    }
}
