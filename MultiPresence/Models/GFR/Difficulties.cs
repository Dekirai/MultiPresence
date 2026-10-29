namespace MultiPresence.Models.GFR
{
    public static class Difficulties
    {
        public static string GetDifficulty(int round, int cycle)
        {
            if (cycle > 0)
            {
                return "Reincarnation " + cycle;
            }

            switch (round)
            {
                case 1:
                    return "Normal";
                case 2:
                    return "Elite";
                case 3:
                    return "Nightmare";
                default:
                    return "Unknown Difficulty";
            }
        }
    }
}
