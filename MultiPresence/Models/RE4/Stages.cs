namespace MultiPresence.Models.RE4
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0: //Debug Rooms
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("ATARI TEST ROOM");
                    getstage.Add("test");
                    getstage.Add("DON BEYA");
                    getstage.Add(""); //Room 5 
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 10
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 15
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 20
                    getstage.Add("EFFECT TEST");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 25
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("EFF TEST0");
                    getstage.Add("EFF TEST1");
                    getstage.Add("EFF TEST2");
                    getstage.Add("EFF TEST3"); //Room 35
                    getstage.Add("EFF TEST4");
                    getstage.Add("EFF TEST5");
                    getstage.Add("EFF TEST5");
                    break;
                case 1: //Village Part
                    getstage.Add("The Road to Pueblo");
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Farm");
                    getstage.Add("Ganado Gorge");
                    getstage.Add("Mendez Manor"); //Room 5
                    getstage.Add("House of Confinement");
                    getstage.Add("Ganado Base");
                    getstage.Add("Cemetery");
                    getstage.Add("Quarry");
                    getstage.Add("Swamp"); //Room 10
                    getstage.Add("Lake");
                    getstage.Add("Waterfall");
                    getstage.Add("Lake Cave");
                    getstage.Add("Merchant's Dock");
                    getstage.Add("Village Gate and Gondola"); //Room 15
                    getstage.Add("");
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Farm");
                    getstage.Add(""); //Room 20
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Church");
                    getstage.Add("Cemetery");
                    getstage.Add("Quarry"); //Room 25
                    getstage.Add("Swamp");
                    getstage.Add("Lake");
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Ganado Stronghold");
                    getstage.Add("Gigante Way"); //Room 30
                    getstage.Add("Shed of Enlightenment");
                    break;
                case 2: //Castle Part
                    getstage.Add("Road to the Castle");
                    getstage.Add("Grand Hall");
                    getstage.Add("Castle Ramparts");
                    getstage.Add("Castle Gate");
                    getstage.Add("Great Hall");
                    getstage.Add("Prison"); //Room 5
                    getstage.Add("Theater Hall");
                    getstage.Add("Entry Hall");
                    getstage.Add("Hall of Worship");
                    getstage.Add("Gallery");
                    getstage.Add("Castle Wall"); //Room 10
                    getstage.Add("Bedroom Garden");
                    getstage.Add("Dining Hall");
                    getstage.Add("Study");
                    getstage.Add("Storage");
                    getstage.Add("Exhibition Chamber"); //Room 15
                    getstage.Add("Transport Cart");
                    getstage.Add("Corridor of Royalty");
                    getstage.Add("Weapon Exhibit");
                    getstage.Add("Ballroom");
                    getstage.Add("Exterior Clock Tower"); //Room 20
                    getstage.Add("Throne"); //Needs real name
                    getstage.Add("Armor Exhibit");
                    getstage.Add("Clock Tower");
                    getstage.Add("Throne Room Hall");
                    getstage.Add("Transport Cat"); //Room 25
                    getstage.Add("Underground Ruins");
                    getstage.Add("Mine Rail");
                    getstage.Add("");
                    getstage.Add("Catacombs");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("Mine Entrance");
                    getstage.Add("Service Tunnel");
                    getstage.Add("Lava Room"); //Needs real name
                    getstage.Add("Excavation Site"); //Room 35
                    getstage.Add("Inferno Chamber");
                    getstage.Add("Ruins");
                    getstage.Add("Salazar Statue Room"); //Needs real name
                    getstage.Add("Salazar's Tower");
                    getstage.Add("Tower Apex"); //Room 40
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
                    getstage.Add("Bunker"); //Room 5
                    getstage.Add("Stairwell");
                    getstage.Add("Operating Room");
                    getstage.Add("Freezer");
                    getstage.Add("Research Lab");
                    getstage.Add("Com Tower"); //Room 10
                    getstage.Add("Waste Disposal");
                    getstage.Add("Storage Room");
                    getstage.Add("Basement");
                    getstage.Add("Administration Corridor");
                    getstage.Add("Road to Annex"); //Room 15 //Needs real name (Where Ashley drives the big machine)
                    getstage.Add("Dust"); //Needs real name (Where Leon & Ashley jump into the trash)
                    getstage.Add("Incinerator");
                    getstage.Add("Merchant Room");
                    getstage.Add("");
                    getstage.Add(""); //Room 20
                    getstage.Add("Dozer Crash Site");
                    getstage.Add("Chapel and Furnace");
                    getstage.Add("Machine Room");
                    getstage.Add("Saddler's Quarters");
                    getstage.Add(""); //Room 25
                    getstage.Add("Cavern Entrance");
                    getstage.Add("Cavern");
                    getstage.Add("Krauser's Stronghold");
                    getstage.Add("Militant Camp");
                    getstage.Add(""); //Room 30
                    getstage.Add("");
                    getstage.Add("Fort"); //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Cliffside Ruins");
                    getstage.Add("");
                    getstage.Add(""); //Room 35
                    getstage.Add("");
                    getstage.Add("Ancient Passage");
                    getstage.Add("Penitentiary");
                    getstage.Add("Security Checkpoint");
                    getstage.Add(""); //Room 40
                    getstage.Add("Capsule Chamber");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 45
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Extraction Chamber");
                    getstage.Add("Construction Yard");
                    getstage.Add("Construction Building"); //Room 50
                    getstage.Add("Underground Water Vein"); //Needs real name (Escaping with the boat)
                    break;
                case 4: //Mercenaries & Assignment Ada
                    getstage.Add("Pueblo");
                    getstage.Add("");
                    getstage.Add("Castle Ramparts");
                    getstage.Add("Militant Grounds");
                    getstage.Add("The Docks");
                    getstage.Add("Outer Stronghold"); //Room 5
                    getstage.Add("Cliffside Encampment");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("Stairwell"); //Room 10
                    getstage.Add("Operating Room");
                    getstage.Add("Freezer");
                    getstage.Add("Research Lab");
                    getstage.Add("Com Tower");
                    getstage.Add("Waste Disposal"); //Room 15
                    getstage.Add("Basement");
                    getstage.Add("Administration Corridor");
                    break;
                case 5: //Separate Ways
                    getstage.Add("Pueblo");
                    getstage.Add("Secret Passage");
                    getstage.Add("Cemetery");
                    getstage.Add("Chruch");
                    getstage.Add("Pueblo");
                    getstage.Add("Farm"); //Room 5
                    getstage.Add("Mendez Manor");
                    getstage.Add("House of Confinement");
                    getstage.Add("Village Gate and Gondola");
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Gigante Way"); //Room 10
                    getstage.Add("Abandoned Cabin");
                    getstage.Add("Theater Hall");
                    getstage.Add("Castle Wall");
                    getstage.Add("Bedroom Garden");
                    getstage.Add("Gallery"); //Room 15
                    getstage.Add("Exhibition Chamber");
                    getstage.Add("Com Tower");
                    getstage.Add("Tunnelway");
                    getstage.Add("Shipyard");
                    getstage.Add("Shipyard Factory"); //Room 20
                    getstage.Add("Dozer Crash Site");
                    getstage.Add("Chapel and Furnace");
                    getstage.Add("Machine Room");
                    getstage.Add("Militant Camp");
                    getstage.Add("Fort"); //Room 25 //Needs real name (Where Leon's heli partner dies after)
                    getstage.Add("Cavern Entrance");
                    getstage.Add("Cavern");
                    getstage.Add("Security Checkpoint");
                    getstage.Add("Capsule Chamber");
                    getstage.Add("Construction Building"); //Room 30
                    getstage.Add("Movie");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 35
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 40
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add(""); //Room 45
                    getstage.Add("");
                    getstage.Add("");
                    getstage.Add("E3 Stage");
                    getstage.Add("E3 Stage");
                    getstage.Add("E3 Stage"); //Room 50
                    getstage.Add("");
                    getstage.Add("");
                    break;
                default:
                    getstage.Add("Unknown Location");
                    break;
            }
            return getstage.ToArray();
        }
    }
}