namespace MultiPresence.Models.SA2
{
    public class Characters
    {
        public static async Task<string[]> GetCharacter(int character)
        {
            List<string> getcharacter = new List<string>();
            switch (character)
            {
                case 0:
                    getcharacter.Add("Sonic");
                    getcharacter.Add("sonic");
                    getcharacter.Add("Amy");
                    getcharacter.Add("amy");
                    break;
                case 1:
                    getcharacter.Add("Shadow");
                    getcharacter.Add("shadow");
                    getcharacter.Add("Metal Sonic");
                    getcharacter.Add("metalsonic");
                    break;
                case 2:
                    getcharacter.Add("Tails");
                    getcharacter.Add("tails");
                    break;
                case 3:
                    getcharacter.Add("Eggman");
                    getcharacter.Add("eggman");
                    break;
                case 4:
                    getcharacter.Add("Knuckles");
                    getcharacter.Add("knuckles");
                    getcharacter.Add("Tikal");
                    getcharacter.Add("tikal");
                    break;
                case 5:
                    getcharacter.Add("Rogue");
                    getcharacter.Add("rogue");
                    getcharacter.Add("Chaos");
                    getcharacter.Add("chaos");
                    break;
                case 6:
                    getcharacter.Add("Tails");
                    getcharacter.Add("tails");
                    getcharacter.Add("Hero Chao");
                    getcharacter.Add("herochao");
                    break;
                case 7:
                    getcharacter.Add("Eggman");
                    getcharacter.Add("eggman");
                    getcharacter.Add("Dark Chao");
                    getcharacter.Add("darkchao");
                    break;

                default:
                    getcharacter.Add("Unknown");
                    getcharacter.Add("unknown");
                    getcharacter.Add("Unknown");
                    getcharacter.Add("unknown");
                    break;
            }
            return getcharacter.ToArray();
        }
    }
}