namespace MultiPresence.Models.CRYO
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public bool Online { get; init; }
        public int PlayerCount { get; init; }
        public int MaxPlayers { get; init; }
        public string Map { get; init; } = "";
        public string MapAsset { get; init; } = "logo";
        public string Difficulty { get; init; } = "";
        public string Objective { get; init; } = "";
        public string Progress { get; init; } = "";
    }
}
