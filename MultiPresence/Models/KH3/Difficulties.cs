namespace MultiPresence.Models.KH3
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
                    getdifficulty = "Standard";
                    break;
                case 2:
                    getdifficulty = "Proud";
                    break;
                case 3:
                    getdifficulty = "Critical";
                    break;

                default:
                    getdifficulty = "Standard";
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
                    getdifficulty = "Anfänger";
                    break;
                case 1:
                    getdifficulty = "Normal";
                    break;
                case 2:
                    getdifficulty = "Profi";
                    break;
                case 3:
                    getdifficulty = "Veteran";
                    break;

                default:
                    getdifficulty = "Normal";
                    break;
            }
            return getdifficulty;
        }
    }
}