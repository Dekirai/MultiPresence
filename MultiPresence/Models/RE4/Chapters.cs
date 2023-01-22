namespace MultiPresence.Models.RE4
{
    public class Chapters
    {
        public static async Task<string> GetChapter(int chapterid)
        {
            string chapter;

            switch (chapterid)
            {
                case 0:
                    chapter = "1-1";
                    break;
                case 1:
                    chapter = "1-2";
                    break;
                case 2:
                    chapter = "1-3";
                    break;
                case 3:
                    chapter = "2-1";
                    break;
                case 4:
                    chapter = "2-2";
                    break;
                case 5:
                    chapter = "2-3";
                    break;
                case 6:
                    chapter = "3-1";
                    break;
                case 7:
                    chapter = "3-2";
                    break;
                case 8:
                    chapter = "3-3";
                    break;
                case 9:
                    chapter = "3-4";
                    break;
                case 10:
                    chapter = "4-1";
                    break;
                case 11:
                    chapter = "4-2";
                    break;
                case 12:
                    chapter = "4-3";
                    break;
                case 13:
                    chapter = "4-4";
                    break;
                case 14:
                    chapter = "5-1";
                    break;
                case 15:
                    chapter = "5-2";
                    break;
                case 16:
                    chapter = "5-3";
                    break;
                case 17:
                    chapter = "5-4";
                    break;
                case 18:
                    chapter = "Final Chapter";
                    break;
                case 20:
                    chapter = "1";
                    break;
                case 21:
                    chapter = "2";
                    break;
                case 22:
                    chapter = "3";
                    break;
                case 23:
                    chapter = "4";
                    break;
                case 24:
                    chapter = "5";
                    break;


                default:
                    chapter = "Unknown chapter";
                    break;
            }

            return chapter;
        }
    }
}