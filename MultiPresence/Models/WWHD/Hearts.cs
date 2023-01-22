namespace MultiPresence.Models.WWHD
{
    public class Hearts
    {
        public static async Task<string> GetHearts(int heart)
        {
            string hearts;

            switch (heart)
            {
                case 0:
                    hearts = "0";
                    break;
                case 1:
                    hearts = "0,25";
                    break;
                case 2:
                    hearts = "0,5";
                    break;
                case 3:
                    hearts = "0,75";
                    break;
                case 4:
                    hearts = "1";
                    break;
                case 5:
                    hearts = "1,25";
                    break;
                case 6:
                    hearts = "1,5";
                    break;
                case 7:
                    hearts = "1,75";
                    break;
                case 8:
                    hearts = "2";
                    break;
                case 9:
                    hearts = "2,25";
                    break;
                case 10:
                    hearts = "2,5";
                    break;
                case 11:
                    hearts = "2,75";
                    break;
                case 12:
                    hearts = "3";
                    break;
                case 13:
                    hearts = "3,25";
                    break;
                case 14:
                    hearts = "3,5";
                    break;
                case 15:
                    hearts = "3,75";
                    break;
                case 16:
                    hearts = "4";
                    break;
                case 17:
                    hearts = "4,25";
                    break;
                case 18:
                    hearts = "4,5";
                    break;
                case 19:
                    hearts = "4,75";
                    break;
                case 20:
                    hearts = "5";
                    break;
                case 21:
                    hearts = "5,25";
                    break;
                case 22:
                    hearts = "5,5";
                    break;
                case 23:
                    hearts = "5,75";
                    break;
                case 24:
                    hearts = "6";
                    break;
                case 25:
                    hearts = "6,25";
                    break;
                case 26:
                    hearts = "6,5";
                    break;
                case 27:
                    hearts = "6,75";
                    break;
                case 28:
                    hearts = "7";
                    break;
                case 29:
                    hearts = "7,25";
                    break;
                case 30:
                    hearts = "7,5";
                    break;
                case 31:
                    hearts = "7,75";
                    break;
                case 32:
                    hearts = "8";
                    break;
                case 33:
                    hearts = "8,25";
                    break;
                case 34:
                    hearts = "8,5";
                    break;
                case 35:
                    hearts = "8,75";
                    break;
                case 36:
                    hearts = "9";
                    break;
                case 37:
                    hearts = "9,25";
                    break;
                case 38:
                    hearts = "9,5";
                    break;
                case 39:
                    hearts = "9,75";
                    break;
                case 40:
                    hearts = "10";
                    break;
                case 41:
                    hearts = "10,25";
                    break;
                case 42:
                    hearts = "10,5";
                    break;
                case 43:
                    hearts = "10,75";
                    break;
                case 44:
                    hearts = "11";
                    break;
                case 45:
                    hearts = "11,25";
                    break;
                case 46:
                    hearts = "11,5";
                    break;
                case 47:
                    hearts = "11,75";
                    break;
                case 48:
                    hearts = "12";
                    break;
                case 49:
                    hearts = "12,25";
                    break;
                case 50:
                    hearts = "12,5";
                    break;
                case 51:
                    hearts = "12,75";
                    break;
                case 52:
                    hearts = "13";
                    break;
                case 53:
                    hearts = "13,25";
                    break;
                case 54:
                    hearts = "13,5";
                    break;
                case 55:
                    hearts = "13,75";
                    break;
                case 56:
                    hearts = "14";
                    break;
                case 57:
                    hearts = "14,25";
                    break;
                case 58:
                    hearts = "14,5";
                    break;
                case 59:
                    hearts = "14,75";
                    break;
                case 60:
                    hearts = "15";
                    break;
                case 61:
                    hearts = "15,25";
                    break;
                case 62:
                    hearts = "15,5";
                    break;
                case 63:
                    hearts = "15,75";
                    break;
                case 64:
                    hearts = "16";
                    break;
                case 65:
                    hearts = "16,25";
                    break;
                case 66:
                    hearts = "16,5";
                    break;
                case 67:
                    hearts = "16,75";
                    break;
                case 68:
                    hearts = "17";
                    break;
                case 69:
                    hearts = "17,25";
                    break;
                case 70:
                    hearts = "17,5";
                    break;
                case 71:
                    hearts = "17,75";
                    break;
                case 72:
                    hearts = "18";
                    break;
                case 73:
                    hearts = "18,25";
                    break;
                case 74:
                    hearts = "18,5";
                    break;
                case 75:
                    hearts = "18,75";
                    break;
                case 76:
                    hearts = "19";
                    break;
                case 77:
                    hearts = "19,25";
                    break;
                case 78:
                    hearts = "19,5";
                    break;
                case 79:
                    hearts = "19,75";
                    break;
                case 80:
                    hearts = "20";
                    break;
                default:
                    hearts = "3";
                    break;
            }

            return hearts;
        }
    }
}