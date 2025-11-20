namespace MultiPresence.Models.MMBN3
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
                    getarea.Add("ACDC Station");
                    getarea.Add("Lan's LivingRoom");
                    getarea.Add("Lan's Room");
                    getarea.Add("Mayl's LivingRoom");
                    getarea.Add("Mayl's Room");
                    getarea.Add("Dex's Room");
                    getarea.Add("Yai's Room");
                    getarea.Add("Higsby's");
                    break;

                case 1:
                    getarea.Add("Class 5-A");
                    getarea.Add("Class 5-B");
                    getarea.Add("Class Hall");
                    getarea.Add("Cross Hall");
                    getarea.Add("Staff Lounge");
                    getarea.Add("Principal's Office");
                    getarea.Add("Lounge Hall");
                    break;

                case 2:
                    getarea.Add("SciLab Station");
                    getarea.Add("SciLab Lobby");
                    getarea.Add("Virus Lab");
                    getarea.Add("Dad's Lab");
                    getarea.Add("Archives");
                    break;

                case 3:
                    getarea.Add("Yoka Station");
                    getarea.Add("Front of Zoo");
                    getarea.Add("Hotel Front");
                    getarea.Add("Hotel Lobby");
                    getarea.Add("Hall");
                    getarea.Add("Guest Room");
                    getarea.Add("Outdoor Bath");
                    getarea.Add("Zoo 1");
                    getarea.Add("Zoo 2");
                    getarea.Add("Secret Cave");
                    break;

                case 4:
                    getarea.Add("Beach Station");
                    getarea.Add("Beach Street");
                    getarea.Add("TV Station Lobby");
                    getarea.Add("TV Station Hall 1");
                    getarea.Add("TV Studio");
                    getarea.Add("TV Station Hall 2");
                    getarea.Add("Editing Room");
                    break;

                case 5:
                    getarea.Add("Hades Isle");
                    getarea.Add("Hades Mountain");
                    getarea.Add("Four Hades");
                    getarea.Add("Eternal Hades");
                    break;

                case 6:
                    getarea.Add("Shoreline");
                    getarea.Add("Hospital Lobby");
                    getarea.Add("Hospital 2F");
                    getarea.Add("Hospital Room");
                    getarea.Add("Hospital 3F");
                    getarea.Add("Basement");
                    break;

                case 7:
                    getarea.Add("Castle Wily");
                    getarea.Add("Wily Lab");
                    getarea.Add("Monitor Room");
                    getarea.Add("Lab Hall");
                    getarea.Add("Sever Room");
                    break;

                case 0x80:
                    getarea.Add("Principal's Comp 1");
                    getarea.Add("Principal's Comp 2");
                    break;

                case 0x81:
                    getarea.Add("Zoo Comp 1");
                    getarea.Add("Zoo Comp 2");
                    getarea.Add("Zoo Comp 3");
                    getarea.Add("Zoo Comp 4");
                    break;

                case 0x82:
                    getarea.Add("Hospital Comp 1");
                    getarea.Add("Hospital Comp 2");
                    getarea.Add("Hospital Comp 3");
                    getarea.Add("Hospital Comp 4");
                    getarea.Add("Hospital Comp 5");
                    break;

                case 0x83:
                    getarea.Add("WWW Comp 1");
                    getarea.Add("WWW Comp 2");
                    getarea.Add("WWW Comp 3");
                    getarea.Add("WWW Comp 4");
                    getarea.Add("Alpha");
                    break;

                case 0x88:
                    getarea.Add("Lan's HP");
                    getarea.Add("Mayl's HP");
                    getarea.Add("Dex's HP");
                    getarea.Add("Yai's HP");
                    getarea.Add("Tamako's HP");
                    break;

                case 0x8A:
                    getarea.Add("EduComputer");
                    getarea.Add("Lion Comp");
                    getarea.Add("Demon Comp");
                    getarea.Add("Editing Comp");
                    getarea.Add("Monitor Room Comp");
                    break;

                case 0x8C:
                    getarea.Add("Doghouse Comp");
                    getarea.Add("Blackboard Comp");
                    getarea.Add("Vending Comp(SciLab)");
                    getarea.Add("Computer Comp");
                    getarea.Add("Board Comp");
                    getarea.Add("School Server Comp");
                    getarea.Add("Relay Comp");
                    getarea.Add("NetBattle Comp");
                    getarea.Add("TV Board Comp");
                    getarea.Add("Phone Comp");
                    getarea.Add("TV Comp");
                    getarea.Add("Bed Comp");
                    getarea.Add("Vending Comp (Hospital)");
                    getarea.Add("Ticket Comp");
                    getarea.Add("Tank Comp");
                    getarea.Add("Old TV Comp");
                    break;

                case 0x8D:
                    getarea.Add("Armor Comp");
                    getarea.Add("Sign Comp");
                    getarea.Add("Alarm Comp");
                    getarea.Add("DoorSensor Comp");
                    getarea.Add("Wall Comp");
                    break;

                case 0x90:
                    getarea.Add("ACDC 1");
                    getarea.Add("ACDC 2");
                    getarea.Add("ACDC 3");
                    getarea.Add("ACDC Square");
                    break;

                case 0x91:
                    getarea.Add("SciLab 1");
                    getarea.Add("SciLab 2");
                    getarea.Add("SciLab Square");
                    break;

                case 0x92:
                    getarea.Add("Yoka 1");
                    getarea.Add("Yoka 2");
                    getarea.Add("Yoka Square");
                    break;

                case 0x93:
                    getarea.Add("Beach 1");
                    getarea.Add("Beach 2");
                    getarea.Add("Beach Square");
                    getarea.Add("Hades Isle (Cyberspace)");
                    break;

                case 0x94:
                    getarea.Add("Undernet 1");
                    getarea.Add("Undernet 2");
                    getarea.Add("Undernet 3");
                    getarea.Add("Undernet 4");
                    getarea.Add("Undernet 5");
                    getarea.Add("Undernet 6");
                    getarea.Add("Undernet 7");
                    getarea.Add("Under Square");
                    break;

                case 0x95:
                    getarea.Add("Secret 1");
                    getarea.Add("Secret 2");
                    getarea.Add("Secret 3");
                    break;

                default:
                    getarea.Add("Unknown Map");
                    break;
            }

            return getarea.ToArray();
        }
    }
}
