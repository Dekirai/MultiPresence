namespace MultiPresence.Models.T7
{
    public class Characters
    {
        public static async Task<string[]> GetCharacter(int character)
        {
            List<string> getcharacter = new List<string>();
            switch (character)
            {
                case 0:
                    getcharacter.Add("Paul");
                    break;
                case 1:
                    getcharacter.Add("Law");
                    break;
                case 2:
                    getcharacter.Add("King");
                    break;
                case 3:
                    getcharacter.Add("Yoshimitsu");
                    break;
                case 4:
                    getcharacter.Add("Hworang");
                    break;
                case 5:
                    getcharacter.Add("Xiaoyu");
                    break;
                case 6:
                    getcharacter.Add("Jin");
                    break;
                case 7:
                    getcharacter.Add("Bryan");
                    break;
                case 8:
                    getcharacter.Add("Heihachi");
                    break;
                case 9:
                    getcharacter.Add("Kazuya");
                    break;
                case 10:
                    getcharacter.Add("Steve");
                    break;
                case 11:
                    getcharacter.Add("Jack-7");
                    break;
                case 12:
                    getcharacter.Add("Asuka");
                    break;
                case 13:
                    getcharacter.Add("Devil Jin");
                    break;
                case 14:
                    getcharacter.Add("Feng");
                    break;
                case 15:
                    getcharacter.Add("Lili");
                    break;
                case 16:
                    getcharacter.Add("Dragunov");
                    break;
                case 17:
                    getcharacter.Add("Leo");
                    break;
                case 18:
                    getcharacter.Add("Lars");
                    break;
                case 19:
                    getcharacter.Add("Alisa");
                    break;
                case 20:
                    getcharacter.Add("Claudio");
                    break;
                case 21:
                    getcharacter.Add("Katarina");
                    break;
                case 22:
                    getcharacter.Add("Lucky Chloe");
                    break;
                case 23:
                    getcharacter.Add("Shaheen");
                    break;
                case 24:
                    getcharacter.Add("Josie");
                    break;
                case 25:
                    getcharacter.Add("Gigas");
                    break;
                case 26:
                    getcharacter.Add("Kazumi");
                    break;
                case 28:
                    getcharacter.Add("Nina");
                    break;
                case 29:
                    getcharacter.Add("Master Raven");
                    break;
                case 30:
                    getcharacter.Add("Lee");
                    break;
                case 31:
                    getcharacter.Add("Bob");
                    break;
                case 32:
                    getcharacter.Add("Akuma");
                    break;
                case 33:
                    getcharacter.Add("Kuma");
                    break;
                case 34:
                    getcharacter.Add("Panda");
                    break;
                case 35:
                    getcharacter.Add("Eddy");
                    break;
                case 36:
                    getcharacter.Add("Eliza");
                    break;
                case 37:
                    getcharacter.Add("Miguel");
                    break;
                case 43:
                    getcharacter.Add("Geese");
                    break;
                case 44:
                    getcharacter.Add("Noctis");
                    break;
                case 45:
                    getcharacter.Add("Anna");
                    break;
                case 46:
                    getcharacter.Add("Lei");
                    break;
                case 47:
                    getcharacter.Add("Marduk");
                    break;
                case 48:
                    getcharacter.Add("Armor King");
                    break;
                case 49:
                    getcharacter.Add("Julia");
                    break;
                case 50:
                    getcharacter.Add("Negan");
                    break;
                case 51:
                    getcharacter.Add("Zafina");
                    break;
                case 52:
                    getcharacter.Add("Ganryu");
                    break;
                case 53:
                    getcharacter.Add("Leroy");
                    break;
                case 54:
                    getcharacter.Add("Fahkumram");
                    break;
                case 55:
                    getcharacter.Add("Kunimitsu");
                    break;
                case 56:
                    getcharacter.Add("Lidia");
                    break;

                default:
                    getcharacter.Add("Unknown");
                    break;
            }
            return getcharacter.ToArray();
        }
    }
}