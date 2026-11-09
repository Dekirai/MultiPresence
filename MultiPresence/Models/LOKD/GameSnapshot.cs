namespace MultiPresence.Models.LOKD
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Character { get; init; } = "";
        public int Chapter { get; init; } = -1;
        public string Location { get; init; } = "";
        public string Area { get; init; } = "";
        public string Room { get; init; } = "";
        public string AreaId { get; init; } = "";
        public string Realm { get; init; } = "";
        public string Reaver { get; init; } = "";
        public int ReaverLevel { get; init; } = -1;
        public int ReaverXp { get; init; } = -1;
        public int Health { get; init; } = -1;
        public int MaxHealth { get; init; } = -1;
        public string Objective { get; init; } = "";
        public string PrimaryObjective { get; init; } = "";
        public string SecondaryObjective { get; init; } = "";
    }
}
