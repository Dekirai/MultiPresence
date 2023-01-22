namespace MultiPresence.Models.MM11
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 1:
                    getstage.Add("\"Block Man\"-Stage");
                    getstage.Add("blockman");
                    break;
                case 2:
                    getstage.Add("\"Acid Man\"-Stage");
                    getstage.Add("acidman");
                    break;
                case 3:
                    getstage.Add("\"Impact Man\"-Stage");
                    getstage.Add("impactman");
                    break;
                case 4:
                    getstage.Add("\"Bounce Man\"-Stage");
                    getstage.Add("bounceman");
                    break;
                case 5:
                    getstage.Add("\"Fuse Man\"-Stage");
                    getstage.Add("fuseman");
                    break;
                case 6:
                    getstage.Add("\"Tundra Man\"-Stage");
                    getstage.Add("tundraman");
                    break;
                case 7:
                    getstage.Add("\"Torch Man\"-Stage");
                    getstage.Add("torchman");
                    break;
                case 8:
                    getstage.Add("\"Blast Man\"-Stage");
                    getstage.Add("blastman");
                    break;
                case 9:
                    getstage.Add("\"Yellow Devil\"-Stage");
                    getstage.Add("wiley");
                    break;
                case 10:
                    getstage.Add("\"Mawverne\"-Stage");
                    getstage.Add("wiley");
                    break;
                case 11:
                    getstage.Add("\"Boss Rush\"-Stage");
                    getstage.Add("wiley");
                    break;
                case 12:
                    getstage.Add("\"Dr. Wiley\"-Stage");
                    getstage.Add("wiley");
                    break;
                default:
                    getstage.Add("Stage Select");
                    getstage.Add("logo");
                    break;
            }
            return getstage.ToArray();
        }
    }
}