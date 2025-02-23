namespace MultiPresence.Models.RE4R
{
    public class Chapters
    {
        public static async Task<string> GetChapter(int chapterid)
        {
            string chapter;

            switch (chapterid)
            {
                case 21100:
                case 30100:
                    chapter = "1";
                    break;
                case 21200:
                case 31100:
                case 31200:
                    chapter = "2";
                    break;
                case 21300:
                case 32100:
                case 32200:
                    chapter = "3";
                    break;
                case 22100:
                case 33100:
                    chapter = "4";
                    break;
                case 22200:
                case 33200:
                    chapter = "5";
                    break;
                case 22300:
                case 34100:
                    chapter = "6";
                    break;
                case 23100:
                case 35100:
                    chapter = "7";
                    break;
                case 23200:
                    chapter = "8";
                    break;
                case 23300:
                    chapter = "9";
                    break;
                case 24100:
                    chapter = "10";
                    break;
                case 24200:
                    chapter = "11";
                    break;
                case 24300:
                    chapter = "12";
                    break;
                case 25100:
                    chapter = "13";
                    break;
                case 25200:
                    chapter = "14";
                    break;
                case 25300:
                    chapter = "15";
                    break;
                case 25400:
                    chapter = "16";
                    break;

                default:
                    chapter = "Unknown chapter";
                    break;
            }

            return chapter;
        }
    }
}