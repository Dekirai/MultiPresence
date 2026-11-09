namespace MultiPresence.Models.SH2
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Area { get; init; } = "";
        public string AreaId { get; init; } = "";
        public string Room { get; init; } = "";
        public string Location { get; init; } = "";
        public float? Health { get; init; }
        public float? MaxHealth { get; init; }
        public string HealthStatus { get; init; } = "";
        public string Weapon { get; init; } = "";
        public int? Ammo { get; init; }
        public int? ReserveAmmo { get; init; }
        public string CombatDifficulty { get; init; } = "";
        public string PuzzleDifficulty { get; init; } = "";
    }
}
