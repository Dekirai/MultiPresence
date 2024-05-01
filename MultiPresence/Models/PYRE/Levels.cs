namespace MultiPresence.Models.PYRE
{
    public class Levels
    {
        public static async Task<string[]> GetLevel(int level)
        {
            List<string> getlevel = new List<string>();
            switch (level)
            {
                case 0:
                    getlevel.Add("Rookie F");
                    break;
                case 1:
                    getlevel.Add("Rookie E");
                    break;
                case 2:
                    getlevel.Add("Rookie D");
                    break;
                case 3:
                    getlevel.Add("Rookie C");
                    break;
                case 4:
                    getlevel.Add("Rookie B");
                    break;
                case 5:
                    getlevel.Add("Rookie A");
                    break;
                case 6:
                    getlevel.Add("Beginner E");
                    break;
                case 7:
                    getlevel.Add("Beginner D");
                    break;
                case 8:
                    getlevel.Add("Beginner C");
                    break;
                case 9:
                    getlevel.Add("Beginner B");
                    break;
                case 10:
                    getlevel.Add("Beginner A");
                    break;
                case 11:
                    getlevel.Add("Junior E");
                    break;
                case 12:
                    getlevel.Add("Junior D");
                    break;
                case 13:
                    getlevel.Add("Junior C");
                    break;
                case 14:
                    getlevel.Add("Junior B");
                    break;
                case 15:
                    getlevel.Add("Junior A");
                    break;
                case 16:
                    getlevel.Add("Senior E");
                    break;
                case 17:
                    getlevel.Add("Senior D");
                    break;
                case 18:
                    getlevel.Add("Senior C");
                    break;
                case 19:
                    getlevel.Add("Senior B");
                    break;
                case 20:
                    getlevel.Add("Senior A");
                    break;
                case 21:
                    getlevel.Add("Amateur E");
                    break;
                case 22:
                    getlevel.Add("Amateur D");
                    break;
                case 23:
                    getlevel.Add("Amateur C");
                    break;
                case 24:
                    getlevel.Add("Amateur B");
                    break;
                case 25:
                    getlevel.Add("Amateur A");
                    break;
                case 26:
                    getlevel.Add("Semi-Pro E");
                    break;
                case 27:
                    getlevel.Add("Semi-Pro D");
                    break;
                case 28:
                    getlevel.Add("Semi-Pro C");
                    break;
                case 29:
                    getlevel.Add("Semi-Pro B");
                    break;
                case 30:
                    getlevel.Add("Semi-Pro A");
                    break;
                case 31:
                    getlevel.Add("Pro E");
                    break;
                case 32:
                    getlevel.Add("Pro D");
                    break;
                case 33:
                    getlevel.Add("Pro C");
                    break;
                case 34:
                    getlevel.Add("Pro B");
                    break;
                case 35:
                    getlevel.Add("Pro A");
                    break;
                case 36:
                    getlevel.Add("National E");
                    break;
                case 37:
                    getlevel.Add("National D");
                    break;
                case 38:
                    getlevel.Add("National C");
                    break;
                case 39:
                    getlevel.Add("National B");
                    break;
                case 40:
                    getlevel.Add("National A");
                    break;
                case 41:
                    getlevel.Add("World Pro E");
                    break;
                case 42:
                    getlevel.Add("World Pro D");
                    break;
                case 43:
                    getlevel.Add("World Pro C");
                    break;
                case 44:
                    getlevel.Add("World Pro B");
                    break;
                case 45:
                    getlevel.Add("World Pro A");
                    break;
                case 46:
                    getlevel.Add("Master E");
                    break;
                case 47:
                    getlevel.Add("Master D");
                    break;
                case 48:
                    getlevel.Add("Master C");
                    break;
                case 49:
                    getlevel.Add("Master B");
                    break;
                case 50:
                    getlevel.Add("Master A");
                    break;
                case 51:
                    getlevel.Add("Top Master E");
                    break;
                case 52:
                    getlevel.Add("Top Master D");
                    break;
                case 53:
                    getlevel.Add("Top Master C");
                    break;
                case 54:
                    getlevel.Add("Top Master B");
                    break;
                case 55:
                    getlevel.Add("Top Master A");
                    break;
                case 56:
                    getlevel.Add("World Master E");
                    break;
                case 57:
                    getlevel.Add("World Master D");
                    break;
                case 58:
                    getlevel.Add("World Master C");
                    break;
                case 59:
                    getlevel.Add("World Master B");
                    break;
                case 60:
                    getlevel.Add("World Master A");
                    break;
                case 61:
                    getlevel.Add("Legend E");
                    break;
                case 62:
                    getlevel.Add("Legend D");
                    break;
                case 63:
                    getlevel.Add("Legend C");
                    break;
                case 64:
                    getlevel.Add("Legend B");
                    break;
                case 65:
                    getlevel.Add("Legend A");
                    break;
                case 66:
                    getlevel.Add("Infinite Legend E");
                    break;
                case 67:
                    getlevel.Add("Infinite Legend D");
                    break;
                case 68:
                    getlevel.Add("Infinite Legend C");
                    break;
                case 69:
                    getlevel.Add("Infinite Legend B");
                    break;
                case 70:
                    getlevel.Add("Infinite Legend A");
                    break;

                default:
                    getlevel.Add("Unknown level");
                    break;
            }
            return getlevel.ToArray();
        }
    }
}
