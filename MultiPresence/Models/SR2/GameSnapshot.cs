namespace MultiPresence.Models.SR2
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Default";
        public string Area { get; init; } = "";
        public string Location { get; init; } = "";
        public string Realm { get; init; } = "";
        public string Reaver { get; init; } = "";
        public int Health { get; init; }
        public int MaxHealth { get; init; }
    }
}
