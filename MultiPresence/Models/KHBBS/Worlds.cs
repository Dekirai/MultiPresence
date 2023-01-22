namespace MultiPresence.Models.KHBBS
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
                    getworld.Add("The Land of Departure");
                    getworld.Add("tlod");
                    break;
                case 2:
                    getworld.Add("Dwarf Woodlands");
                    getworld.Add("dwarf");
                    break;
                case 3:
                    getworld.Add("Castle of Dreams");
                    getworld.Add("cod");
                    break;
                case 4:
                    getworld.Add("Enchanted Dominion");
                    getworld.Add("dominion");
                    break;
                case 5:
                    getworld.Add("The Mysterious Tower");
                    getworld.Add("tower");
                    break;
                case 6:
                    getworld.Add("Radiant Garden");
                    getworld.Add("radiant");
                    break;
                case 7:
                    getworld.Add("Event");
                    getworld.Add("logo");
                    break;
                case 8:
                    getworld.Add("Olympus Coliseum");
                    getworld.Add("olymp");
                    break;
                case 9:
                    getworld.Add("Deep Space");
                    getworld.Add("deepspace");
                    break;
                case 10:
                    getworld.Add("Destiny Islands");
                    getworld.Add("destiny");
                    break;
                case 11:
                    getworld.Add("Never Land");
                    getworld.Add("neverland");
                    break;
                case 12:
                    getworld.Add("Disney Town");
                    getworld.Add("disneytown");
                    break;
                case 13:
                    getworld.Add("The Keyblade Graveyard");
                    getworld.Add("graveyard");
                    break;
                case 15:
                    getworld.Add("Mirage Arena");
                    getworld.Add("mirage");
                    break;
                case 16:
                    getworld.Add("Command Board");
                    getworld.Add("board");
                    break;
                case 17:
                    getworld.Add("World Map");
                    getworld.Add("worldmap");
                    break;
                case 18:
                    getworld.Add("The Hundred Acre Wood");
                    getworld.Add("acre");
                    break;
                case 19:
                    getworld.Add("The Badlands");
                    getworld.Add("logo");
                    break;
                case 255:
                    getworld.Add("Main Menu");
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