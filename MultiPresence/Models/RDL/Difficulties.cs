namespace MultiPresence.Models.RDL
{
    public class Difficulties
    {
        public static string GetDifficulty(int difficulty)
        {
            string getdifficulty;

            switch (difficulty)
            {
                case 0:
                    getdifficulty = "Couch Potato";
                    break;
                case 1:
                    getdifficulty = "Contestant";
                    break;
                case 2:
                    getdifficulty = "Gladiator";
                    break;
                case 3:
                    getdifficulty = "Hero";
                    break;
                case 4:
                    getdifficulty = "Exterminator";
                    break;

                default:
                    getdifficulty = "Gladiator";
                    break;
            }
            return getdifficulty;
        }
    }
}