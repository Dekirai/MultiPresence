namespace MultiPresence.Models.CROC2
{
    internal sealed class GameSnapshot
    {
        public string State { get; set; } = "Default";
        public string Location { get; set; } = "";
        public string World { get; set; } = "";
        public string Level { get; set; } = "";
        public string MapId { get; set; } = "";
        public string AreaType { get; set; } = "";
        public string GameplayMode { get; set; } = "";
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int BonusHearts { get; set; }
        public int Crystals { get; set; }
        public int CurrentCrystals { get; set; }
        public int LevelCrystals { get; set; }
        public int GoldenGobbos { get; set; }
        public int Keys { get; set; }
        public int ClockworkGobbos { get; set; }
        public bool IsOverworld { get; set; }
    }
}
