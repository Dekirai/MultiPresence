namespace MultiPresence.Models.HL
{
    public static class Difficulties
    {
        public static string GetDifficulty(int difficulty)
        {
            return difficulty switch
            {
                0 => "Story",
                1 => "Easy",
                2 => "Normal",
                3 => "Hard",
                _ => "Unknown Difficulty"
            };
        }
    }
}
