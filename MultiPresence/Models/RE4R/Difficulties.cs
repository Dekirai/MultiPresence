namespace MultiPresence.Models.RE4R
{
    public class Difficulties
    {
        public static async Task<string> GetDifficulty(int difficultyid)
        {
            string difficulty;

            switch (difficultyid)
            {
                case 10:
                    difficulty = "Assisted";
                    break;
                case 20:
                    difficulty = "Normal";
                    break;
                case 30:
                    difficulty = "Hardcore";
                    break;
                case 40:
                    difficulty = "Professional";
                    break;

                default:
                    difficulty = "Unknown difficulty";
                    break;
            }

            return difficulty;
        }
    }
}