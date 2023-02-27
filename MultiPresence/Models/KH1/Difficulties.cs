namespace MultiPresence.Models.KH1
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

                default:
                    getdifficulty = "Standard";
                    break;
            }
            return getdifficulty;
        }
    }
}