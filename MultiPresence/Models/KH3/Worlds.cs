namespace MultiPresence.Models.KH3
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
                case 31:
                    getworld.Add("Main Menu");
                    getworld.Add("logo");
                    break;
                case 1:
                    getworld.Add("Scala ad Caelum");
                    getworld.Add("scala");
                    break;
                case 3:
                    getworld.Add("Dark World");
                    getworld.Add("darkworld");
                    break;
                case 4:
                    getworld.Add("Olympus");
                    getworld.Add("olympus");
                    break;
                case 5:
                    getworld.Add("Toy Box");
                    getworld.Add("toybox");
                    break;
                case 7:
                    getworld.Add("Kingdom of Corona");
                    getworld.Add("corona");
                    break;
                case 8:
                    getworld.Add("Arendelle");
                    getworld.Add("arendelle");
                    break;
                case 9:
                    getworld.Add("The Caribbean");
                    getworld.Add("caribbean");
                    break;
                case 10:
                    getworld.Add("100 Acre Wood");
                    getworld.Add("100acre");
                    break;
                case 11:
                    getworld.Add("Monstropolis");
                    getworld.Add("monstropolis");
                    break;
                case 12:
                    getworld.Add("Twilight Town");
                    getworld.Add("twilighttown");
                    break;
                case 13:
                    getworld.Add("The Mysterious Tower");
                    getworld.Add("tower");
                    break;
                case 14:
                    getworld.Add("Keyblade Graveyard");
                    getworld.Add("graveyard");
                    break;
                case 19:
                    getworld.Add("San Fransokyo");
                    getworld.Add("sanfransokyo");
                    break;
                case 22:
                    getworld.Add("The Final World");
                    getworld.Add("finalworld");
                    break;
                case 23:
                    getworld.Add("Destiny Islands");
                    getworld.Add("destinyislands");
                    break;
                case 24:
                    getworld.Add("Radiant Garden");
                    getworld.Add("radiantgarden");
                    break;
                case 25:
                    getworld.Add("Land of Departure");
                    getworld.Add("departure");
                    break;
                case 27:
                case 28:
                    getworld.Add("World Map");
                    getworld.Add("worldmap");
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