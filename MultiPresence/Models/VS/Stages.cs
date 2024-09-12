namespace MultiPresence.Models.VS
{
    public class Stages
    {
        public static async Task<string> GetStages(int stage)
        {
            string getstage;

            switch (stage)
            {
                case 0:
                    getstage = "Mad Forest";
                    break;
                case 1:
                    getstage = "Moongolow";
                    break;
                case 3:
                    getstage = "Inlaid Library";
                    break;
                case 7:
                    getstage = "Green Acres";
                    break;
                case 8:
                    getstage = "Dairy Plant";
                    break;
                case 9:
                    getstage = "Il Molise";
                    break;
                case 10:
                    getstage = "Gallo Tower";
                    break;
                case 11:
                    getstage = "The Bone Zone";
                    break;
                case 12:
                    getstage = "Holy Forbidden";
                    break;
                case 13:
                    getstage = "Cappella Magna";
                    break;
                case 14:
                    getstage = "Boss Rash";
                    break;
                case 15:
                    getstage = "Eudaimonia M.";
                    break;
                case 17:
                    getstage = "Tiny Bridge";
                    break;
                case 20:
                    getstage = "Mt.Moonspell";
                    break;
                case 21:
                    getstage = "Bat Country";
                    break;
                case 22:
                    getstage = "Astral Stair";
                    break;
                case 23:
                    getstage = "Lake Foscari";
                    break;
                case 24:
                    getstage = "Abyss Foscari";
                    break;
                case 25:
                    getstage = "Whiteout";
                    break;
                case 26:
                    getstage = "Polus Replica";
                    break;
                case 27:
                    getstage = "Space 54";
                    break;
                case 28:
                    getstage = "Neo Galuga";
                    break;
                case 29:
                    getstage = "Hectic Highway";
                    break;
                case 30:
                    getstage = "Room 1665";
                    break;
                case 31:
                    getstage = "Laborratory";
                    break;
                case 33:
                    getstage = "Carlo Cart";
                    break;

                default:
                    getstage = "Unknown";
                    break;
            }
            return getstage;
        }
    }
}