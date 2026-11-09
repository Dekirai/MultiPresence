namespace MultiPresence.Models.SHF
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Location { get; init; } = "";
        public string AreaId { get; init; } = "";
        public string Objective { get; init; } = "";
        public string ObjectiveName { get; init; } = "";
        public int? ObjectiveCurrent { get; init; }
        public int? ObjectiveMax { get; init; }
        public float? Health { get; init; }
        public float? MaxHealth { get; init; }
        public float? Stamina { get; init; }
        public float? MaxStamina { get; init; }
    }
}
