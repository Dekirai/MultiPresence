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
                    getcharacter.Add("Haru Estia");
                    getcharacter.Add("haru");
                    break;
                case 2:
                    getcharacter.Add("Erwin Arclight");
                    getcharacter.Add("erwin");
                    break;
                case 3:
                    getcharacter.Add("Lily Bloomerchen");
                    getcharacter.Add("lily");
                    break;
                case 4:
                    getcharacter.Add("Jin Seipatsu");
                    getcharacter.Add("jin");
                    break;
                case 5:
                    getcharacter.Add("Stella Unibell");
                    getcharacter.Add("stella");
                    break;
                case 6:
                    getcharacter.Add("Iris Yuma");
                    getcharacter.Add("iris");
                    break;
                case 7:
                    getcharacter.Add("Chii Aruel");
                    getcharacter.Add("chii");
                    break;
                case 8:
                    getcharacter.Add("Ephnel");
                    getcharacter.Add("ephnel");
                    break;
                case 9:
                    getcharacter.Add("Lee Nabi");
                    getcharacter.Add("nabi");
                    break;
                case 10:
                    getcharacter.Add("Dhana Opini");
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