namespace MultiPresence.Models.MMBN2
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
                    getarea.Add("Lan's House");
                    getarea.Add("Lan's Room");
                    getarea.Add("Mayl's House");
                    getarea.Add("Mayl's Room");
                    getarea.Add("Dex's House");
                    getarea.Add("Yai's Room");
                    getarea.Add("Yai's Bath");
                    getarea.Add("Room 5A");
                    getarea.Add("Yai's Hall");
                    break;

                case 1:
                    getarea.Add("Marine Harbor");
                    getarea.Add("Marine Station");
                    getarea.Add("Lobby");
                    getarea.Add("License Office");
                    getarea.Add("Test Room");
                    getarea.Add("Dad's Lab");
                    getarea.Add("Mother Computer");
                    break;

                case 2:
                    getarea.Add("Okuden Station");
                    getarea.Add("Camp Entrance");
                    getarea.Add("Camp Road 1");
                    getarea.Add("Camp Road 2");
                    getarea.Add("Camp");
                    getarea.Add("Okuden Dam");
                    break;

                case 3:
                    getarea.Add("Den Airport");
                    getarea.Add("Boarding (Electopia)");
                    getarea.Add("Airport Station");
                    break;

                case 4:
                    getarea.Add("Netopia Airport");
                    getarea.Add("Boarding (Netopia)");
                    break;

                case 5:
                    getarea.Add("Netopia Park");
                    getarea.Add("Netopia Town");
                    getarea.Add("Underground");
                    getarea.Add("Hotel Room");
                    getarea.Add("Jewelry Shop");
                    getarea.Add("Netopia Castle");
                    getarea.Add("Banquet Room");
                    getarea.Add("Trap Room");
                    getarea.Add("Arrow Room");
                    getarea.Add("Arch Room");
                    getarea.Add("Lower Stairs");
                    getarea.Add("Fire Room");
                    getarea.Add("Confusion Room");
                    getarea.Add("Aboveground");
                    getarea.Add("Watchtower");
                    break;

                case 6:
                    getarea.Add("Crew Room");
                    getarea.Add("Economy");
                    getarea.Add("Business");
                    getarea.Add("First Class");
                    getarea.Add("Cockpit");
                    break;

                case 7:
                    getarea.Add("Kotobuki Town");
                    getarea.Add("Apartment 1F");
                    getarea.Add("Apartment 2F");
                    getarea.Add("Apartment 8F");
                    getarea.Add("Apartment 25F");
                    getarea.Add("Apartment 27F");
                    getarea.Add("Apartment 4F");
                    getarea.Add("Apartment 9F");
                    getarea.Add("Apartment 20F");
                    getarea.Add("Apartment 23F");
                    getarea.Add("Apartment 24F");
                    getarea.Add("Apartment 30F");
                    break;

                case 8:
                    getarea.Add("Penthouse");
                    getarea.Add("Final Boss Room");
                    getarea.Add("Room 021");
                    getarea.Add("Room 082");
                    getarea.Add("Room 253");
                    getarea.Add("Room 271");
                    getarea.Add("Room 042");
                    getarea.Add("Room 093");
                    getarea.Add("Room 201");
                    getarea.Add("Room 232");
                    getarea.Add("Room 243");
                    break;

                case 0x80:
                    getarea.Add("Gas Comp 1");
                    getarea.Add("Gas Comp 2");
                    break;

                case 0x81:
                    getarea.Add("Bomb Comp 1");
                    getarea.Add("Bomb Comp 2");
                    getarea.Add("Bomb Comp 3");
                    getarea.Add("Bomb Comp 4");
                    break;

                case 0x82:
                    getarea.Add("Mother Comp 1");
                    getarea.Add("Mother Comp 2");
                    getarea.Add("Mother Comp 3");
                    getarea.Add("Mother Comp 4");
                    getarea.Add("Mother Comp 5");
                    break;

                case 0x83:
                    getarea.Add("Castle Comp 1");
                    getarea.Add("Castle Comp 2");
                    getarea.Add("Castle Comp 3");
                    getarea.Add("Castle Comp 4");
                    getarea.Add("Castle Comp 5");
                    break;

                case 0x84:
                    getarea.Add("Air Comp 1");
                    getarea.Add("Air Comp 2");
                    getarea.Add("Air Comp 3");
                    getarea.Add("Air Comp 4");
                    getarea.Add("Air Comp 5");
                    break;

                case 0x85:
                    getarea.Add("Apartment Comp 1");
                    getarea.Add("Apartment Comp 2");
                    getarea.Add("Apartment Comp 3");
                    getarea.Add("Apartment Comp 4");
                    getarea.Add("Gospel Server 1");
                    getarea.Add("Gospel Server 2");
                    break;

                case 0x88:
                    getarea.Add("Lan's PC");
                    getarea.Add("Mayl's PC");
                    getarea.Add("Dex's PC");
                    getarea.Add("Yai's PC");
                    getarea.Add("Ribitta's Van Comp");
                    getarea.Add("Raoul's Radio");
                    getarea.Add("Millions' Bag");
                    break;

                case 0x8C:
                    getarea.Add("Blackboard Comp");
                    getarea.Add("Broken Toy Comp");
                    getarea.Add("Doghouse Comp");
                    getarea.Add("Control Panel Comp");
                    getarea.Add("Piano Comp");
                    getarea.Add("Coffee Machine Comp");
                    getarea.Add("Portable Game Comp");
                    getarea.Add("Telephone Comp");
                    getarea.Add("Guardian Comp");
                    getarea.Add("Gas Stove Comp");
                    getarea.Add("Bear Comp");
                    getarea.Add("Monitor Comp");
                    getarea.Add("Wide Monitor Comp");
                    getarea.Add("Flight Board Comp");
                    getarea.Add("Gift Shop Comp");
                    getarea.Add("Bronze Statue Comp");
                    break;

                case 0x8D:
                    getarea.Add("Refrigerator Comps");
                    getarea.Add("Goddess Comp");
                    getarea.Add("Television Comp");
                    getarea.Add("Vending Machine Comp");
                    getarea.Add("Autolock Comp");
                    break;

                case 0x90:
                    getarea.Add("Den Area１");
                    getarea.Add("Den Area２");
                    getarea.Add("Den Area３");
                    getarea.Add("Square Entrance");
                    getarea.Add("The Square");
                    getarea.Add("Board Room");
                    break;

                case 0x91:
                    getarea.Add("Koto Area");
                    getarea.Add("UnderKoto");
                    getarea.Add("KotoSquare Entrance");
                    getarea.Add("Koto Square 1");
                    getarea.Add("Koto Square 2");
                    getarea.Add("Gospel HQ");
                    break;

                case 0x92:
                    getarea.Add("Yumland 1");
                    getarea.Add("Yumland 2");
                    getarea.Add("YumSquare Entrance");
                    getarea.Add("YumSquare");
                    getarea.Add("Treasure Room");
                    break;

                case 0x93:
                    getarea.Add("Netopia 1");
                    getarea.Add("Netopia 2");
                    getarea.Add("Netopia 3");
                    getarea.Add("NetSquare Entrance");
                    getarea.Add("Netopia Square");
                    break;

                case 0x94:
                    getarea.Add("Undernet 1");
                    getarea.Add("Undernet 2");
                    getarea.Add("Undernet 3");
                    getarea.Add("Undernet 4");
                    getarea.Add("Undernet 5");
                    getarea.Add("Undernet 6");
                    getarea.Add("Undernet 7");
                    getarea.Add("WWW Area 1");
                    getarea.Add("WWW Area 2");
                    getarea.Add("WWW Area 3");
                    getarea.Add("UnderSquare Entrance");
                    getarea.Add("UnderSquare");
                    getarea.Add("UnderBoard");
                    break;

                default:
                    getarea.Add("Unknown Map");
                    break;
            }
            return getarea.ToArray();
        }
    }
}
