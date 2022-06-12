namespace MultiPresence.Models.T7
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0:
                    getstage.Add("Mishima Dojo");
                    getstage.Add("mishimadojo");
                    break;
                case 1:
                    getstage.Add("Forgotten Realm");
                    getstage.Add("forgottenrealm");
                    break;
                case 2:
                    getstage.Add("Jungle Outpost");
                    getstage.Add("jungleoutpost");
                    break;
                case 3:
                    getstage.Add("Artic Snowfall");
                    getstage.Add("articsnowfall");
                    break;
                case 4:
                    getstage.Add("Twilight Conflict");
                    getstage.Add("twilightconflict");
                    break;
                case 5:
                    getstage.Add("Dragon's Nest");
                    getstage.Add("dragonsnest");
                    break;
                case 6:
                    getstage.Add("Souq");
                    getstage.Add("souq");
                    break;
                case 7:
                    getstage.Add("Devil's Pit");
                    getstage.Add("devilspit");
                    break;
                case 8:
                    getstage.Add("Mishima Building");
                    getstage.Add("mishimabuilding");
                    break;
                case 9:
                    getstage.Add("Abandoned Temple");
                    getstage.Add("abandonedtemple");
                    break;
                case 30:
                    getstage.Add("Duomo Di Sirio");
                    getstage.Add("duomodisirio");
                    break;
                case 31:
                    getstage.Add("Arena");
                    getstage.Add("arena");
                    break;
                case 32:
                    getstage.Add("G Corp. Helipad");
                    getstage.Add("gcorphelipad");
                    break;
                case 33:
                    getstage.Add("G Corp. Helipad (Night)");
                    getstage.Add("gcorphelipadnight");
                    break;
                case 35:
                    getstage.Add("Brimstone & Fire");
                    getstage.Add("brimstoneandfire");
                    break;
                case 36:
                    getstage.Add("Precipice of Fate");
                    getstage.Add("precipiceoffate");
                    break;
                case 37:
                    getstage.Add("Violet Systems");
                    getstage.Add("violetsystem");
                    break;
                case 39:
                    getstage.Add("Kinder Gym");
                    getstage.Add("kindergym");
                    break;
                case 40:
                    getstage.Add("Infinite Azure");
                    getstage.Add("infiniteazure");
                    break;
                case 41:
                    getstage.Add("Geometric Plane");
                    getstage.Add("geometricplane");
                    break;
                case 42:
                    getstage.Add("Warm-Up");
                    getstage.Add("warmup");
                    break;
                case 51:
                    getstage.Add("Howard Estate");
                    getstage.Add("howardestate");
                    break;
                case 52:
                    getstage.Add("Hammerhead");
                    getstage.Add("hammerhead");
                    break;
                case 53:
                    getstage.Add("Jungle Outpost 2");
                    getstage.Add("jungleoutpost2");
                    break;
                case 54:
                    getstage.Add("Twilight Conflict 2");
                    getstage.Add("twilightconflict2");
                    break;
                case 55:
                    getstage.Add("Infinite Azure 2");
                    getstage.Add("infiniteazure2");
                    break;
                case 56:
                    getstage.Add("Last Day on Earth");
                    getstage.Add("lastdayonearth");
                    break;
                case 57:
                    getstage.Add("Cave of Enlightenment");
                    getstage.Add("caveofenlightenment");
                    break;
                case 58:
                    getstage.Add("Vermilion Gates");
                    getstage.Add("vermiliongates");
                    break;
                case 59:
                    getstage.Add("Island Paradise");
                    getstage.Add("islandparadise");
                    break;
                default:
                    getstage.Add("Unknown");
                    getstage.Add("logo");
                    break;
            }
            return getstage.ToArray();
        }
    }
}