namespace MultiPresence.Models.RE
{
    public class Stages
    {
        internal static async Task<(string Floor, string Room)> GetLocation(int stage, int room, int? camera)
        {
            // Several PC room files cover two locations. The background camera identifies the current one.
            string? location = (stage, room, camera) switch
            {
                (1, 0x03, 3 or >= 11 and <= 18) => "Mansion 1F:Northern Corridor",
                (1, 0x03, 7) => "Mansion B1:Tea Room",
                (1, 0x03, >= 0 and <= 2 or >= 4 and <= 10) => "Mansion 1F:Tea Room",
                (1, 0x09, >= 0 and <= 11 or >= 20 and <= 24) => "Mansion 1F:Winding Corridor",
                (1, 0x09, >= 12 and <= 19 or >= 25 and <= 40) => "Mansion 1F:Bathroom",
                (1, 0x0B, >= 0 and <= 8 or 13 or 14) => "Mansion 1F:Graveyard",
                (1, 0x0B, >= 9 and <= 12 or >= 15 and <= 23) => "Mansion B1:Crypt",
                (1, 0x11, >= 0 and <= 5 or 8) => "Mansion 1F:Large Art Room",
                (1, 0x11, 6 or 7 or >= 9 and <= 11) => "Mansion 1F:Mirror Room",
                (2, 0x0C, >= 0 and <= 2) => "Mansion 1F:Secret Passage",
                (2, 0x0C, >= 3 and <= 14) => "Mansion 2F:Sliding Trap Room",
                (4, 0x0D, >= 0 and <= 7) => "Aqua Ring B1:Aqua Ring Entry",
                (4, 0x0D, >= 8 and <= 11) => "Aqua Ring B1/B2:Aqua Ring Exit",
                (5, 0x07, >= 0 and <= 4) => "Laboratory B3:Surgeon's Room",
                (5, 0x07, >= 5 and <= 14) => "Laboratory B3:Morgue",
                _ => null
            };

            if (location == null)
            {
                var rooms = await GetStage(stage);
                if ((uint)room >= (uint)rooms.Length) return ("", "Unknown Room");
                location = rooms[room];
            }

            var parts = location.Split(':', 2);
            return parts.Length == 2 ? (parts[0], parts[1]) : ("", "Unknown Room");
        }

        public static async Task<string[]> GetStage(int stage)
        {
            List<string> getstage = new List<string>();

            switch (stage)
            {
                case 0:
                    getstage.Add("In Main Menu: ");
                    break;
                case 1:
                    getstage.Add("Mansion 1F:Medical Storage Room");
                    getstage.Add("Mansion 1F/2F:West Wing Inner Stairway");
                    getstage.Add("Mansion 1F:Small Storeroom");
                    getstage.Add("Mansion 1F:Tea Room / Northern Corridor");
                    getstage.Add("");
                    getstage.Add("Mansion 1F:Dining Room"); //Room 5
                    getstage.Add("Mansion 1F/2F:Main Hall");
                    getstage.Add("Mansion 1F:Art Room");
                    getstage.Add("Mansion 1F:L Corridor");
                    getstage.Add("Mansion 1F:Winding Corridor / Bathroom");
                    getstage.Add("Mansion 1F:Dark Corridor"); //Room 10
                    getstage.Add("Mansion 1F/B1:Graveyard / Crypt");
                    getstage.Add("Mansion 1F:Greenhouse");
                    getstage.Add("Mansion 1F:Tiger Statue Room");
                    getstage.Add("Mansion 1F:Researcher's Bedroom");
                    getstage.Add("Mansion 1F:Bar"); //Room 15
                    getstage.Add("");
                    getstage.Add("Mansion 1F:Large Art Room / Mirror Room");
                    getstage.Add("Mansion 1F/2F:West Wing Outer Stairway");
                    getstage.Add("");
                    getstage.Add("Mansion 1F:Outside Boiler"); //Room 20
                    getstage.Add("Mansion 1F:Marble Room");
                    getstage.Add("Mansion 1F:Living Room");
                    getstage.Add("Mansion 1F:Large Gallery");
                    getstage.Add("Mansion 1F:East Wing Storeroom");
                    getstage.Add("Mansion 1F:Courtyard Study"); //Room 25
                    getstage.Add("Mansion 1F:Roofed Passage");
                    getstage.Add("Mansion 1F:Garden Shed");
                    getstage.Add("Mansion 1F:Hidden Wardrobe");
                    break;
                case 2:
                    getstage.Add("Mansion:Kitchen Elevator");
                    getstage.Add("");
                    getstage.Add("Mansion 2F:Dining Room");
                    getstage.Add("Mansion 2F:Outside Balcony");
                    getstage.Add("Mansion 2F:C Corridor");
                    getstage.Add("Mansion 2F:Armor Room"); //Room 5
                    getstage.Add("Mansion 2F:Small Library");
                    getstage.Add("Mansion 1F/2F:East Wing Stairway");
                    getstage.Add("Mansion 2F:Deer Room");
                    getstage.Add("Mansion 2F:Bedroom");
                    getstage.Add("Mansion 2F:Fish Tank Room"); //Room 10
                    getstage.Add("Mansion 2F:Resting Room");
                    getstage.Add("Mansion 1F/2F:Sliding Trap Room / Secret Passage");
                    getstage.Add("Mansion 2F:Pillar Room");
                    getstage.Add("Mansion 2F:Attic Entry");
                    getstage.Add("Mansion 2F:Small Dining Room"); //Room 15
                    getstage.Add("Mansion 2F:Attic");
                    getstage.Add("");
                    getstage.Add("Mansion 2F:Outside Patio");
                    getstage.Add("Mansion 2F:Elevator Corridor");
                    getstage.Add("Mansion 2F:Mirror Corridor"); //Room 20
                    getstage.Add("Mansion 2F:Trophy Room");
                    getstage.Add("Mansion 2F/3F:Library");
                    getstage.Add("Mansion 2F/3F:Death Passage");
                    getstage.Add("");
                    getstage.Add("Mansion 2F:Materials Room"); //Room 25
                    getstage.Add("Mansion B1:Concrete Passage 1");
                    getstage.Add("Mansion B1:Concrete Passage 2");
                    getstage.Add("Mansion B1:Kitchen");
                    break;
                case 3:
                    getstage.Add("Courtyard 1F:Main Garden");
                    getstage.Add("Courtyard 1F:Water Pool");
                    getstage.Add("Courtyard 1F:Falls Area");
                    getstage.Add("Heliport:Landing Point");
                    getstage.Add("Courtyard 1F:Zigzag Path");
                    getstage.Add("Courtyard 1F:Fountain"); //Room 5
                    getstage.Add("Courtyard B1:Underground Statue Room");
                    getstage.Add("Courtyard B1:Mining Area");
                    getstage.Add("Courtyard B1:Forked Passage");
                    getstage.Add("Courtyard B1:Generator Room");
                    getstage.Add("Courtyard B1:Enrico Room"); //Room 10
                    getstage.Add("Courtyard B1:Boulder Passage 1");
                    getstage.Add("Courtyard B1:Spider Room");
                    getstage.Add("Courtyard B1:Straight Passage");
                    getstage.Add("");
                    getstage.Add("Courtyard B1:Boulder Passage 2"); //Room 15
                    getstage.Add("");
                    getstage.Add("Courtyard 1F:Muddy Path");
                    getstage.Add("Courtyard 1F:Small Headstone Area");
                    getstage.Add("Courtyard 1F:Forest Pathway");
                    getstage.Add("Courtyard 1F:Cabin"); //Room 20
                    getstage.Add("Altar B1/B2:Jessica's Tomb");
                    getstage.Add("Courtyard B2:Garbage Compactor Room");
                    getstage.Add("Courtyard B2:Winding Underground Passage");
                    getstage.Add("Courtyard B2:Underground Storage Room");
                    getstage.Add("Courtyard B2:Lisa's Room"); //Room 25
                    getstage.Add("Altar B1:Stairway Passage");
                    break;
                case 4:
                    getstage.Add("Residence 1F:Entry Corridor");
                    getstage.Add("Residence 1F:Room 001");
                    getstage.Add("Residence 1F:Room 001 Bathroom");
                    getstage.Add("Residence 1F:Storeroom");
                    getstage.Add("Residence 1F:Recreation Room");
                    getstage.Add("Residence 1F:Central Corridor"); //Room 5
                    getstage.Add("Residence 1F:Room 002");
                    getstage.Add("Residence 1F:Room 002 Bathroom");
                    getstage.Add("Residence 1F:Gallery");
                    getstage.Add("Residence 1F:Medical Room");
                    getstage.Add("Residence 1F:Room 003"); //Room 10
                    getstage.Add("Residence 1F:Room 003 Bathroom");
                    getstage.Add("Residence 1F:Plant 42 Room");
                    getstage.Add("Aqua Ring B1/B2:Aqua Ring Entry / Exit");
                    getstage.Add("Aqua Ring B1:Aqua Ring Walkway");
                    getstage.Add("Aqua Ring B1:Guard Room"); //Room 15
                    getstage.Add("Aqua Ring B2:Aqua Ring Storage");
                    getstage.Add("Aqua Ring B1/B2:Control Room");
                    getstage.Add("Aqua Ring B2:Aqua Ring Water Tank");
                    break;
                case 5:
                    getstage.Add("Laboratory B1:Laboratory Entrance");
                    getstage.Add("Laboratory B1:Passage to Heliport");
                    getstage.Add("Laboratory B2:Item Box Room");
                    getstage.Add("Laboratory B2/B3:Laboratory Stairway");
                    getstage.Add("Laboratory B2:Visual Data Room");
                    getstage.Add("Laboratory B3:O Room"); //Room 5
                    getstage.Add("Laboratory B3:Operating Room");
                    getstage.Add("Laboratory B3:Morgue / Surgeon's Room");
                    getstage.Add("Laboratory B3:Pass Code Corridor");
                    getstage.Add("Laboratory B3:X-Ray Room A");
                    getstage.Add("Laboratory B3:X-Ray Room B"); //Room 10
                    getstage.Add("Laboratory B3:Cell Entrance");
                    getstage.Add("Laboratory B3:Elevator Entry");
                    getstage.Add("Heliport:Heliport Elevator");
                    getstage.Add("Laboratory B3:Laboratory Lounge Room");
                    getstage.Add("Laboratory B3:Power Maze A"); //Room 15
                    getstage.Add("Laboratory B3:Power Maze B");
                    getstage.Add("Laboratory B3:Power Control Room");
                    getstage.Add("Laboratory B3:Cell");
                    getstage.Add("Laboratory B4:Main Laboratory");
                    getstage.Add("Laboratory B4:Main Laboratory Entrance"); //Room 20
                    getstage.Add("Laboratory B3/B4:Main Elevator");
                    break;
                default:
                    getstage.Add("Unknown Location");
                    break;
            }
            return getstage.ToArray();
        }
    }
}
