namespace MultiPresence.Models.BXP
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Location { get; init; } = "";
        public string Stage { get; init; } = "";
        public string Character { get; init; } = "";
        public string Speed { get; init; } = "";
        public int? Gold { get; init; }
        public int? Wheat { get; init; }
        public int? Wood { get; init; }
        public int? Stone { get; init; }
        public int? CharacterLevel { get; init; }
        public int? Experience { get; init; }
        public int? MaxExperience { get; init; }
        public int? Level { get; init; }
        public int? Layer { get; init; }
        public int? RunGold { get; init; }
        public int? Health { get; init; }
        public int? MaxHealth { get; init; }
        public int? Kills { get; init; }
    }
}
