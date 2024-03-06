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
                case 0x03E8:
                case 0x03E9:
                case 0x03EA:
                case 0x03EB:
                case 0x03EC:
                case 0x03F0:
                case 0x03FC:
                case 0x0410:
                case 0x041A:
                case 0x044E:
                case 0x0450:
                case 0x0456:
                case 0x0458:
                case 0x04B1:
                case 0x04B5:
                case 0x0514:
                case 0x0515:
                case 0x0516:
                case 0x0517:
                case 0x0518:
                case 0x05DD:
                case 0x05DF:
                case 0x05E1:
                case 0x05E4:
                case 0x0834:
                case 0x0835:
                case 0x0836:
                case 0x0837:
                case 0x0838:
                case 0x0839:
                case 0x0840:
                case 0x0842:
                case 0x0898:
                case 0x089E:
                case 0x08FC:
                case 0x08FD:
                case 0x08FE:
                case 0x08FF:
                case 0x0900:
                case 0x0902:
                case 0x09C5:
                case 0x09C8:
                case 0x09C9:
                case 0x09CA:
                case 0x09CC:
                case 0x09CD:
                    getstage.Add("Raid Mode");
                    getstage.Add("");
                    break;
                default:
                    getstage.Add("");
                    getstage.Add("");
                    break;
            }
            return getstage.ToArray();
        }
    }
}