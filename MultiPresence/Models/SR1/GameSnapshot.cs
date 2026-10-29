namespace MultiPresence.Models.SR1
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Area { get; init; } = "";
        public string Location { get; init; } = "";
        public string Realm { get; init; } = "";
        public string Objective { get; init; } = "";
        public int Health { get; init; }
        public int MaxHealth { get; init; }
        public int Mana { get; init; }
        public int MaxMana { get; init; }
        public int HealthUpgrades { get; init; }
        public int EnergyUpgrades { get; init; }
    }
}
