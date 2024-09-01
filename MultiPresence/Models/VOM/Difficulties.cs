namespace MultiPresence.Models.VOM
{
    public class Difficulties
    {
        public static async Task<string> GetDifficulty(int difficulty)
        {
            string getdifficulty;

            switch (difficulty)
            {
                case 0:
                    getdifficulty = "Beginner";
                    break;
                case 1:
                    getdifficulty = "Easy";
                    break;
                case 2:
                    getdifficulty = "Normal";
                    break;
                case 3:
                    getdifficulty = "Hard";
                    break;

                default:
                    getdifficulty = "Normal";
                    break;
            }
            return getdifficulty;
        }

        public static async Task<string> GetDifficultyDE(int difficulty)
        {
            string getdifficulty;

            switch (difficulty)
            {
                case 0:
                    getdifficulty = "Neuling";
                    break;
                case 1:
                    getdifficulty = "Einfach";
                    break;
                case 2:
                    getdifficulty = "Normal";
                    break;
                case 3:
                    getdifficulty = "Schwer";
                    break;

                default:
                    getdifficulty = "Normal";
                    break;
            }
            return getdifficulty;
        }
    }
}