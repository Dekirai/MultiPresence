namespace MultiPresence.Models.KH3
{
    public class Worlds
    {
        public static async Task<string[]> GetWorld(string world)
        {
            List<string> getworld = new List<string>();

            switch (world)
            {
                case "bt":
                case "bt_DLC":
                    getworld.Add("Scala ad Caelum");
                    getworld.Add("scala");
                    break;
                case "dw":
                case "dw_DLC":
                    getworld.Add("Dark World");
                    getworld.Add("darkworld");
                    break;
                case "he":
                    getworld.Add("Olympus");
                    getworld.Add("olympus");
                    break;
                case "ts":
                    getworld.Add("Toy Box");
                    getworld.Add("toybox");
                    break;
                case "ra":
                    getworld.Add("Kingdom of Corona");
                    getworld.Add("corona");
                    break;
                case "fz":
                    getworld.Add("Arendelle");
                    getworld.Add("arendelle");
                    break;
                case "ca":
                    getworld.Add("The Caribbean");
                    getworld.Add("caribbean");
                    break;
                case "po":
                    getworld.Add("100 Acre Wood");
                    getworld.Add("100acre");
                    break;
                case "mi":
                    getworld.Add("Monstropolis");
                    getworld.Add("monstropolis");
                    break;
                case "tt":
                    getworld.Add("Twilight Town");
                    getworld.Add("twilighttown");
                    break;
                case "yt":
                    getworld.Add("The Mysterious Tower");
                    getworld.Add("tower");
                    break;
                case "kg":
                case "kg_DLC":
                    getworld.Add("Keyblade Graveyard");
                    getworld.Add("graveyard");
                    break;
                case "bx":
                    getworld.Add("San Fransokyo");
                    getworld.Add("sanfransokyo");
                    break;
                case "ew":
                    getworld.Add("The Final World");
                    getworld.Add("finalworld");
                    break;
                case "di":
                    getworld.Add("Destiny Islands");
                    getworld.Add("destinyislands");
                    break;
                case "rg":
                case "rg_DLC":
                    getworld.Add("Radiant Garden");
                    getworld.Add("radiantgarden");
                    break;
                case "dp":
                    getworld.Add("Land of Departure");
                    getworld.Add("departure");
                    break;
                case "ss":
                case "ss_DLC":
                    getworld.Add("Shibuya");
                    getworld.Add("logo");
                    break;
                case "sf":
                    getworld.Add("Secret Forest");
                    getworld.Add("logo");
                    break;
                case "sp":
                    getworld.Add("Unknown");
                    getworld.Add("logo");
                    break;
                case "gm":
                case "wm":
                    getworld.Add("World Map");
                    getworld.Add("worldmap");
                    break;
                default:
                    getworld.Add("Main Menu");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }

        public static async Task<string[]> GetWorldDE(string world)
        {
            List<string> getworld = new List<string>();

            switch (world)
            {
                case "bt":
                case "bt_DLC":
                    getworld.Add("Scala ad Caelum");
                    getworld.Add("scala");
                    break;
                case "dw":
                case "dw_DLC":
                    getworld.Add("Das Reich der Dunkelheit");
                    getworld.Add("darkworld");
                    break;
                case "he":
                    getworld.Add("Olymp");
                    getworld.Add("olympus");
                    break;
                case "ts":
                    getworld.Add("Spielzeugkiste");
                    getworld.Add("toybox");
                    break;
                case "ra":
                    getworld.Add("Königreich von Corona");
                    getworld.Add("corona");
                    break;
                case "fz":
                    getworld.Add("Arendelle");
                    getworld.Add("arendelle");
                    break;
                case "ca":
                    getworld.Add("Die Karibik");
                    getworld.Add("caribbean");
                    break;
                case "po":
                    getworld.Add("Hundertmorgenwald");
                    getworld.Add("100acre");
                    break;
                case "mi":
                    getworld.Add("Monstropolis");
                    getworld.Add("monstropolis");
                    break;
                case "tt":
                    getworld.Add("Twilight Town");
                    getworld.Add("twilighttown");
                    break;
                case "yt":
                    getworld.Add("Der Mystische Turm");
                    getworld.Add("tower");
                    break;
                case "kg":
                case "kg_DLC":
                    getworld.Add("Der Schlüsselschwertfriedhof");
                    getworld.Add("graveyard");
                    break;
                case "ex":
                    getworld.Add("Kampftor");
                    getworld.Add("battlegates");
                    break;
                case "bx":
                    getworld.Add("San Fransokyo");
                    getworld.Add("sanfransokyo");
                    break;
                case "ew":
                    getworld.Add("Die letzte Welt");
                    getworld.Add("finalworld");
                    break;
                case "di":
                    getworld.Add("Insel des Schicksals");
                    getworld.Add("destinyislands");
                    break;
                case "rg":
                case "rg_DLC":
                    getworld.Add("Radiant Garden");
                    getworld.Add("radiantgarden");
                    break;
                case "dp":
                    getworld.Add("Land des Aufbruchs");
                    getworld.Add("departure");
                    break;
                case "ss":
                case "ss_DLC":
                    getworld.Add("Shibuya");
                    getworld.Add("logo");
                    break;
                case "sf":
                    getworld.Add("Geheimer Wald");
                    getworld.Add("logo");
                    break;
                case "sp":
                    getworld.Add("Unknown");
                    getworld.Add("logo");
                    break;
                case "gm":
                case "wm":
                    getworld.Add("Weltkarte");
                    getworld.Add("worldmap");
                    break;
                default:
                    getworld.Add("Main Menu");
                    getworld.Add("logo");
                    break;
            }
            return getworld.ToArray();
        }
    }
}