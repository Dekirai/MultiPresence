namespace MultiPresence.Models.SPTG
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Loading";
        public string Character { get; init; } = "";
        public int CharacterId { get; init; }
        public int Level { get; init; }
        public string Stage { get; init; } = "";
        public uint StageId { get; init; }
        public string Difficulty { get; init; } = "";
        public int DifficultyId { get; init; }
        public float Health { get; init; }
        public float GutPoints { get; init; }
        public float Defense { get; init; }
        public float Speed { get; init; }
        public float Willpower { get; init; }
        public float Strength { get; init; }
        public int Lives { get; init; }
        public double Money { get; init; }
        public float TotalExperience { get; init; }
    }
}
