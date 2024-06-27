namespace MultiPresence.Models.KHIII
{
    public class Rooms
    {
        public static async Task<string> GetRoom(string room)
        {
            string getroom;

            switch (room)
            {
                case "gm_01":
                    getroom = "Starlight Way";
                    break;
                case "gm_02":
                    getroom = "Misty Stream";
                    break;
                case "gm_03":
                    getroom = "Eclipse Galaxy";
                    break;
                case "gm_50":
                    getroom = "Cutscene";
                    break;
                case "wm_01":
                    getroom = "World Map";
                    break;
                case "tt_01":
                    getroom = "The Neighborhood";
                    break;
                case "tt_40":
                    getroom = "Bistro";
                    break;
                case "tt_50":
                    getroom = "Computer Laboratory";
                    break;
                case "he_01":
                    getroom = "Realm of the Gods";
                    break;
                case "he_02":
                    getroom = "Mount Olympus";
                    break;
                case "he_03":
                    getroom = "Thebes";
                    break;
                case "he_04":
                    getroom = "Thebes";
                    break;
                case "he_05":
                    getroom = "Titans Battle Arena";
                    break;
                case "he_06":
                    getroom = "Titans Battle Tornado";
                    break;
                case "he_50":
                    getroom = "Cutscene";
                    break;
                case "he_52":
                    getroom = "Cutscene";
                    break;

                default:
                    getroom = "Unknown";
                    break;
            }
            return getroom;
        }
    }
}