namespace MultiPresence.Models.RE4
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Waiting for game data";
        public string Mode { get; init; } = "";
        public string Character { get; init; } = "";
        public int CharacterId { get; init; } = -1;
        public string Difficulty { get; init; } = "";
        public int DifficultyId { get; init; } = -1;
        public int Health { get; init; } = -1;
        public int MaxHealth { get; init; } = -1;
        public string HealthStatus { get; init; } = "";
        public string Location { get; init; } = "";
        public string Area { get; init; } = "";
        public string AreaIconName { get; init; } = "";
        public string Room { get; init; } = "";
        public int RoomId { get; init; } = -1;
        public int StageId { get; init; } = -1;
        public string Chapter { get; init; } = "";
        public int ChapterId { get; init; } = -1;
        public string Weapon { get; init; } = "";
        public int WeaponId { get; init; } = -1;
        public int Ammo { get; init; } = -1;
        public int ReserveAmmo { get; init; } = -1;
        public bool? InfiniteAmmo { get; init; }
        public int Money { get; init; } = -1;
        public int Score { get; init; } = -1;
    }
}
