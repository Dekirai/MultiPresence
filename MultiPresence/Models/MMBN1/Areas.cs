namespace MultiPresence.Models.MMBN
{
    public class Areas
    {
        public static async Task<string[]> GetArea(int area)
        {
            List<string> getarea = new List<string>();
            switch (area)
            {
                case 1:
                    getarea.Add("ACDC Town");
                    getarea.Add("School Gate");
                    getarea.Add("Lan's Living Room");
                    getarea.Add("Lan's Room");
                    getarea.Add("Mayl's House");
                    getarea.Add("Mayl's Room");
                    getarea.Add("Dex's Room");
                    getarea.Add("Yai's Room");
                    getarea.Add("Higsby's");
                    getarea.Add("ACDC Station");
                    getarea.Add("Secret Station");
                    break;

                case 0:
                    getarea.Add("Class 5-A");
                    getarea.Add("Class 5-B");
                    getarea.Add("Library");
                    getarea.Add("2F Hallway");
                    getarea.Add("Class 1-A");
                    getarea.Add("Class 1-B");
                    getarea.Add("AV Room");
                    getarea.Add("Infirmary");
                    getarea.Add("1F Hallway");
                    getarea.Add("Cross Hall");
                    getarea.Add("Storage");
                    getarea.Add("Staff Lounge");
                    getarea.Add("Lounge Hall");
                    break;

                case 2:
                    getarea.Add("Government Complex");
                    getarea.Add("Government Station");
                    getarea.Add("Waterworks Lobby");
                    getarea.Add("SciLab Lobby");
                    getarea.Add("Government Complex Hall");
                    getarea.Add("Dad's Lab");
                    getarea.Add("Waterworks Office");
                    getarea.Add("Waterworks Control Room");
                    getarea.Add("Pump Room");
                    getarea.Add("Filter Room");
                    break;

                case 3:
                    getarea.Add("Central Den");
                    getarea.Add("Den Station");
                    getarea.Add("Den Block 1");
                    getarea.Add("Den Block 2");
                    getarea.Add("Den Block 3");
                    getarea.Add("Den Block 4");
                    getarea.Add("Antique Shop");
                    getarea.Add("Summer School");
                    break;

                case 4:
                    getarea.Add("Restaurant Hall");
                    getarea.Add("Restaurant");
                    getarea.Add("Power Plant Hall");
                    getarea.Add("Power Plant");
                    getarea.Add("Power Plant Control");
                    getarea.Add("Generator Room");
                    break;

                case 5:
                    getarea.Add("WWW Base");
                    getarea.Add("Wily's Lab");
                    getarea.Add("Rocket Hangar");
                    getarea.Add("WWW Base Hall 1");
                    getarea.Add("WWW Base Hall 2");
                    getarea.Add("WWW Base Hall 3");
                    break;

                case 0x80:
                    getarea.Add("School Comp 1");
                    getarea.Add("School Comp 2");
                    getarea.Add("School Comp 3");
                    getarea.Add("School Comp 4");
                    getarea.Add("School Comp 5");
                    break;

                case 0x81:
                    getarea.Add("Oven Comp 1");
                    getarea.Add("Oven Comp 2");
                    break;

                case 0x82:
                    getarea.Add("Waterworks Comp01");
                    getarea.Add("Waterworks Comp02");
                    getarea.Add("Waterworks Comp03");
                    getarea.Add("Waterworks Comp04");
                    getarea.Add("Waterworks Comp05");
                    getarea.Add("Waterworks Comp06");
                    break;

                case 0x83:
                    getarea.Add("Traffic Light Comp 1");
                    getarea.Add("Traffic Light Comp 2");
                    getarea.Add("Traffic Light Comp 3");
                    getarea.Add("Traffic Light Comp 4");
                    getarea.Add("Traffic Light Comp 5");
                    break;

                case 0x84:
                    getarea.Add("Power Plant Comp 1");
                    getarea.Add("Power Plant Comp 2");
                    getarea.Add("Power Plant Comp 3");
                    break;

                case 0x85:
                    getarea.Add("WWW Comp 1");
                    getarea.Add("WWW Comp 2");
                    getarea.Add("WWW Comp 3");
                    getarea.Add("WWW Comp 4");
                    getarea.Add("WWW Comp 5");
                    getarea.Add("Rocket Comp");
                    break;

                case 0x88:
                    getarea.Add("Lan's PC");
                    getarea.Add("Mayl's Piano Comp");
                    getarea.Add("Yai's Portrait Comp");
                    getarea.Add("Dex's PC");
                    break;

                case 0x89:
                    getarea.Add("Dad's PC");
                    getarea.Add("Lunch Stand Comp");
                    break;

                case 0x8A:
                    getarea.Add("Antique Comp");
                    break;

                case 0x8B:
                    getarea.Add("Fish Stand Comp");
                    break;

                case 0x8C:
                    getarea.Add("Doghouse Comp");
                    getarea.Add("Servbot Comp");
                    getarea.Add("New Game Machine Comp");
                    getarea.Add("Telephone Comp");
                    getarea.Add("Car Comp");
                    getarea.Add("Waterworks Vending Machine Comp");
                    getarea.Add("TV Comp");
                    getarea.Add("Large Monitor Comp");
                    getarea.Add("Control Equipment Comp");
                    getarea.Add("SciLab Vending Machine Comp");
                    getarea.Add("Recycled PET Comp");
                    getarea.Add("Big Vase Comp");
                    getarea.Add("Blackboard Comp");
                    break;

                case 0x90:
                    getarea.Add("Internet 1");
                    getarea.Add("Internet 2");
                    getarea.Add("Internet 3");
                    getarea.Add("Internet 4");
                    getarea.Add("Undernet 1");
                    getarea.Add("Undernet 2");
                    getarea.Add("Undernet 3");
                    getarea.Add("Undernet 4");
                    getarea.Add("Undernet 5");
                    getarea.Add("Undernet 6");
                    getarea.Add("Undernet 7");
                    getarea.Add("Undernet 8");
                    getarea.Add("Undernet 9");
                    getarea.Add("Undernet 10");
                    getarea.Add("Undernet 11");
                    getarea.Add("Undernet 12");
                    break;

                default:
                    getarea.Add("Unknown Map");
                    break;
            }

            return getarea.ToArray();
        }
    }
}
