namespace MultiPresence.Models.PYRE
{
    public class Characters
    {
        public static async Task<string[]> GetCharacter(int character)
        {
            List<string> getcharacter = new List<string>();
            switch (character)
            {
                case 0:
                    getcharacter.Add("None");
                    break;
                case 180:
                    getcharacter.Add("Kaz");
                    break;
                case 181:
                    getcharacter.Add("Spika");
                    break;
                case 189:
                    getcharacter.Add("Nuri Classic");
                    break;
                case 190:
                    getcharacter.Add("Hana Classic");
                    break;
                case 191:
                    getcharacter.Add("Azer");
                    break;
                case 192:
                    getcharacter.Add("Cecilia Classic");
                    break;
                case 193:
                    getcharacter.Add("Max");
                    break;
                case 194:
                    getcharacter.Add("Kooh");
                    break;
                case 195:
                    getcharacter.Add("Arin");
                    break;
                case 196:
                    getcharacter.Add("Lucia");
                    break;
                case 197:
                    getcharacter.Add("Nell");
                    break;
                case 198:
                    getcharacter.Add("Nuri");
                    break;
                case 199:
                    getcharacter.Add("Hana");
                    break;
                case 200:
                    getcharacter.Add("Cecilia");
                    break;

                default:
                    getcharacter.Add("Unknown character");
                    break;
            }
            return getcharacter.ToArray();
        }
    }
}
