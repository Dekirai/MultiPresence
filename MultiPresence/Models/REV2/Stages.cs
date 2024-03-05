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
                case 0x0410:
                case 0x0837:
                case 0x0835:
                case 0x0514:
                case 0x0838:
                case 0x08FE:
                case 0x09C9:
                case 0x03E9:
                case 0x044E:
                case 0x04B5:
                case 0x041A:
                case 0x05DF:
                case 0x08FF:
                case 0x04B1:
                case 0x0900:
                case 0x03E8:
                case 0x0840:
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