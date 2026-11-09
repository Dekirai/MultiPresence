namespace MultiPresence.Models.RE0
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Waiting for game data";
        public string Character { get; init; } = "";
        public int CharacterId { get; init; } = -1;
        public string Difficulty { get; init; } = "";
        public int DifficultyId { get; init; } = -1;
        public string Location { get; init; } = "";
        public string Region { get; init; } = "";
        public string Floor { get; init; } = "";
        public string Room { get; init; } = "";
        public int RoomId { get; init; } = -1;
        public string Weapon { get; init; } = "";
        public int WeaponId { get; init; } = -1;
        public int Ammo { get; init; } = -1;
        public int ReserveAmmo { get; init; } = -1;
        public bool InfiniteAmmo { get; init; }
        public int Health { get; init; } = -1;
    }
}
