namespace MultiPresence.Models.KHCOM
{
    public class Worlds
    {
        public static async Task<string[]> GetWorld(int world)
        {
            List<string> getworld = new List<string>();

            switch (world)
            {
                case 0:
                    getworld.Add("Castle Oblivion");
                    getworld.Add("castle");
                    break;
                case 1:
                    getworld.Add("Traverse Town");
                    getworld.Add("traversetown");
                    break;
                case 2:
                    getworld.Add("Agrabah");
                    getworld.Add("agrabah");
                    break;
                case 3:
                    getworld.Add("Olympus Coliseum");
                    getworld.Add("olympus");
                    break;
                case 4:
                    getworld.Add("Wonderland");
                    getworld.Add("wonderland");
                    break;
                case 5:
                    getworld.Add("Monstro");
                    getworld.Add("monstro");
                    break;
                case 6:
                    getworld.Add("Halloween Town");
                    getworld.Add("halloweentown");
                    break;
                case 7:
                    getworld.Add("Atlantica");
                    getworld.Add("atlantica");
                    break;
                case 8:
                    getworld.Add("Neverland");
                    getworld.Add("neverland");
                    break;
                case 9:
                    getworld.Add("Hollow Bastion");
                    getworld.Add("hollowbastion");
                    break;
                case 10:
                    getworld.Add("100 Acre Woods");
                    getworld.Add("acre");
                    break;
                case 11:
                    getworld.Add("Twilight Town");
                    getworld.Add("twilighttown");
                    break;
                case 12:
                    getworld.Add("Destiny Islands");
                    getworld.Add("destiny");
                    break;
                case 13:
                    getworld.Add("Castle Oblivion");
                    getworld.Add("castle");
                    break;
                default:
                    getworld.Add("Undefined world");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }
    }
}