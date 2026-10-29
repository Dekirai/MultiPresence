namespace MultiPresence.Models.VC
{
    internal sealed record GameSnapshot
    {
        public string State { get; init; } = "Menu";
        public string Activity { get; init; } = "In Main Menu";
        public string DungeonId { get; init; } = "";
        public string[] CharacterIds { get; init; } = [];
        public string[] HandIds { get; init; } = [];
        public int Floor { get; init; }
        public int CompletedFloors { get; init; }
        public int Level { get; init; }
        public int Kills { get; init; }
        public int Coins { get; init; }
        public float Health { get; init; }
        public float MaxHealth { get; init; }
        public float Armor { get; init; }
        public float Mana { get; init; }
        public int StartingMana { get; init; }
        public int Experience { get; init; }
        public int NextLevelExperience { get; init; }
        public int TotalExperience { get; init; }
        public int Turn { get; init; }
        public int Combo { get; init; }
        public int DrawCount { get; init; }
        public int DiscardCount { get; init; }
        public bool PlayerTurn { get; init; }
        public bool Endless { get; init; }
        public bool Hurry { get; init; }
        public bool HandSizeLimit { get; init; }
    }
}
