namespace MultiPresence.Models.REV2
{
    public class Chapters
    {
        public static async Task<string> GetChapter(int chapter)
        {
            string getchapter;

            switch (chapter)
            {
                case 1:
                    getchapter = "I";
                    break;
                case 2:
                    getchapter = "II";
                    break;
                case 3:
                    getchapter = "III";
                    break;
                case 4:
                    getchapter = "IV";
                    break;
                case 5:
                    getchapter = "V";
                    break;
                case 6:
                    getchapter = "VI";
                    break;
                case 7:
                    getchapter = "VII";
                    break;
                case 8:
                    getchapter = "VIII";
                    break;
                case 9:
                    getchapter = "?";
                    break;
                case 10:
                    getchapter = "IX";
                    break;


                default:
                    getchapter = "Unknown";
                    break;
            }
            return getchapter;
        }
    }
}