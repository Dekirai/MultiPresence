namespace MultiPresence.Models.RE
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public int Floor { get; init; }
        public int Room { get; init; }
        public int? Camera { get; init; }
        public int Character { get; init; }
        public int Weapon { get; init; }
        public int Health { get; init; }
        public int MaxHealth { get; init; }
        public byte AmmoClip { get; init; }
        public byte MaxAmmoClip { get; init; }
    }
}
