namespace MultiPresence.Models.REV2
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public int StageId { get; init; }
        public int? CampaignMode { get; init; }
        public string Location { get; init; } = "";
        public int? Episode { get; init; }
        public string CampaignCharacter { get; init; } = "";
        public string Partner { get; init; } = "";
        public string Objective { get; init; } = "";
        public int? ChapterId { get; init; }
        public int? Mission { get; init; }
        public int? DifficultyId { get; init; }
        public int? RecommendedLevel { get; init; }
        public int? Kills { get; init; }
        public int? EnemyTarget { get; init; }
        public int? Badges { get; init; }
        public int? CharacterId { get; init; }
        public int? Level { get; init; }
        public long? Money { get; init; }
        public int? RedHearts { get; init; }
        public int? BlueHearts { get; init; }
        public string PlayerName { get; init; } = "";
    }
}
