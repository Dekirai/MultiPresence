namespace MultiPresence.Models.RE
{
    public class Stages
    {
        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0:
                    getstage.Add("In Main Menu: ");
                    break;
                case 1:
                    getstage.Add("Mansion 1F:Medical Storeroom");
                    getstage.Add("Mansion 2F:West Wing Stairway");
                    getstage.Add("Mansion 1F:Small Storeroom");
                    getstage.Add("Mansion 1F:First Encounter");
                    getstage.Add("");
                    getstage.Add("Mansion 1F:Dining Room"); //Room 5 
                    getstage.Add("Mansion 1F:Main Hall");
                    getstage.Add("");
                    getstage.Add("Mansion 1F:L Corridor");
                    getstage.Add("Mansion 1F:Winding Corridor");
                    getstage.Add("Mansion 1F:Gallery Corridor"); //Room 10
                    getstage.Add("Mansion 1F:Tomb");
                    getstage.Add("Mansion 1F:Greenhouse");
                    getstage.Add("Mansion 1F:Tiger Statue");
                    getstage.Add("Mansion 1F:Bedroom");
                    getstage.Add("Mansion 1F:Piano"); //Room 15
                    getstage.Add("");
                    getstage.Add("Mansion 1F:Big Mirror Room");
                    getstage.Add("Mansion 1F:After First Encounter");
                    getstage.Add("");
                    getstage.Add("Mansion 1F:Outside"); //Room 20
                    getstage.Add("Mansion 1F:Marble Room");
                    getstage.Add("Mansion 1F:Living Room");
                    getstage.Add("Mansion 1F:Gallery Puzzle");
                    getstage.Add("Mansion 1F:Storeroom");
                    getstage.Add("Mansion 1F:Spencer Cabinet"); //Room 25
                    getstage.Add("Mansion 1F:Outside Passage");
                    getstage.Add("Mansion 1F:Shed");
                    getstage.Add("Mansion 1F:Closet");
                    break;
                case 2:
                    getstage.Add("Mansion:Elevator");
                    getstage.Add("");
                    getstage.Add("Mansion 2F:Blue Gem Statue");
                    getstage.Add("Mansion 2F:Outside Balcony");
                    getstage.Add("Mansion 2F:C Corridor");
                    getstage.Add("Mansion 2F:Armor Room"); //Room 5
                    getstage.Add("Mansion 2F:Library");
                    getstage.Add("Mansion 1F:East Wing Stairway");
                    getstage.Add("Mansion 2F:Deer Room");
                    getstage.Add("Mansion 2F:Bedroom");
                    getstage.Add("Mansion 2F:Fishtank"); //Room 10
                    getstage.Add("Mansion 2F:Fireplace");
                    getstage.Add("Mansion 2F:Moving Walls");
                    getstage.Add("Mansion 2F:Pillar Room");
                    getstage.Add("Mansion 2F:Attic");
                    getstage.Add("Mansion 2F:Dining Room"); //Room 15
                    getstage.Add("Mansion 2F:Attic");
                    getstage.Add("");
                    getstage.Add("Mansion 2F:Outside Patio");
                    getstage.Add("Mansion 2F:Elevator Corridor");
                    getstage.Add("Mansion 2F:Arrow Object"); //Room 20
                    getstage.Add("Mansion 2F:Eagle Room");
                    getstage.Add("Mansion 3F:Library");
                    getstage.Add("Mansion 2F:Fake Key Trap");
                    getstage.Add("");
                    getstage.Add("Mansion 2F:Materials Room"); //Room 25
                    getstage.Add("Mansion B1:Cellar");
                    getstage.Add("Mansion B1:Cellar");
                    getstage.Add("Mansion 2F:Kitchen");
                    break;
                case 3:
                    getstage.Add("Courtyard:Garden");
                    getstage.Add("Courtyard:Pool");
                    getstage.Add("Courtyard:Waterfall");
                    getstage.Add("Courtyard:Roof");
                    getstage.Add("Courtyard:To Residence");
                    getstage.Add("Courtyard:Lab Entrance"); //Room 5
                    getstage.Add("Courtyard B1:Statue");
                    getstage.Add("Courtyard B1:Cave Entrance");
                    getstage.Add("Courtyard B1:To the Elevator");
                    getstage.Add("Courtyard B1:Elevator");
                    getstage.Add("Courtyard B1:Enrico"); //Room 10
                    getstage.Add("Courtyard B1:Boulder");
                    getstage.Add("Courtyard B1:Spider");
                    getstage.Add("Courtyard B1:After Spider");
                    getstage.Add("");
                    getstage.Add("Courtyard B1:Boulder+"); //Room 15
                    getstage.Add("");
                    getstage.Add("Courtyard:Muddy Path");
                    getstage.Add("Courtyard:Headstone");
                    getstage.Add("Courtyard:Pathway to Cabin");
                    getstage.Add("Courtyard:Cabin"); //Room 20
                    getstage.Add("Alter B1:Lisa");
                    getstage.Add("Courtyard B2:Start");
                    getstage.Add("Courtyard B2:Lisa");
                    getstage.Add("Courtyard B2:Crate with Flamethrower");
                    getstage.Add("Courtyard B2:Lisa's Room"); //Room 25
                    getstage.Add("Altar B1:Start");
                    break;
                case 4:
                    getstage.Add("Residence:Entrance");
                    getstage.Add("Residence:Room 001");
                    getstage.Add("Residence:Room 001 - Bathroom");
                    getstage.Add("Residence:Typewriter Room");
                    getstage.Add("Residence:Pools");
                    getstage.Add("Residence:Hall"); //Room 5
                    getstage.Add("Residence:Room 002");
                    getstage.Add("Residence:Room 002 - Bathroom");
                    getstage.Add("Residence:Gallery");
                    getstage.Add("Residence:VJolt Lab");
                    getstage.Add("Residence:Room 003"); //Room 10
                    getstage.Add("Residence:Room 003 - Bathroom");
                    getstage.Add("Residence:Plant Fight");
                    getstage.Add("Aqua Ring B1:Entrance");
                    getstage.Add("Aqua Ring B1:Sharks");
                    getstage.Add("Aqua Ring B1:Plant Roots"); //Room 15
                    getstage.Add("Aqua Ring B2:After Power Room");
                    getstage.Add("Aqua Ring B2:Power Room");
                    getstage.Add("Aqua Ring B2:Frying Sharks");
                    break;
                case 5:
                    getstage.Add("Laboratory B1:Entrance");
                    getstage.Add("Laboratory B1:To the Roof");
                    getstage.Add("Laboratory B2:Entrance");
                    getstage.Add("Laboratory B2:Hall");
                    getstage.Add("Laboratory B2:Projector Room");
                    getstage.Add("Laboratory B3:Entrance"); //Room 5
                    getstage.Add("Laboratory B3:Morgue - Computer");
                    getstage.Add("Laboratory B3:Morgue - Power Key Door");
                    getstage.Add("Laboratory B3:MO Disc Doors");
                    getstage.Add("Laboratory B3:X-Ray Room");
                    getstage.Add("Laboratory B3:Refulling Device"); //Room 10
                    getstage.Add("Laboratory B3:Holding Cell");
                    getstage.Add("Laboratory B3:Elevator");
                    getstage.Add("Roof:Elevator");
                    getstage.Add("Laboratory B3:Elevator Save Room");
                    getstage.Add("Laboratory B3:Fuel Supply"); //Room 15
                    getstage.Add("Laboratory B3:Before Elevator Switch");
                    getstage.Add("Laboratory B3:Elevator Switch");
                    getstage.Add("Laboratory B3:Inside Holding Cell");
                    getstage.Add("Laboratory B3:Tyrant");
                    getstage.Add("Laboratory B3:After Elevator"); //Room 20
                    getstage.Add("Laboratory B3:Elevator");
                    break;
                default:
                    getstage.Add("Unknown Location");
                    break;
            }
            return getstage.ToArray();
        }
    }
}