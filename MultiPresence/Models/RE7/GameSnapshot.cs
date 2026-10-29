namespace MultiPresence.Models.RE7
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Main Menu";
        public string Mode { get; init; } = "";
        public string Character { get; init; } = "";
        public string CharacterId { get; init; } = "";
        public float Health { get; init; }
        public float MaxHealth { get; init; }
        public string HealthStatus { get; init; } = "";
        public int DifficultyId { get; init; } = -1;
        public string Difficulty { get; init; } = "";
        public string Location { get; init; } = "";
        public string Area { get; init; } = "";
        public string Room { get; init; } = "";
        public string RoomId { get; init; } = "";
        public string MapId { get; init; } = "";
        public string Objective { get; init; } = "";
        public string PrimaryObjective { get; init; } = "";
        public string SecondaryObjective { get; init; } = "";
        public string ObjectivePath { get; init; } = "";
        public string Objectives { get; init; } = "";
        public string ObjectiveId { get; init; } = "";
        public string PrimaryObjectiveId { get; init; } = "";
        public string SecondaryObjectiveId { get; init; } = "";
        public string Weapon { get; init; } = "";
        public string WeaponId { get; init; } = "";
        public int Ammo { get; init; } = -1;
        public int ReserveAmmo { get; init; } = -1;
        public bool? InfiniteAmmo { get; init; }
    }
}
