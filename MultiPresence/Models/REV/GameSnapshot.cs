namespace MultiPresence.Models.REV
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public int? Episode { get; init; }
        public string EpisodeTitle { get; init; } = "";
        public int? DifficultyId { get; init; }
        public string Character { get; init; } = "";
        public string Partner { get; init; } = "";
        public string Location { get; init; } = "";
        public string Objective { get; init; } = "";
        public string Stage { get; init; } = "";
        public string StageName { get; init; } = "";
        public int? Level { get; init; }
    }
}
