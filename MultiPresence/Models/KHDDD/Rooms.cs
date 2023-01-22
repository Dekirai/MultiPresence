namespace MultiPresence.Models.KHDDD
{
    public class Rooms
    {
        public static async Task<string[]> GetRoom(string world)
        {
            List<string> getroom = new List<string>();

            switch (world)
            {
                case "Destiny Islands":
                    getroom.Add("Destiny Islands");
                    getroom.Add("Beach");
                    getroom.Add("Beach");
                    getroom.Add("Beach");
                    getroom.Add("Main Island");
                    getroom.Add("Ocean");
                    break;

                case "Mysterious Tower":
                    getroom.Add("Mysterious Tower");
                    getroom.Add("Chamber");
                    getroom.Add("Tower");
                    getroom.Add("Tower Entrance");
                    getroom.Add("Station of Awakening");
                    getroom.Add("Beach");
                    break;

                case "Traverse Town":
                    getroom.Add("Traverse Town");
                    getroom.Add("First District");
                    getroom.Add("Second District");
                    getroom.Add("Third District");
                    getroom.Add("Fourth District");
                    getroom.Add("Fifth District");
                    getroom.Add("Garden");
                    getroom.Add("Post Office");
                    getroom.Add("Back Streets");
                    getroom.Add("Fountain Plaza");
                    getroom.Add("Fifth District");
                    getroom.Add("Garden");
                    getroom.Add("Fountain Plaza");
                    getroom.Add("Fountain Plaza");
                    getroom.Add("Third District");
                    break;

                case "Country of the Musketeers":
                    getroom.Add("Country of the Musketeers");
                    getroom.Add("The Opéra");
                    getroom.Add("Grand Lobby");
                    getroom.Add("Theatre");
                    getroom.Add("Mont Saint-Michel");
                    getroom.Add("Tower Road");
                    getroom.Add("Tower");
                    getroom.Add("Dungeon");
                    getroom.Add("Training Yard");
                    getroom.Add("Shore");
                    getroom.Add("Green Room");
                    getroom.Add("Machine Room");
                    getroom.Add("Backstage");
                    getroom.Add("Cell");
                    getroom.Add("Mountain Road");
                    getroom.Add("Training Yard");
                    getroom.Add("Theatre");
                    getroom.Add("The Opéra");
                    getroom.Add("Tower");
                    break;

                case "Symphony of Sorcery":
                    getroom.Add("Symphony of Sorcery");
                    getroom.Add("Precipice");
                    getroom.Add("Cloudwalk");
                    getroom.Add("Glen");
                    getroom.Add("???");
                    getroom.Add("Fields");
                    getroom.Add("Moonlight Wood");
                    getroom.Add("Golden Wood");
                    getroom.Add("???");
                    getroom.Add("Snowgleam Wood");
                    getroom.Add("Evil Grounds");
                    getroom.Add("Precipice");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("Chamber");
                    getroom.Add("Chamber");
                    getroom.Add("Tower");
                    getroom.Add("Tower Entrance");
                    getroom.Add("Tower Entrance");
                    break;

                case "Prankster's Paradise":
                    getroom.Add("Prankster's Paradise");
                    getroom.Add("Amusement Park");
                    getroom.Add("Ocean Floor");
                    getroom.Add("Ocean Depths");
                    getroom.Add("Promontory");
                    getroom.Add("Ocean Surface");
                    getroom.Add("Monstro: Mouth");
                    getroom.Add("Monstro: Belly");
                    getroom.Add("Monstro: Gullet");
                    getroom.Add("Monstro: Cavity");
                    getroom.Add("Monstro: Bowels");
                    getroom.Add("Windup Way");
                    getroom.Add("Circus");
                    getroom.Add("Ocean Surface");
                    getroom.Add("Ocean Depths");
                    getroom.Add("Amusement Park");
                    getroom.Add("Promontory");
                    getroom.Add("Ocean Surface");
                    getroom.Add("Monstro: Belly");
                    getroom.Add("Ocean Depths");
                    break;

                case "Radiant Garden":
                    getroom.Add("Radiant Garden");
                    getroom.Add("Ansem's Study");
                    getroom.Add("Control Room");
                    getroom.Add("Station Plaza");
                    getroom.Add("Castle Doors");
                    getroom.Add("Library");
                    break;

                case "La Cité des Cloches":
                    getroom.Add("La Cité des Cloches");
                    getroom.Add("Square");
                    getroom.Add("Square");
                    getroom.Add("Nave");
                    getroom.Add("Bell Tower");
                    getroom.Add("Graveyard Gate");
                    getroom.Add("Tunnels");
                    getroom.Add("Old Graveyard");
                    getroom.Add("Catacombs");
                    getroom.Add("Court of Miracles");
                    getroom.Add("Town");
                    getroom.Add("Bridge");
                    getroom.Add("Outskirts");
                    getroom.Add("Windmill");
                    getroom.Add("Square");
                    getroom.Add("Nave");
                    getroom.Add("Bell Tower");
                    getroom.Add("Windmill");
                    getroom.Add("Bridge");
                    getroom.Add("Square");
                    getroom.Add("Court of Miracles");
                    break;

                case "The Grid":
                    getroom.Add("The Grid");
                    getroom.Add("Portal");
                    getroom.Add("Portal Stairs");
                    getroom.Add("???");
                    getroom.Add("Throneship");
                    getroom.Add("Rectifier 1F");
                    getroom.Add("Solar Sailer");
                    getroom.Add("Docks");
                    getroom.Add("City");
                    getroom.Add("Throughput");
                    getroom.Add("Bridge");
                    getroom.Add("Arena");
                    getroom.Add("Stadium");
                    getroom.Add("Rectifier 2F");
                    getroom.Add("Flynn's Hideout");
                    getroom.Add("Solar Sailer");
                    getroom.Add("Arena");
                    getroom.Add("Portal");
                    getroom.Add("Solar Sailer");
                    break;

                case "The World That Never Was":
                    getroom.Add("The World That Never Was");
                    getroom.Add("Avenue to Dreams");
                    getroom.Add("Contorted City");
                    getroom.Add("Nightmarish Abyss");
                    getroom.Add("Delusive Beginning");
                    getroom.Add("Walk of Delusions");
                    getroom.Add("Fact within Fiction");
                    getroom.Add("Verge of Chaos");
                    getroom.Add("Sanctum of Time");
                    getroom.Add("Darkness's Call");
                    getroom.Add("Darkest End");
                    getroom.Add("Where Nothing Gathers");
                    getroom.Add("Nightmarish Abyss");
                    getroom.Add("Memory's Skyscraper");
                    getroom.Add("Brink of Despair");
                    getroom.Add("First District");
                    getroom.Add("Beach");
                    getroom.Add("Beach");
                    getroom.Add("Beach");
                    getroom.Add("Station of Awakening");
                    getroom.Add("Nightmarish Abyss");
                    break;

                case "World Map":
                    getroom.Add("World Map");
                    getroom.Add("World Map");
                    break;

                case "Spirit Space":
                    getroom.Add("Spirit Space");
                    getroom.Add("Flick Rush");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("???");
                    getroom.Add("Petting Plaza");
                    break;

                case "Main Menu":
                    getroom.Add("Main Menu");
                    break;

            }
            return getroom.ToArray();
        }
    }
}