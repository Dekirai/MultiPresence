namespace MultiPresence.Models.PYRE
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();
            switch (stage)
            {
                case 0:
                    getstage.Add("Blue Lagoon");
                    break;
                case 1:
                    getstage.Add("Blue Water");
                    break;
                case 2:
                    getstage.Add("Sepia Wind");
                    break;
                case 3:
                    getstage.Add("Wind Hill");
                    break;
                case 4:
                    getstage.Add("Wiz Wiz");
                    break;
                case 5:
                    getstage.Add("West Wiz");
                    break;
                case 6:
                    getstage.Add("Blue Moon");
                    break;
                case 7:
                    getstage.Add("Silvia Cannon");
                    break;
                case 8:
                    getstage.Add("Ice Cannon");
                    break;
                case 9:
                    getstage.Add("White Wiz");
                    break;
                case 10:
                    getstage.Add("Shining Sand");
                    break;
                case 11:
                    getstage.Add("Pink Wind");
                    break;
                case 13:
                    getstage.Add("Deep Inferno");
                    break;
                case 14:
                    getstage.Add("Ice Spa");
                    break;
                case 15:
                    getstage.Add("Lost Seaway");
                    break;
                case 16:
                    getstage.Add("Eastern Valley");
                    break;
                case 17:
                    getstage.Add("Special Shuffle Course");
                    break;
                case 18:
                    getstage.Add("Ice Inferno");
                    break;
                case 19:
                    getstage.Add("Wiz City");
                    break;
                case 20:
                    getstage.Add("Abbot Mine");
                    break;
                case 21:
                    getstage.Add("Mystic Ruins");
                    break;
                case 64:
                    getstage.Add("Grand Zodiac");
                    break;
                case 127:
                    getstage.Add("Random");
                    break;

                default:
                    getstage.Add("Unknown Map");
                    break;
            }
            return getstage.ToArray();
        }
    }
}
