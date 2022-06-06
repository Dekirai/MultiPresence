namespace MultiPresence.Models.KHBBS
{
    public class Characters
    {
        public static async Task<string> GetCharacter(int character)
        {
            string getcharacter;

            switch (character)
            {
                case 1:
                case 4:
                case 7:
                case 10:
                case 13:
                    getcharacter = "Ventus";
                    break;
                case 2:
                case 5:
                case 8:
                case 11:
                case 14:
                    getcharacter = "Aqua";
                    break;
                case 3:
                case 6:
                case 9:
                case 12:
                case 15:
                    getcharacter = "Terra";
                    break;

                default:
                    getcharacter = "Unknown";
                    break;
            }
            return getcharacter;
        }
    }
}