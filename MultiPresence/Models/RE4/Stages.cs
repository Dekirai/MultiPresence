namespace MultiPresence.Models.RE4
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 1: //Village Part
                    getstage.Add("The Road to Pueblo");
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Farm");
                    getstage.Add("Ganado Gorge");
                    getstage.Add("Mendez Manor");
                    getstage.Add("House of Confinement");
                    getstage.Add("Ganado Base");
                    getstage.Add("Cemetery");
                    getstage.Add("Quarry");
                    getstage.Add("Swamp");
                    getstage.Add("Lake");
                    getstage.Add("Waterfall");
                    getstage.Add("Merchant"); //Needs real name
                    getstage.Add("Merchant's Dock");
                    getstage.Add("Village Gate and Gondola");
                    getstage.Add("");
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Farm");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Church");
                    getstage.Add("Cemetery");
                    getstage.Add("Quarry");
                    getstage.Add("");
                    getstage.Add("Lake");
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Ganado Stronghold");
                    getstage.Add("Gigante Way");
                    getstage.Add("Shed of Enlightenment");
                    break;
                case 2: //Castle Part
                    getstage.Add("Road to the Castle");
                    getstage.Add("Grand Hall");
                    getstage.Add("Castle Ramparts");
                    getstage.Add("Castle Gate");
                    getstage.Add("Great Hall");
                    getstage.Add("Prison");
                    getstage.Add("Theater Hall");
                    getstage.Add("Entry Hall");
                    getstage.Add("Hall of Worship");
                    getstage.Add("Gallery");
                    getstage.Add("Castle Wall");
                    getstage.Add("Bedroom Garden");
                    getstage.Add("Dining Hall");
                    getstage.Add("Study");
                    getstage.Add("Storage");
                    getstage.Add("Exhibition Chamber");
                    getstage.Add("Transport Cart");
                    getstage.Add("Corridor of Royalty");
                    getstage.Add("Weapon Exhibit");
                    getstage.Add("Ballroom");
                    getstage.Add("Exterior Clock Tower");
                    getstage.Add("Throne"); //Needs real name
                    getstage.Add("Armor Exhibit");
                    getstage.Add("Clock Tower");
                    getstage.Add("Throne Room Hall");
                    getstage.Add("Transport Cat");
                    getstage.Add("Underground Ruins");
                    getstage.Add("Mine Rail");
                    getstage.Add("");
                    getstage.Add("Catacombs");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Mine Entrance");
                    getstage.Add("Service Tunnel");
                    getstage.Add("Lava Room"); //Needs real name
                    getstage.Add("Excavation Site");
                    getstage.Add("Inferno Chamber");
                    getstage.Add("Ruins");
                    getstage.Add("Salazar Statue Room"); //Needs real name
                    getstage.Add("Salazar's Tower");
                    getstage.Add("Tower Apex");
                    getstage.Add("Sewer");
                    getstage.Add("Dock");
                    getstage.Add("Elevator");
                    getstage.Add("Minigame");
                    break;
                case 3: //Island Part
                    getstage.Add("Outer Stronghold");
                    getstage.Add("Cliffside Encampment");
                    getstage.Add("");
                    getstage.Add("Larder");
                    getstage.Add("Security");
                    getstage.Add("Bunker");
                    getstage.Add("Stairwell");
                    getstage.Add("Operating Room");
                    getstage.Add("Freezer");
                    getstage.Add("Research Lab");
                    getstage.Add("Com Tower");
                    getstage.Add("Waste Disposal");
                    getstage.Add("Storage Room");
                    getstage.Add("Basement");
                    getstage.Add("Administration Corridor");
                    getstage.Add("Road to Annex"); //Needs real name (Where Ashley drives the big machine)
                    getstage.Add("Dust"); //Needs real name (Where Leon & Ashley jump into the trash)
                    getstage.Add("Incinerator");
                    getstage.Add("Merchant Room");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Dozer Crash Site");
                    getstage.Add("Chapel and Furnace");
                    getstage.Add("Machine Room");
                    getstage.Add("Saddler's Quarters");
                    getstage.Add("");
                    getstage.Add("Cavern Entrance");
                    getstage.Add("Cavern");
                    getstage.Add("Krauser's Stronghold");
                    getstage.Add("Militant Camp");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Fort"); //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Cliffside Ruins");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Ancient Passage");
                    getstage.Add("Penitentiary");
                    getstage.Add("Security Checkpoint");
                    getstage.Add("");
                    getstage.Add("Capsule Chamber");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Extraction Chamber");
                    getstage.Add("Construction Yard");
                    getstage.Add("Construction Building");
                    getstage.Add("Underground Water Vein"); //Needs real name (Escaping with the boat)
                    break;
                default:
                    getstage.Add("Unknown Location");
                    break;
            }
            return getstage.ToArray();
        }
    }
}