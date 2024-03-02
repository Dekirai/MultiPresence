namespace MultiPresence.Models.REV2
{
    public class Characters
    {
        public static async Task<string> GetCharacter(int character)
        {
            string getcharacter;

            switch (character)
            {
                case 0:
                case 1:
                case 2:
                    getcharacter = "Barry";
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    getcharacter = "Claire";
                    break;
                case 7:
                case 8:
                case 9:
                    getcharacter = "Moira";
                    break;
                case 10:
                    getcharacter = "Neil";
                    break;
                case 11:
                    getcharacter = "Pedro";
                    break;
                case 12:
                    getcharacter = "Evgeny";
                    break;
                case 13:
                    getcharacter = "Gabe";
                    break;
                case 14:
                    getcharacter = "Alex";
                    break;
                case 15:
                case 22:
                    getcharacter = "Gina";
                    break;
                case 16:
                    getcharacter = "Chris";
                    break;
                case 17:
                    getcharacter = "Leon";
                    break;
                case 18:
                    getcharacter = "Jill";
                    break;
                case 19:
                    getcharacter = "Albert";
                    break;
                case 20:
                    getcharacter = "Hunk";
                    break;
                case 21:
                    getcharacter = "Cipher";
                    break;
                case 23:
                    getcharacter = "LADY HUNK";
                    break;
                case 24:
                    getcharacter = "Rachael";
                    break;
                case 25:
                    getcharacter = "Lottie";
                    break;
                case 26:
                    getcharacter = "Mutant Pedro";
                    break;
                case 27:
                    getcharacter = "Jessica";
                    break;

                default:
                    getcharacter = "Unknown";
                    break;
            }
            return getcharacter;
        }
    }
}