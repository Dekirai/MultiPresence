namespace MultiPresence.Models.PYRE
{
    public class Modes
    {
        public static async Task<string[]> GetMode(int mode)
        {
            List<string> getmode = new List<string>();
            switch (mode)
            {
                case 0:
                    getmode.Add("Stroke");
                    break;
                case 1:
                    getmode.Add("Match");
                    break;
                case 2:
                    getmode.Add("Lounge");
                    break;
                case 4:
                    getmode.Add("Tourney");
                    break;
                case 7:
                    getmode.Add("Pang Battle");
                    break;
                case 10:
                    getmode.Add("Approach");
                    break;
                case 11:
                    getmode.Add("Chip Mode");
                    break;
                case 23:
                case 40:
                case 255:
                    getmode.Add("Lobby");
                    break;

                default:
                    getmode.Add("Unknown Mode");
                    break;
            }
            return getmode.ToArray();
        }
    }
}
