namespace MultiPresence.Models.DDV
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Location { get; init; } = "";
        public string Quest { get; init; } = "";
        public string Objective { get; init; } = "";
        public string PlayerName { get; init; } = "";
        public long? Money { get; init; }
        public long? Dreamlight { get; init; }
        public int? Level { get; init; }
        public int? Energy { get; init; }
        public int? MaxEnergy { get; init; }
    }
}
