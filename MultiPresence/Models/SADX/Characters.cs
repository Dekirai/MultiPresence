namespace MultiPresence.Models.SADX
{
    public class Characters
    {
        public static async Task<string> GetCharacter(int characterid)
        {
            string character;

            switch (characterid)
            {
                case 0:
                    character = "Sonic";
                    break;
                case 1:
                    character = "Tails";
                    break;
                case 2:
                    character = "Knuckles";
                    break;
                case 3:
                    character = "Amy";
                    break;
                case 4:
                    character = "Big";
                    break;
                case 5:
                    character = "E-102";
                    break;
                case 6:
                    character = "Super Sonic"; //?
                    break;

                default:
                    character = "Unknown character";
                    break;
            }

            return character;
        }
    }
}