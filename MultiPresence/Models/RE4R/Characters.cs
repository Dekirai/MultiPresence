namespace MultiPresence.Models.RE4R
{
    public class Characters
    {
        public static async Task<string> GetCharacter(int characterid)
        {
            string character;

            switch (characterid)
            {
                case 0:
                    character = "Leon";
                    break;
                case 1:
                    character = "Ashley";
                    break;
                case 2:
                    character = "Ada";
                    break;
                case 3:
                    character = "Hunk";
                    break;
                case 4:
                    character = "Krauser";
                    break;
                case 5:
                    character = "Wesker";
                    break;

                default:
                    character = "Unknown character";
                    break;
            }

            return character;
        }
    }
}