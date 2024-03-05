namespace MultiPresence.Models.REV2
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0x07D0:
                    getstage.Add("Raid Mode");
                    getstage.Add("In Lobby");
                    break;
                case 0x08FC:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0410:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0837:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0835:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0514:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0838:
                    getstage.Add("Raid Mode");
                    break;
                case 0x08FE:
                    getstage.Add("Raid Mode");
                    break;
                case 0x09C9:
                    getstage.Add("Raid Mode");
                    break;
                case 0x03E9:
                    getstage.Add("Raid Mode");
                    break;
                case 0x044E:
                    getstage.Add("Raid Mode");
                    break;
                case 0x04B5:
                    getstage.Add("Raid Mode");
                    break;
                case 0x041A:
                    getstage.Add("Raid Mode");
                    break;
                case 0x05DF:
                    getstage.Add("Raid Mode");
                    break;
                case 0x08FF:
                    getstage.Add("Raid Mode");
                    break;
                case 0x04B1:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0900:
                    getstage.Add("Raid Mode");
                    break;
                case 0x03E8:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0840:
                    getstage.Add("Raid Mode");
                    break;
                case 0x0517:
                    getstage.Add("Raid Mode");
                    break;
                default:
                    getstage.Add("Raid Mode");
                    break;
            }
            return getstage.ToArray();
        }
    }
}