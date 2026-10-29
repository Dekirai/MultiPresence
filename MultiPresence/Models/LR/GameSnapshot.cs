namespace MultiPresence.Models.LR
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public int Level { get; init; }
        public bool Online { get; init; }
        public int PlayerCount { get; init; }
        public int MaxPlayers { get; init; }
        public int Chapter { get; init; }
        public string CaseName { get; init; } = "";
        public string CaseAsset { get; init; } = "logo";
        public string Difficulty { get; init; } = "";
        public string CaseType { get; init; } = "";
        public string MapSize { get; init; } = "";
        public int Lives { get; init; }
        public string Objective { get; init; } = "";
        public string Progress { get; init; } = "";
    }
}
