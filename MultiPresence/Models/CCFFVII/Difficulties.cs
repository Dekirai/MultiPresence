namespace MultiPresence.Models.CCFFVII
{
    public class Difficulties
    {
        public static async Task<string> GetDifficulty(int difficulty)
        {
            string getdifficulty;

            switch (difficulty)
            {
                case 0:
                    getdifficulty = "Normal";
                    break;
                case 1:
                    getdifficulty = "Hard";
                    break;

                default:
                    getdifficulty = "Normal";
                    break;
            }
            return getdifficulty;
        }
    }
}