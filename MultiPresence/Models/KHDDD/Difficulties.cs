namespace MultiPresence.Models.KHDDD
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
    }
}