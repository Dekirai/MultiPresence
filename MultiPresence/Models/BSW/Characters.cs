namespace MultiPresence.Models.BSW
{
    public class Characters
    {
        public static async Task<string[]> GetCharacter(int character)
        {
            List<string> getcharacter = new List<string>();
            switch (character)
            {
                case 1:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 2:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 3:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 4:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 5:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 6:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 7:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 8:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 9:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    break;
                case 10:
                    getcharacter.Add("Dana Opinyi");
                    getcharacter.Add("dana");
                    break;

                default:
                    getcharacter.Add("Unknown");
                    getcharacter.Add("unknown");
                    break;
            }
            return getcharacter.ToArray();
        }
    }
}