namespace MultiPresence.Models.TK8
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Mode { get; init; } = "";
        public string Session { get; init; } = "";
        public bool HasLocalFighter { get; init; }
        public bool IsMatchmaking { get; init; }
        public string Rank { get; init; } = "";
        public int? RankId { get; init; }
        public string LeftFighter { get; init; } = "";
        public string RightFighter { get; init; } = "";
        public int? LocalSide { get; init; }
        public string Stage { get; init; } = "";
        public int? StageId { get; init; }
        public int? LeftRounds { get; init; }
        public int? RightRounds { get; init; }
        public int? RoundsToWin { get; init; }
        public int? CurrentRound { get; init; }
    }
}
