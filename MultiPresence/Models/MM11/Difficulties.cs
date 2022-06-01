namespace MultiPresence.Models.MM11
{
    public class Difficulties
    {
        public static async Task<string[]> GetDifficulty(int difficulty)
        {
            List<string> getdifficulty = new List<string>();
            switch (difficulty)
            {
                case 0:
                    getdifficulty.Add("Newcomer");
                    break;
                case 1:
                    getdifficulty.Add("Casual");
                    break;
                case 2:
                    getdifficulty.Add("Normal");
                    break;
                case 3:
                    getdifficulty.Add("Superhero");
                    break;
            }
            return getdifficulty.ToArray();
        }
    }
}