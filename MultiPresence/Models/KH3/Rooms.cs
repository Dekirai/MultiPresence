namespace MultiPresence.Models.KHIII
{
    public class Rooms
    {
        public static async Task<string> GetRoom(string room)
        {
            string getroom;

            switch (room)
            {
                case "gm_01":
                    getroom = "Gummi - Starlight Way";
                    break;
                case "gm_02":
                    getroom = "Gummi - Misty Stream";
                    break;
                case "gm_03":
                    getroom = "Gummi - Eclipse Galaxy";
                    break;
                case "gm_50":
                    getroom = "Cutscene";
                    break;
                case "wm_01":
                    getroom = "World Map";
                    break;
                case "tt_01":
                    getroom = "The Neighborhood";
                    break;
                case "tt_40":
                    getroom = "Bistro";
                    break;
                case "tt_50":
                    getroom = "Computer Laboratory";
                    break;
                case "he_01":
                    getroom = "Realm of the Gods";
                    break;
                case "he_02":
                    getroom = "Mount Olympus";
                    break;
                case "he_03":
                    getroom = "Thebes";
                    break;
                case "he_04":
                    getroom = "Thebes";
                    break;
                case "he_05":
                    getroom = "Titans Battle Arena";
                    break;
                case "he_06":
                    getroom = "Titans Battle Tornado";
                    break;
                case "he_50":
                    getroom = "Cutscene";
                    break;
                case "he_52":
                    getroom = "Cutscene";
                    break;
                case "bt_01":
                    getroom = "The Stairway to the Sky";
                    break;
                case "bt_02":
                    getroom = "The Stairway to the Sky - Deformed";
                    break;
                case "bt_03":
                    getroom = "The Stairway to the Sky - Underwater";
                    break;
                case "bt_04":
                    getroom = "The Stairway to the Sky - Destroyed";
                    break;
                case "bt_07":
                    getroom = "Breezy Quarter";
                    break;
                case "bt_08":
                    getroom = "The Stairway to the Sky";
                    break;
                case "bt_50":
                    getroom = "Chess Room";
                    break;
                case "bt_51":
                    getroom = "Intro";
                    break;
                case "bx_01":
                    getroom = "The Bridge";
                    break;
                case "bx_02":
                    getroom = "The City";
                    break;
                case "bx_03":
                    getroom = "Hiro's Garage";
                    break;
                case "ca_01":
                    getroom = "Port Royal";
                    break;
                case "ca_02":
                    getroom = "The High Seas";
                    break;
                case "ca_03":
                    getroom = "Under the sea";
                    break;
                case "ca_04":
                    getroom = "Davy Jones' Locker";
                    break;
                case "ca_05":
                    getroom = "Over the Edge";
                    break;
                case "ca_50":
                    getroom = "Cutscene";
                    break;
                case "di_50":
                    getroom = "Cutscene";
                    break;
                case "dp_01":
                case "dp_50":
                    getroom = "Land of Departure";
                    break;
                case "dw_21":
                case "dw_22":
                case "dw_71":
                    getroom = "Dark World";
                    break;
                case "ew_01":
                case "ew_02":
                case "ew_03":
                case "ew_04":
                case "ew_21":
                case "ew_22":
                case "ew_23":
                case "ew_24":
                case "ew_25":
                case "ew_26":
                case "ew_27":
                case "ew_28":
                    getroom = "The Final World";
                    break;
                case "ex_21":
                    getroom = "Battlegate 1 - Olympus";
                    break;
                case "ex_22":
                    getroom = "Battlegate 2 - Olympus";
                    break;
                case "ex_23":
                    getroom = "Battlegate 3 - Twilight Town";
                    break;
                case "ex_24":
                    getroom = "Battlegate 4 - Toy Box";
                    break;
                case "ex_25":
                    getroom = "Battlegate 5 - Toy Box";
                    break;
                case "ex_26":
                    getroom = "Battlegate 6 - Kingdom of Corona";
                    break;
                case "ex_27":
                    getroom = "Battlegate 7 - Kingdom of Corona";
                    break;
                case "ex_28":
                case "ex_30":
                case "ex_32":
                case "ex_36":
                case "ex_40":
                    getroom = "Unused Battlegate";
                    break;
                case "ex_29":
                    getroom = "Battlegate 8 - Monstropolis";
                    break;
                case "ex_31":
                    getroom = "Battlegate 9 - Arendelle";
                    break;
                case "ex_33":
                    getroom = "Battlegate 10 - Caribbean";
                    break;
                case "ex_34":
                    getroom = "Battlegate 11 - San Fransokyo";
                    break;
                case "ex_35":
                    getroom = "Battlegate 12 - San Fransokyo";
                    break;
                case "ex_37":
                    getroom = "Battlegate 13 - Keyblade Graveyard";
                    break;
                case "ex_38":
                    getroom = "Battlegate 14 - Keyblade Graveyard";
                    break;
                case "ex_39":
                    getroom = "Battlegate 0 - Keyblade Graveyard";
                    break;
                case "fz_01":
                    getroom = "The North Mountain";
                    break;
                case "fz_02":
                    getroom = "The Labyrinth of Ice";
                    break;
                case "fz_03":
                    getroom = "Trinity Sled Minigame";
                    break;
                case "fz_04":
                    getroom = "???";
                    break;
                case "fz_05":
                    getroom = "???";
                    break;
                case "fz_06":
                    getroom = "Sköl Fight";
                    break;
                case "kg_01":
                case "kg_02":
                case "kg_03":
                case "kg_04":
                case "kg_05":
                case "kg_06":
                case "kg_07":
                case "kg_08":
                case "kg_50":
                case "kg_51":
                case "kg_52":
                    getroom = "???";
                    break;
                case "mi_01":
                    getroom = "Monsters. Inc.";
                    break;
                case "mi_02":
                    getroom = "The Factory";
                    break;
                case "mi_03":
                    getroom = "The Powerplant";
                    break;
                case "mi_04":
                    getroom = "The Door Vault";
                    break;
                case "mi_50":
                    getroom = "Outside the Factory";
                    break;
                case "po_01":
                case "po_02":
                case "po_03":
                case "po_04":
                    getroom = "???";
                    break;
                case "ra_01":
                    getroom = "The Forest";
                    break;
                case "ra_02":
                    getroom = "The Kingdom";
                    break;
                case "ra_50":
                    getroom = "Cutscene";
                    break;
                case "rg_01":
                    getroom = "Merlin's House";
                    break;
                case "rg_03":
                    getroom = "Garden of Assemblage";
                    break;
                case "rg_10":
                    getroom = "Data Battle - Xehanort";
                    break;
                case "rg_11":
                    getroom = "Data Battle - Ansem";
                    break;
                case "rg_12":
                    getroom = "Data Battle - Xemnas";
                    break;
                case "rg_13":
                    getroom = "Data Battle - Xigbar";
                    break;
                case "rg_14":
                    getroom = "Data Battle - Luxord";
                    break;
                case "rg_15":
                    getroom = "Data Battle - Larxene";
                    break;
                case "rg_16":
                    getroom = "Data Battle - Marluxia";
                    break;
                case "rg_17":
                    getroom = "Data Battle - Saix";
                    break;
                case "rg_18":
                    getroom = "Data Battle - Terranort";
                    break;
                case "rg_19":
                    getroom = "Data Battle - Riku";
                    break;
                case "rg_20":
                    getroom = "Data Battle - Vanitas";
                    break;
                case "rg_21":
                    getroom = "Data Battle - Young Xehanort";
                    break;
                case "rg_22":
                    getroom = "Data Battle - Xion";
                    break;
                case "rg_50":
                    getroom = "???";
                    break;
                case "rg_51":
                    getroom = "???";
                    break;
                case "sf_50":
                    getroom = "???";
                    break;
                case "sp_01":
                    getroom = "???";
                    break;
                case "ss_01":
                    getroom = "Yozora Fight";
                    break;
                case "ss_50":
                    getroom = "Cutscene";
                    break;
                case "ss_51":
                    getroom = "Cutscene";
                    break;
                case "ts_01":
                    getroom = "Andy's House";
                    break;
                case "ts_02":
                    getroom = "Galaxy Toys";
                    break;
                case "ts_03":
                    getroom = "Verum Rex Minigame";
                    break;
                case "ts_04":
                    getroom = "UFO Battle Arena";
                    break;
                case "yt_50":
                    getroom = "Cutscene";
                    break;

                default:
                    getroom = "Unknown";
                    break;
            }
            return getroom;
        }
    }
}