namespace MultiPresence.Models.SXSG
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Loading";
        public string Stage { get; init; } = "";
        public string StageId { get; init; } = "";
        public string Character { get; init; } = "";
        public string Mode { get; init; } = "";
        public int Rings { get; init; }
        public double Time { get; init; }
    }
}
