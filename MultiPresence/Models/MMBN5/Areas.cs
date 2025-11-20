namespace MultiPresence.Models.MMBN5
{
    public class Areas
    {
        public static async Task<string[]> GetArea(int area)
        {
            List<string> getarea = new List<string>();
            switch (area)
            {
                case 0:
                    getarea.Add("ACDC Town");
                    getarea.Add("Lan's House");
                    getarea.Add("Lan's Room");
                    getarea.Add("Mayl's Room");
                    getarea.Add("Dex's Room");
                    getarea.Add("Yai's Room");
                    getarea.Add("Higsby's");
                    getarea.Add("Higsby's Base");
                    break;

                case 1:
                    getarea.Add("Dad's Lab");
                    getarea.Add("SciLab Front");
                    getarea.Add("SciLab Lobby");
                    getarea.Add("OpRoom");
                    getarea.Add("Mission Control");
                    break;

                case 2:
                    getarea.Add("Oran Isle");
                    getarea.Add("Old Mine");
                    getarea.Add("Big Cave");
                    getarea.Add("Old Mine 1");
                    getarea.Add("Old Mine 2");
                    getarea.Add("Old Mine 3");
                    getarea.Add("Old Mine 4");
                    getarea.Add("Old Mine 5");
                    getarea.Add("Old Mine 6");
                    getarea.Add("Old Mine 7");
                    getarea.Add("Old Mine 8");
                    getarea.Add("Old Mine 9");
                    break;

                case 3:
                    getarea.Add("Dock");
                    getarea.Add("Hall");
                    getarea.Add("Deck");
                    getarea.Add("Engine Room");
                    getarea.Add("Fiesta Room");
                    getarea.Add("Bridge");
                    break;

                case 4:
                    getarea.Add("End City");
                    getarea.Add("Mum Room");
                    getarea.Add("Lily Room");
                    getarea.Add("Castle Keep");
                    getarea.Add("Castle Exterior");
                    break;

                case 5:
                    getarea.Add("Nebula Control");
                    getarea.Add("Factory Exterior");
                    getarea.Add("Factory Hall");
                    getarea.Add("DarkChip Factory");
                    getarea.Add("Final Boss room(Sever Room)");
                    break;

                case 0x80:
                    getarea.Add("Main Comp 1");
                    getarea.Add("Main Comp 2");
                    break;

                case 0x81:
                    getarea.Add("Drill Comp 1");
                    getarea.Add("Drill Comp 2");
                    getarea.Add("Drill Comp 3");
                    getarea.Add("Drill Comp 4");
                    break;

                case 0x82:
                    getarea.Add("Ship Comp 1");
                    getarea.Add("Ship Comp 2");
                    getarea.Add("Ship Comp 3");
                    getarea.Add("Ship Comp 4");
                    break;

                case 0x83:
                    getarea.Add("Gargoyle Comp 1");
                    getarea.Add("Gargoyle Comp 2");
                    getarea.Add("Gargoyle Comp 3");
                    getarea.Add("Gargoyle Comp 4");
                    break;

                case 0x84:
                    getarea.Add("Factory Comp 1");
                    getarea.Add("Factory Comp 2");
                    getarea.Add("Factory Comp 3");
                    getarea.Add("Factory Comp 4");
                    getarea.Add("Soul Server Comp");
                    break;

                case 0x88:
                    getarea.Add("Lan's HP");
                    getarea.Add("Mayl's HP");
                    getarea.Add("Dex's HP");
                    getarea.Add("Yai's HP");
                    getarea.Add("SciLab HP");
                    getarea.Add("Gargoyle Castle HP");
                    break;

                case 0x8A:
                    getarea.Add("ACDC Town Vision Burst");
                    getarea.Add("SciLab Vision Burst");
                    getarea.Add("Oran Isle Vision Burst");
                    break;

                case 0x8C:
                    getarea.Add("Doghouse Comp");
                    getarea.Add("Old Terminal Comp");
                    getarea.Add("Kitchen Comp");
                    getarea.Add("Electric Lock Comp");
                    getarea.Add("Radar Comp");
                    getarea.Add("Air Conditioner Comp");
                    getarea.Add("Screw Comp");
                    getarea.Add("Engine Comp");
                    getarea.Add("View Comp");
                    getarea.Add("Chip Maker Comp");
                    getarea.Add("Air Filter Comp");
                    getarea.Add("Armor Comp");
                    getarea.Add("Helmet Comp");
                    getarea.Add("Katana Comp");
                    getarea.Add("Server Comp");
                    getarea.Add("Furnace Comp");
                    break;

                case 0x8D:
                    getarea.Add("Elevator Comp");
                    getarea.Add("Crane Comp");
                    getarea.Add("Tree Comp");
                    getarea.Add("Old Comp");
                    getarea.Add("Dad's Comp");
                    getarea.Add("Sculpture Comp");
                    getarea.Add("Terminal Comp");
                    getarea.Add("NetBattle Comp");
                    getarea.Add("Wine Case Comp");
                    getarea.Add("Dumpling Comp");
                    getarea.Add("Experiment Server Comp");
                    getarea.Add("Wind God Comp");
                    getarea.Add("Pipe Comp");
                    getarea.Add("Message Comp");
                    break;

                case 0x8E:
                    getarea.Add("Squirrel Comp 1");
                    getarea.Add("Squirrel Comp 2");
                    getarea.Add("Squirrel Comp 3");
                    getarea.Add("Squirrel Comp 4");
                    getarea.Add("Squirrel Comp 5");
                    getarea.Add("Squirrel Comp 6");
                    getarea.Add("Squirrel Comp 7");
                    getarea.Add("Squirrel Comp 8");
                    getarea.Add("Squirrel Comp 9");
                    getarea.Add("Squirrel Comp 10");
                    getarea.Add("Squirrel Comp 11");
                    getarea.Add("Squirrel Comp 12");
                    getarea.Add("Squirrel Comp 13");
                    getarea.Add("Squirrel Comp 14");
                    getarea.Add("Squirrel Comp 15");
                    getarea.Add("Squirrel Comp 16");
                    break;

                case 0x86:
                    getarea.Add("ACDC Area 3");
                    getarea.Add("Oran Area 3");
                    getarea.Add("SciLab Area 3");
                    getarea.Add("End Area 2");
                    getarea.Add("Undernet 4");
                    getarea.Add("Nebula Area 1");
                    getarea.Add("Nebula Area 3");
                    getarea.Add("End Area 5");
                    getarea.Add("Nebula Area 5");
                    break;

                case 0x90:
                    getarea.Add("ACDC Area 1");
                    getarea.Add("ACDC Area 2");
                    break;

                case 0x91:
                    getarea.Add("Oran Area 1");
                    getarea.Add("Oran Area 2");
                    break;

                case 0x92:
                    getarea.Add("SciLab Area 1");
                    getarea.Add("SciLab Area 2");
                    getarea.Add("SciLab Area 4");
                    break;

                case 0x93:
                    getarea.Add("End Area 1");
                    getarea.Add("End Area 3");
                    getarea.Add("End Area 4");
                    break;

                case 0x94:
                    getarea.Add("Undernet 1");
                    getarea.Add("Undernet 2");
                    getarea.Add("Undernet 3");
                    getarea.Add("Nebula Area 2");
                    getarea.Add("Nebula Area 4");
                    getarea.Add("Nebula Area 6");
                    break;

                default:
                    getarea.Add("Unknown Map");
                    break;
            }

            return getarea.ToArray();
        }
    }
}
