namespace MultiPresence.Models.DMC5
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Mode { get; init; } = "";
        public int? Mission { get; init; }
        public string MissionTitle { get; init; } = "";
        public string Difficulty { get; init; } = "";
        public string Character { get; init; } = "";
        public int? Health { get; init; }
        public int? MaxHealth { get; init; }
        public int? RedOrbs { get; init; }
        public int? BlueOrbs { get; init; }
        public int? PurpleOrbs { get; init; }
        public int? GoldOrbs { get; init; }
    }
}
