namespace MultiPresence.Models.MSM2
{
    internal sealed class GameSnapshot
    {
        public string State { get; set; } = "Default";
        public string Character { get; set; } = "";
        public string CharacterIcon { get; set; } = "logo";
        public int CharacterId { get; set; }
        public int Level { get; set; }
        public int UltimateLevel { get; set; }
        public bool NewGamePlus { get; set; }
        public string Location { get; set; } = "";
        public int LocationId { get; set; }
        public string Difficulty { get; set; } = "";
        public int DifficultyId { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public int SkillPoints { get; set; }
        public int Experience { get; set; }
        public int NextLevelExperience { get; set; }
        public int TotalExperience { get; set; }
    }
}
