namespace MultiPresence.Models.RE4
{
    public class Difficulties
    {
        public static async Task<string> GetDifficulty(int difficultyid)
        {
            string difficulty;

            switch (difficultyid)
            {
                case 3:
                    difficulty = "Easy";
                    break;
                case 0:
                case 1:
                case 2:
                case 4:
                case 5:
                    difficulty = "Normal";
                    break;
                case 6:
                    difficulty = "Professional";
                    break;

                default:
                    difficulty = "Unknown Difficulty";
                    break;
            }

            return difficulty;
        }

        public static async Task<string> GetDifficultyDE(int difficultyid)
        {
            string difficulty;

            switch (difficultyid)
            {
                case 3:
                    difficulty = "Leicht";
                    break;
                case 0:
                case 1:
                case 2:
                case 4:
                case 5:
                    difficulty = "Normal";
                    break;
                case 6:
                    difficulty = "Profi";
                    break;

                default:
                    difficulty = "Unknown Difficulty";
                    break;
            }

            return difficulty;
        }
    }
}