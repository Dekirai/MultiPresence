namespace MultiPresence.Models.PYRE
{
    public class Characters
    {
        public static async Task<string[]> GetCharacter(int character)
        {
            List<string> getcharacter = new List<string>();
            switch (character)
            {
                case 67108864:
                    getcharacter.Add("Nuri");
                    break;
                case 67108865:
                    getcharacter.Add("Hana");
                    break;
                case 67108866:
                    getcharacter.Add("Azer");
                    break;
                case 67108867:
                    getcharacter.Add("Cecilia");
                    break;
                case 67108868:
                    getcharacter.Add("Max");
                    break;
                case 67108869:
                    getcharacter.Add("Kooh");
                    break;
                case 67108870:
                    getcharacter.Add("Arin");
                    break;
                case 67108871:
                    getcharacter.Add("Kaz");
                    break;
                case 67108872:
                    getcharacter.Add("Lucia");
                    break;
                case 67108873:
                    getcharacter.Add("Nell");
                    break;
                case 67108874:
                    getcharacter.Add("Spika");
                    break;
                case 67108875:
                    getcharacter.Add("Nuri R");
                    break;
                case 67108876:
                    getcharacter.Add("Hana R");
                    break;
                case 67108878:
                    getcharacter.Add("Cecilia R");
                    break;

                default:
                    getcharacter.Add("Unknown character");
                    break;
            }
            return getcharacter.ToArray();
        }
    }
}
