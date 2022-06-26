namespace MultiPresence.Models.KHDDD
{
    public class Worlds
    {
        public static async Task<string[]> GetWorld(int world)
        {
            List<string> getworld = new List<string>();

            //Order:
            //String to show on discord
            //String to use as imagekey

            switch (world)
            {
                case 0:
                    getworld.Add("Main Menu");
                    getworld.Add("logo");
                    break;
                case 1:
                    getworld.Add("Destiny Islands");
                    getworld.Add("destiny");
                    break;
                case 2:
                    getworld.Add("Mysterious Tower");
                    getworld.Add("mysterious");
                    break;
                case 3:
                    getworld.Add("Traverse Town");
                    getworld.Add("traversetown");
                    break;
                case 4:
                    getworld.Add("Country of the Musketeers");
                    getworld.Add("musketeers");
                    break;
                case 5:
                    getworld.Add("Symphony of Sorcery");
                    getworld.Add("symphony");
                    break;
                case 6:
                    getworld.Add("Prankster's Paradise");
                    getworld.Add("prankster");
                    break;
                case 7:
                    getworld.Add("Radiant Garden");
                    getworld.Add("radiant");
                    break;
                case 8:
                    getworld.Add("La Cité des Cloches");
                    getworld.Add("cloches");
                    break;
                case 9:
                    getworld.Add("The Grid");
                    getworld.Add("grid");
                    break;
                case 10:
                    getworld.Add("The World That Never Was");
                    getworld.Add("twtnw");
                    break;
                case 11:
                    getworld.Add("World Map");
                    getworld.Add("worldmap");
                    break;
                case 12:
                    getworld.Add("Spirit Space");
                    getworld.Add("logo");
                    break;
                default:
                    getworld.Add("Undefined location");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }
    }
}