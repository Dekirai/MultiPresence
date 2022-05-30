namespace MultiPresence.Models.WWHD
{
    public class wwhd_values
    {
        public static async Task<string> GetItemValue(string Itemname)
        {
            string item;

            switch (Itemname)
            {
                case "Telescope":
                    item = "0x20";
                    break;

                case "Wind Waker":
                    item = "0x22";
                    break;

                case "Grappling Hook":
                    item = "0x25";
                    break;

                case "Picto Box":
                    item = "0x23";
                    break;

                case "Deluxe Box":
                    item = "0x26";
                    break;

                case "Iron Boots":
                    item = "0x29";
                    break;

                case "Hookshot":
                    item = "0x2F";
                    break;

                case "Bombs":
                    item = "0x31";
                    break;

                case "Skull Hammer":
                    item = "0x33";
                    break;

                case "Hero's Bow":
                    item = "0x27";
                    break;

                case "Hero's Bow (Fire & Ice)":
                    item = "0x35";
                    break;

                case "Hero's Bow (Fire, Ice & Light)":
                    item = "0x36";
                    break;

                case "Magic Armor":
                    item = "0x2A";
                    break;

                case "Boomerang":
                    item = "0x2D";
                    break;

                case "Deku Leaf":
                    item = "0x34";
                    break;

                case "Bait Bag":
                    item = "0x2C";
                    break;

                case "Delivery Bag":
                    item = "0x30";
                    break;

                case "Spoils Bag":
                    item = "0x24";
                    break;

                case "Hero's Sword":
                    item = "0x38";
                    break;

                case "Master Sword":
                    item = "0x39";
                    break;

                case "Master Sword (half restored)":
                    item = "0x3A";
                    break;

                case "Master Sword (fully restored)":
                    item = "0x3E";
                    break;

                case "Hero's Shield":
                    item = "0x3B";
                    break;

                case "Mirror Shield":
                    item = "0x3C";
                    break;

                case "Hero's Charm":
                    item = "0x01";
                    break;

                case "Tingle Bottle":
                    item = "0x21";
                    break;

                case "Power Bracelets":
                    item = "0x28";
                    break;

                case "No Bottle":
                    item = "0xFF";
                    break;

                case "Empty":
                    item = "0x50";
                    break;

                case "Red Elixier":
                    item = "0x51";
                    break;

                case "Green Elixier":
                    item = "0x52";
                    break;

                case "Blue Elixier":
                    item = "0x53";
                    break;

                case "Soup (Half)":
                    item = "0x54";
                    break;

                case "Soup":
                    item = "0x55";
                    break;

                case "Water":
                    item = "0x56";
                    break;

                case "Fairy":
                    item = "0x57";
                    break;

                case "Pollen":
                    item = "0x58";
                    break;

                case "Magic Water":
                    item = "0x59";
                    break;

                default:
                    item = "0x20";
                    break;
            }

            return item;
        }

        public static async Task<string> GetRealName(string name)
        {
            string mapName;

            switch (name)
            {
                case "sea":
                    mapName = "Great Sea";
                    break;
                case "sea_E":
                    mapName = "Credits";
                    break;
                case "sea_T":
                    mapName = "Title screen";
                    break;
                case "Name":
                    mapName = "File select";
                    break;
                case "Ocean":
                    mapName = "Boating Course";
                    break;
                case "Abship":
                    mapName = "Submarine Interior";
                    break;
                case "ma2room":
                    mapName = "Forsaken Fortress - Interior I";
                    break;
                case "ma3room":
                    mapName = "Forsaken Fortress - Interior II";
                    break;
                case "majroom":
                    mapName = "Forsaken Fortress - Interior III";
                    break;
                case "MajyuE":
                    mapName = "Forsaken Fortress - Exterior";
                    break;
                case "Mjtower":
                    mapName = "Forsaken Fortress - Helmaroc's Room";
                    break;
                case "M2tower":
                    mapName = "Forsaken Fortress - Helmaroc's Room";
                    break;
                case "M2ganon":
                    mapName = "Forsaken Fortress - Ganondorf's lair";
                    break;
                case "LinkRM":
                    mapName = "Outset Island - Link's House";
                    break;
                case "LinkUG":
                    mapName = "Outset Island - Basement";
                    break;
                case "Siren":
                    mapName = "Tower of Gods";
                    break;
                case "SirenMB":
                    mapName = "Tower of Gods - Miniboss Room";
                    break;
                case "SirenB":
                    mapName = "Tower of Gods - Boss Room";
                    break;
                case "Obshop":
                    mapName = "Beedle's Shop Ship";
                    break;
                case "Ojhous":
                    mapName = "Outset Island - Orca's House";
                    break;
                case "Ojhous2":
                    mapName = "Outset Island - Sturgeon's House";
                    break;
                case "Omasao":
                    mapName = "Outset Island - Mesa's House";
                    break;
                case "Onobuta":
                    mapName = "Outset Island - Abe's House";
                    break;
                case "A_mori":
                    mapName = "Outset Island - Forest";
                    break;
                case "A_umikz":
                    mapName = "Pirate Ship - Deck";
                    break;
                case "Asoko":
                    mapName = "Pirate Ship - Interior";
                    break;
                case "Pnezumi":
                    mapName = "Windfall Island - Prison";
                    break;
                case "Pdrgsh":
                    mapName = "Windfall Island - Potion Shop";
                    break;
                case "Obombh":
                    mapName = "Windfall Island - Bomb Shop";
                    break;
                case "Orichh":
                    mapName = "Windfall Island - Auction House";
                    break;
                case "Opub":
                case "Opubh":
                    mapName = "Windfall Island - Cafe Bar";
                    break;
                case "Ocmera":
                    mapName = "Windfall Island - Lenzo's Studio";
                    break;
                case "Nitiyou":
                    mapName = "Windfall Island - School of Joy";
                    break;
                case "Kaisen":
                    mapName = "Windfall Island - Minigame House";
                    break;
                case "Atorizk":
                    mapName = "Dragon Roost Island - Mail Center";
                    break;
                case "Adanmae":
                    mapName = "Dragon Roost Island - Spring";
                    break;
                case "Comori":
                    mapName = "Dragon Roost Island - Komali's Room";
                    break;
                case "M_NewD2":
                    mapName = "Dragon Roost Cavern";
                    break;
                case "M_Dra09":
                    mapName = "Dragon Roost Cavern - Outside";
                    break;
                case "M_DragB":
                    mapName = "Dragon Roost Cavern - Boss Room";
                    break;
                case "Omori":
                    mapName = "Forest Haven - Interior";
                    break;
                case "Ocrogh":
                    mapName = "Forest Haven - Bomb Shop";
                    break;
                case "Otkura":
                    mapName = "Forest Haven - Behind waterfall";
                    break;
                case "Edaichi":
                    mapName = "Earth Temple - Entrance";
                    break;
                case "M_Dai":
                    mapName = "Earth Temple";
                    break;
                case "M_DaiMB":
                    mapName = "Earth Temple - Miniboss Room";
                    break;
                case "M_DaiB":
                    mapName = "Earth Temple - Boss Room";
                    break;
                case "Ekaze":
                    mapName = "Wind Temple - Entrance";
                    break;
                case "kaze":
                    mapName = "Wind Temple";
                    break;
                case "kazeMB":
                    mapName = "Wind Temple - Miniboss Room";
                    break;
                case "kazeB":
                    mapName = "Wind Temple - Boss Room";
                    break;
                case "kindan":
                    mapName = "Forbidden Woods";
                    break;
                case "kinMB":
                    mapName = "Forbidden Woods - Miniboss Room";
                    break;
                case "kinBOSS":
                    mapName = "Forbidden Woods - Boss Room";
                    break;
                case "Pjavdou":
                    mapName = "Outset Island - Jabun's Cavern";
                    break;
                case "Hyrule":
                    mapName = "Hyrule Castle - Exterior";
                    break;
                case "Hyroom":
                    mapName = "Hyrule Castle - Interior";
                    break;
                case "kenroom":
                    mapName = "Hyrule Castle - Basement";
                    break;
                case "ADMumi":
                    mapName = "Tower of Gods - Cutscene";
                    break;
                case "MiniKaz":
                    mapName = "Inside Fire Mountain";
                    break;
                case "MiniHyo":
                    mapName = "Inside Ice Ring Isle";
                    break;
                case "ITest62":
                    mapName = "Inside Ice Ring Isle - Grotto";
                    break;
                case "ITest63":
                    mapName = "Shark Island - Cave";
                    break;
                case "Cave01":
                    mapName = "Bomb Island - Cave";
                    break;
                case "Cave02":
                    mapName = "Star Island - Cave";
                    break;
                case "Cave03":
                    mapName = "Cliff Plateau Isles - Cave";
                    break;
                case "Cave04":
                    mapName = "Rock Spire Isle - Cave";
                    break;
                case "Cave05":
                    mapName = "Horsehoe Island - Cave";
                    break;
                case "Cave07":
                    mapName = "Pawprint Isle - Cave";
                    break;
                case "Cave09":
                    mapName = "Savage Labyrinth";
                    break;
                case "Cave10":
                    mapName = "Savage Labyrinth";
                    break;
                case "Cave11":
                    mapName = "Savage Labyrinth";
                    break;
                case "ShipD":
                    mapName = "Islet of Steel - Interior";
                    break;
                case "SubD42":
                    mapName = "Needle Rock Isle - Cave";
                    break;
                case "SubD43":
                    mapName = "Angular Isles - Cave";
                    break;
                case "SubD44":
                    mapName = "Stonewatcher Island - Cave";
                    break;
                case "SubD71":
                    mapName = "Bomb Island - Early Cave";
                    break;
                case "TF_01":
                    mapName = "Stonewatcher Island - Triforce Cave";
                    break;
                case "TF_02":
                    mapName = "Overlook Island - Triforce Cave";
                    break;
                case "TF_03":
                    mapName = "Bird's Peak Rock - Triforce Cave";
                    break;
                case "TF_04":
                    mapName = "Link's Oasis - Triforce Cave";
                    break;
                case "TF_06":
                    mapName = "Dragon Roost Island - Cave";
                    break;
                case "TyuTyu":
                    mapName = "Pawprint Isle - Cave";
                    break;
                case "Abesso":
                    mapName = "Link's Oasis";
                    break;
                case "Fairy01":
                    mapName = "Northern Fairy Island - Fairy";
                    break;
                case "Fairy02":
                    mapName = "Eastern Fairy Island - Fairy";
                    break;
                case "Fairy03":
                    mapName = "Western Fairy Island - Fairy";
                    break;
                case "Fairy04":
                    mapName = "Outset Island Forest - Fairy";
                    break;
                case "Fairy05":
                    mapName = "Thorned Fairy Island - Fairy";
                    break;
                case "Fairy06":
                    mapName = "Southern Fairy Island - Fairy";
                    break;
                case "WarpD":
                    mapName = "Diamond Steppe Island - Cave";
                    break;
                case "PShip":
                    mapName = "Ghost Ship";
                    break;
                case "GanonA":
                    mapName = "Ganon's Tower - Entrance";
                    break;
                case "GanonB":
                    mapName = "Ganon's Tower - DRC Trial";
                    break;
                case "GanonC":
                    mapName = "Ganon's Tower - WT Trial";
                    break;
                case "GanonD":
                    mapName = "Ganon's Tower - FW Trial";
                    break;
                case "GanonE":
                    mapName = "Ganon's Tower - ET Trial";
                    break;
                case "GanonN":
                    mapName = "Ganon's Tower - Staircase to Center";
                    break;
                case "GanonM":
                    mapName = "Ganon's Tower - Center";
                    break;
                case "GanonJ":
                    mapName = "Ganon's Tower - Maze";
                    break;
                case "GanonL":
                    mapName = "Ganon's Tower - Staircase to Ganon";
                    break;
                case "GanonK":
                    mapName = "Ganon's Tower - Ganon's Room";
                    break;
                case "GTower":
                    mapName = "Ganon's Tower - Final Fight";
                    break;
                case "Xboss0":
                    mapName = "Ganon's Tower - DRC Trial Boss";
                    break;
                case "Xboss1":
                    mapName = "Ganon's Tower - FW Trial Boss";
                    break;
                case "Xboss2":
                    mapName = "Ganon's Tower - ET Trial Boss";
                    break;
                case "Xboss3":
                    mapName = "Ganon's Tower - WT Trial Boss";
                    break;
                case "ENDumi":
                    mapName = "End Game Cutscene";
                    break;
                case "Pfigure":
                    mapName = "Nintendo Gallery";
                    break;
                case "figureA":
                    mapName = "Gallery - Great Sea Figurines";
                    break;
                case "figureB":
                    mapName = "Gallery - Windfall Island Figurines";
                    break;
                case "figureC":
                    mapName = "Gallery - Outset Island Figurines";
                    break;
                case "figureD":
                    mapName = "Gallery - Boss Figurines";
                    break;
                case "figureE":
                    mapName = "Gallery - Enemy Figurines";
                    break;
                case "figureF":
                    mapName = "Gallery - DRC Figurines";
                    break;
                case "figureG":
                    mapName = "Gallery - Forest Haven Figurines";
                    break;
                ////////UNUSED\\\\\\\\
                case "Cave06":
                    mapName = "Outset Island - Unused Cave";
                    break;
                case "DmSpot0":
                    mapName = "Outset Island - Test";
                    break;
                case "A_nami":
                    mapName = "Invisible Island";
                    break;
                case "ITest61":
                    mapName = "Bomb Island - Cave Test";
                    break;
                case "Ebesso":
                    mapName = "Unused Island";
                    break;
                case "kazan":
                    mapName = "Unused Fire Mountain";
                    break;
                case "Mukao":
                    mapName = "Unused Temple Island";
                    break;
                case "Cave08":
                    mapName = "Unused Room (Room + Spawn: 3)";
                    break;
                case "Msmoke":
                    mapName = "Msmoke - Test Room";
                    break;
                case "PShip2":
                    mapName = "Submarine - Unused Room";
                    break;
                case "PShip3":
                    mapName = "Submarine - Unused Room 2";
                    break;
                case "SubD51":
                    mapName = "Bomb Island - Early Cave";
                    break;
                case "tincle":
                    mapName = "Tingle's Paint Room";
                    break;
                //////UNUSED END\\\\\\
                default:
                    mapName = "Stage not defined";
                    break;
            }

            return mapName;
        }

        public static async Task<string[]> GetRoomName(string Roomid)
        {
            List<string> getroom = new List<string>();

            switch (Roomid)
            {
                case "0":
                    getroom.Add("Great Sea"); //sea
                    break;

                case "1":
                    getroom.Add("Forsaken Fortress"); //sea
                    break;

                case "2":
                    getroom.Add("Star Island"); //sea
                    break;

                case "3":
                    getroom.Add("Northern Fairy Island"); //sea
                    break;

                case "4":
                    getroom.Add("Gale Isle"); //sea
                    break;

                case "5":
                    getroom.Add("Crescent Moon Island"); //sea
                    break;

                case "6":
                    getroom.Add("Seven-Star Isles"); //sea
                    break;

                case "7":
                    getroom.Add("Overlook Island"); //sea
                    break;

                case "8":
                    getroom.Add("Four-Eye Reef"); //sea
                    break;

                case "9":
                    getroom.Add("Mother and Child Isles"); //sea
                    break;

                case "10":
                    getroom.Add("Spectacle Island"); //sea
                    break;

                case "11":
                    getroom.Add("Windfall Island"); //sea
                    break;

                case "12":
                    getroom.Add("Pawprint Isle"); //sea
                    break;

                case "13":
                    getroom.Add("Dragon Roost Island"); //sea
                    break;

                case "14":
                    getroom.Add("Flight Control Platform"); //sea
                    break;

                case "15":
                    getroom.Add("Western Fairy Island"); //sea
                    break;

                case "16":
                    getroom.Add("Rock Spire Isle"); //sea
                    break;

                case "17":
                    getroom.Add("Tingle Island"); //sea
                    break;

                case "18":
                    getroom.Add("North Triangle Isle"); //sea
                    break;

                case "19":
                    getroom.Add("Eastern Fairy Isle"); //sea
                    break;

                case "20":
                    getroom.Add("Fire Mountain"); //sea
                    break;

                case "21":
                    getroom.Add("Star Belt Archipelago"); //sea
                    break;

                case "22":
                    getroom.Add("Three-Eye Isle"); //sea
                    break;

                case "23":
                    getroom.Add("Greatfish Isle"); //sea
                    break;

                case "24":
                    getroom.Add("Cyclops Reef"); //sea
                    break;

                case "25":
                    getroom.Add("Six-Eye Reef"); //sea
                    break;

                case "26":
                    getroom.Add("Tower of Gods"); //sea
                    break;

                case "27":
                    getroom.Add("East Triangle Isle"); //sea
                    break;

                case "28":
                    getroom.Add("Thorned Faily Island"); //sea
                    break;

                case "29":
                    getroom.Add("Needle Rock Isle"); //sea
                    break;

                case "30":
                    getroom.Add("Islet of Steel"); //sea
                    break;

                case "31":
                    getroom.Add("Stonewatcher Island"); //sea
                    break;

                case "32":
                    getroom.Add("South Triangle Isle"); //sea
                    break;

                case "33":
                    getroom.Add("Link's Oasis"); //sea
                    break;

                case "34":
                    getroom.Add("Bomb Island"); //sea
                    break;

                case "35":
                    getroom.Add("Bird's Peak Rock"); //sea
                    break;

                case "36":
                    getroom.Add("Diamond Steppe Island"); //sea
                    break;

                case "37":
                    getroom.Add("Five-Eye Reef"); //sea
                    break;

                case "38":
                    getroom.Add("Shark Island"); //sea
                    break;

                case "39":
                    getroom.Add("Southern Fairy Island"); //sea
                    break;

                case "40":
                    getroom.Add("Ice Ring Isle"); //sea
                    break;

                case "41":
                    getroom.Add("Forest Haven"); //sea
                    break;

                case "42":
                    getroom.Add("Cliff Plateu Isles"); //sea
                    break;

                case "43":
                    getroom.Add("Horsehoe Isle"); //sea
                    break;

                case "44":
                    getroom.Add("Outset Island"); //sea
                    break;

                case "45":
                    getroom.Add("Headstone Island"); //sea
                    break;

                case "46":
                    getroom.Add("Two-Eye Reef"); //sea
                    break;

                case "47":
                    getroom.Add("Angular Isles"); //sea
                    break;

                case "48":
                    getroom.Add("Boating Course"); //sea
                    break;

                case "49":
                    getroom.Add("Five-Star Isles"); //sea
                    break;

                default:
                    getroom.Add("Room not defined yet");
                    break;
            }
            return getroom.ToArray();
        }

        public static async Task<string[]> RPC_GetRealName(string name)
        {
            List<string> mapName = new List<string>();

            switch (name)
            {
                case "sea":
                    mapName.Add("Great Sea");
                    mapName.Add("greatsea");
                    break;
                case "sea_E":
                    mapName.Add("Credits");
                    mapName.Add("greatsea");
                    break;
                case "sea_T":
                    mapName.Add("Title screen");
                    mapName.Add("titlescreen");
                    break;
                case "Name":
                    mapName.Add("File select");
                    mapName.Add("fileselect");
                    break;
                case "Ocean":
                    mapName.Add("Boating Course");
                    mapName.Add("greatsea");
                    break;
                case "Abship":
                    mapName.Add("Submarine Interior");
                    mapName.Add("submarine");
                    break;
                case "ma2room":
                    mapName.Add("Forsaken Fortress - Interior I");
                    mapName.Add("greatsea");
                    break;
                case "ma3room":
                    mapName.Add("Forsaken Fortress - Interior II");
                    mapName.Add("greatsea");
                    break;
                case "majroom":
                    mapName.Add("Forsaken Fortress - Interior III");
                    mapName.Add("greatsea");
                    break;
                case "MajyuE":
                    mapName.Add("Forsaken Fortress - Exterior");
                    mapName.Add("greatsea");
                    break;
                case "Mjtower":
                    mapName.Add("Forsaken Fortress - Helmaroc's Room");
                    mapName.Add("greatsea");
                    break;
                case "M2tower":
                    mapName.Add("Forsaken Fortress - Helmaroc's Room");
                    mapName.Add("greatsea");
                    break;
                case "M2ganon":
                    mapName.Add("Forsaken Fortress - Ganondorf's lair");
                    mapName.Add("greatsea");
                    break;
                case "LinkRM":
                    mapName.Add("Outset Island - Link's House");
                    mapName.Add("house_link");
                    break;
                case "LinkUG":
                    mapName.Add("Outset Island - Basement");
                    mapName.Add("greatsea");
                    break;
                case "Siren":
                    mapName.Add("Tower of Gods");
                    mapName.Add("greatsea");
                    break;
                case "SirenMB":
                    mapName.Add("Tower of Gods - Miniboss Room");
                    mapName.Add("greatsea");
                    break;
                case "SirenB":
                    mapName.Add("Tower of Gods - Boss Room");
                    mapName.Add("greatsea");
                    break;
                case "Obshop":
                    mapName.Add("Beedle's Shop Ship");
                    mapName.Add("beedleshop");
                    break;
                case "Ojhous":
                    mapName.Add("Outset Island - Orca's House");
                    mapName.Add("house_orca");
                    break;
                case "Ojhous2":
                    mapName.Add("Outset Island - Sturgeon's House");
                    mapName.Add("house_sturgeon");
                    break;
                case "Omasao":
                    mapName.Add("Outset Island - Mesa's House");
                    mapName.Add("house_mesa");
                    break;
                case "Onobuta":
                    mapName.Add("Outset Island - Abe's House");
                    mapName.Add("house_abe");
                    break;
                case "A_mori":
                    mapName.Add("Outset Island - Forest");
                    mapName.Add("outset_forest");
                    break;
                case "A_umikz":
                    mapName.Add("Pirate Ship - Deck");
                    mapName.Add("pirateship_deck");
                    break;
                case "Asoko":
                    mapName.Add("Pirate Ship - Interior");
                    mapName.Add("pirateship_interior");
                    break;
                case "Pnezumi":
                    mapName.Add("Windfall Island - Prison");
                    mapName.Add("greatsea");
                    break;
                case "Pdrgsh":
                    mapName.Add("Windfall Island - Potion Shop");
                    mapName.Add("greatsea");
                    break;
                case "Obombh":
                    mapName.Add("Windfall Island - Bomb Shop");
                    mapName.Add("greatsea");
                    break;
                case "Orichh":
                    mapName.Add("Windfall Island - Auction House");
                    mapName.Add("greatsea");
                    break;
                case "Opub":
                case "Opubh":
                    mapName.Add("Windfall Island - Cafe Bar");
                    mapName.Add("greatsea");
                    break;
                case "Ocmera":
                    mapName.Add("Windfall Island - Lenzo's Studio");
                    mapName.Add("greatsea");
                    break;
                case "Nitiyou":
                    mapName.Add("Windfall Island - School of Joy");
                    mapName.Add("greatsea");
                    break;
                case "Kaisen":
                    mapName.Add("Windfall Island - Minigame House");
                    mapName.Add("greatsea");
                    break;
                case "Atorizk":
                    mapName.Add("Dragon Roost Island - Mail Center");
                    mapName.Add("dri_mailcenter");
                    break;
                case "Adanmae":
                    mapName.Add("Dragon Roost Island - Spring");
                    mapName.Add("dri_spring");
                    break;
                case "Comori":
                    mapName.Add("Dragon Roost Island - Komali's Room");
                    mapName.Add("dri_komali");
                    break;
                case "M_NewD2":
                    mapName.Add("Dragon Roost Cavern");
                    mapName.Add("drc");
                    break;
                case "M_Dra09":
                    mapName.Add("Dragon Roost Cavern - Outside");
                    mapName.Add("drc_outside");
                    break;
                case "M_DragB":
                    mapName.Add("Dragon Roost Cavern - Boss Room");
                    mapName.Add("drc_boss");
                    break;
                case "Omori":
                    mapName.Add("Forest Haven - Interior");
                    mapName.Add("greatsea");
                    break;
                case "Ocrogh":
                    mapName.Add("Forest Haven - Bomb Shop");
                    mapName.Add("greatsea");
                    break;
                case "Otkura":
                    mapName.Add("Forest Haven - Behind waterfall");
                    mapName.Add("greatsea");
                    break;
                case "Edaichi":
                    mapName.Add("Earth Temple - Entrance");
                    mapName.Add("greatsea");
                    break;
                case "M_Dai":
                    mapName.Add("Earth Temple");
                    mapName.Add("greatsea");
                    break;
                case "M_DaiMB":
                    mapName.Add("Earth Temple - Miniboss Room");
                    mapName.Add("greatsea");
                    break;
                case "M_DaiB":
                    mapName.Add("Earth Temple - Boss Room");
                    mapName.Add("greatsea");
                    break;
                case "Ekaze":
                    mapName.Add("Wind Temple - Entrance");
                    mapName.Add("greatsea");
                    break;
                case "kaze":
                    mapName.Add("Wind Temple");
                    mapName.Add("greatsea");
                    break;
                case "kazeMB":
                    mapName.Add("Wind Temple - Miniboss Room");
                    mapName.Add("greatsea");
                    break;
                case "kazeB":
                    mapName.Add("Wind Temple - Boss Room");
                    mapName.Add("greatsea");
                    break;
                case "kindan":
                    mapName.Add("Forbidden Woods");
                    mapName.Add("greatsea");
                    break;
                case "kinMB":
                    mapName.Add("Forbidden Woods - Miniboss Room");
                    mapName.Add("greatsea");
                    break;
                case "kinBOSS":
                    mapName.Add("Forbidden Woods - Boss Room");
                    mapName.Add("greatsea");
                    break;
                case "Pjavdou":
                    mapName.Add("Outset Island - Jabun's Cavern");
                    mapName.Add("greatsea");
                    break;
                case "Hyrule":
                    mapName.Add("Hyrule Castle - Exterior");
                    mapName.Add("greatsea");
                    break;
                case "Hyroom":
                    mapName.Add("Hyrule Castle - Interior");
                    mapName.Add("greatsea");
                    break;
                case "kenroom":
                    mapName.Add("Hyrule Castle - Basement");
                    mapName.Add("greatsea");
                    break;
                case "ADMumi":
                    mapName.Add("Tower of Gods - Cutscene");
                    mapName.Add("greatsea");
                    break;
                case "MiniKaz":
                    mapName.Add("Inside Fire Mountain");
                    mapName.Add("greatsea");
                    break;
                case "MiniHyo":
                    mapName.Add("Inside Ice Ring Isle");
                    mapName.Add("greatsea");
                    break;
                case "ITest62":
                    mapName.Add("Inside Ice Ring Isle - Grotto");
                    mapName.Add("greatsea");
                    break;
                case "ITest63":
                    mapName.Add("Shark Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave01":
                    mapName.Add("Bomb Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave02":
                    mapName.Add("Star Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave03":
                    mapName.Add("Cliff Plateau Isles - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave04":
                    mapName.Add("Rock Spire Isle - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave05":
                    mapName.Add("Horsehoe Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave07":
                    mapName.Add("Pawprint Isle - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Cave09":
                    mapName.Add("Savage Labyrinth");
                    mapName.Add("savagelabyrinth");
                    break;
                case "Cave10":
                    mapName.Add("Savage Labyrinth");
                    mapName.Add("savagelabyrinth");
                    break;
                case "Cave11":
                    mapName.Add("Savage Labyrinth");
                    mapName.Add("savagelabyrinth");
                    break;
                case "ShipD":
                    mapName.Add("Islet of Steel - Interior");
                    mapName.Add("greatsea");
                    break;
                case "SubD42":
                    mapName.Add("Needle Rock Isle - Cave");
                    mapName.Add("greatsea");
                    break;
                case "SubD43":
                    mapName.Add("Angular Isles - Cave");
                    mapName.Add("greatsea");
                    break;
                case "SubD44":
                    mapName.Add("Stonewatcher Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "SubD71":
                    mapName.Add("Bomb Island - Early Cave");
                    mapName.Add("greatsea");
                    break;
                case "TF_01":
                    mapName.Add("Stonewatcher Island - Triforce Cave");
                    mapName.Add("greatsea");
                    break;
                case "TF_02":
                    mapName.Add("Overlook Island - Triforce Cave");
                    mapName.Add("greatsea");
                    break;
                case "TF_03":
                    mapName.Add("Bird's Peak Rock - Triforce Cave");
                    mapName.Add("greatsea");
                    break;
                case "TF_04":
                    mapName.Add("Link's Oasis - Triforce Cave");
                    mapName.Add("oasis_cave");
                    break;
                case "TF_06":
                    mapName.Add("Dragon Roost Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "TyuTyu":
                    mapName.Add("Pawprint Isle - Cave");
                    mapName.Add("greatsea");
                    break;
                case "Abesso":
                    mapName.Add("Link's Oasis");
                    mapName.Add("oasis_inside");
                    break;
                case "Fairy01":
                    mapName.Add("Northern Fairy Island - Fairy");
                    mapName.Add("greatsea");
                    break;
                case "Fairy02":
                    mapName.Add("Eastern Fairy Island - Fairy");
                    mapName.Add("greatsea");
                    break;
                case "Fairy03":
                    mapName.Add("Western Fairy Island - Fairy");
                    mapName.Add("greatsea");
                    break;
                case "Fairy04":
                    mapName.Add("Outset Island Forest - Fairy");
                    mapName.Add("greatsea");
                    break;
                case "Fairy05":
                    mapName.Add("Thorned Fairy Island - Fairy");
                    mapName.Add("greatsea");
                    break;
                case "Fairy06":
                    mapName.Add("Southern Fairy Island - Fairy");
                    mapName.Add("greatsea");
                    break;
                case "WarpD":
                    mapName.Add("Diamond Steppe Island - Cave");
                    mapName.Add("greatsea");
                    break;
                case "PShip":
                    mapName.Add("Ghost Ship");
                    mapName.Add("greatsea");
                    break;
                case "GanonA":
                    mapName.Add("Ganon's Tower - Entrance");
                    mapName.Add("greatsea");
                    break;
                case "GanonB":
                    mapName.Add("Ganon's Tower - DRC Trial");
                    mapName.Add("greatsea");
                    break;
                case "GanonC":
                    mapName.Add("Ganon's Tower - WT Trial");
                    mapName.Add("greatsea");
                    break;
                case "GanonD":
                    mapName.Add("Ganon's Tower - FW Trial");
                    mapName.Add("greatsea");
                    break;
                case "GanonE":
                    mapName.Add("Ganon's Tower - ET Trial");
                    mapName.Add("greatsea");
                    break;
                case "GanonN":
                    mapName.Add("Ganon's Tower - Staircase to Center");
                    mapName.Add("greatsea");
                    break;
                case "GanonM":
                    mapName.Add("Ganon's Tower - Center");
                    mapName.Add("greatsea");
                    break;
                case "GanonJ":
                    mapName.Add("Ganon's Tower - Maze");
                    mapName.Add("greatsea");
                    break;
                case "GanonL":
                    mapName.Add("Ganon's Tower - Staircase to Ganon");
                    mapName.Add("greatsea");
                    break;
                case "GanonK":
                    mapName.Add("Ganon's Tower - Ganon's Room");
                    mapName.Add("greatsea");
                    break;
                case "GTower":
                    mapName.Add("Ganon's Tower - Final Fight");
                    mapName.Add("greatsea");
                    break;
                case "Xboss0":
                    mapName.Add("Ganon's Tower - DRC Trial Boss");
                    mapName.Add("greatsea");
                    break;
                case "Xboss1":
                    mapName.Add("Ganon's Tower - FW Trial Boss");
                    mapName.Add("greatsea");
                    break;
                case "Xboss2":
                    mapName.Add("Ganon's Tower - ET Trial Boss");
                    mapName.Add("greatsea");
                    break;
                case "Xboss3":
                    mapName.Add("Ganon's Tower - WT Trial Boss");
                    mapName.Add("greatsea");
                    break;
                case "ENDumi":
                    mapName.Add("End Game Cutscene");
                    mapName.Add("greatsea");
                    break;
                case "Pfigure":
                    mapName.Add("Nintendo Gallery");
                    mapName.Add("greatsea");
                    break;
                case "figureA":
                    mapName.Add("Gallery - Great Sea Figurines");
                    mapName.Add("greatsea");
                    break;
                case "figureB":
                    mapName.Add("Gallery - Windfall Island Figurines");
                    mapName.Add("greatsea");
                    break;
                case "figureC":
                    mapName.Add("Gallery - Outset Island Figurines");
                    mapName.Add("greatsea");
                    break;
                case "figureD":
                    mapName.Add("Gallery - Boss Figurines");
                    mapName.Add("greatsea");
                    break;
                case "figureE":
                    mapName.Add("Gallery - Enemy Figurines");
                    mapName.Add("greatsea");
                    break;
                case "figureF":
                    mapName.Add("Gallery - DRC Figurines");
                    mapName.Add("greatsea");
                    break;
                case "figureG":
                    mapName.Add("Gallery - Forest Haven Figurines");
                    mapName.Add("greatsea");
                    break;
                ////////UNUSED\\\\\\\\
                case "Cave06":
                    mapName.Add("Outset Island - Unused Cave");
                    mapName.Add("greatsea");
                    break;
                case "DmSpot0":
                    mapName.Add("Outset Island - Test");
                    mapName.Add("greatsea");
                    break;
                case "A_nami":
                    mapName.Add("Invisible Island");
                    mapName.Add("greatsea");
                    break;
                case "ITest61":
                    mapName.Add("Bomb Island - Cave Test");
                    mapName.Add("greatsea");
                    break;
                case "Ebesso":
                    mapName.Add("Unused Island");
                    mapName.Add("greatsea");
                    break;
                case "kazan":
                    mapName.Add("Unused Fire Mountain");
                    mapName.Add("greatsea");
                    break;
                case "Mukao":
                    mapName.Add("Unused Temple Island");
                    mapName.Add("greatsea");
                    break;
                case "Cave08":
                    mapName.Add("Unused Room (Room + Spawn: 3)");
                    mapName.Add("greatsea");
                    break;
                case "Msmoke":
                    mapName.Add("Msmoke - Test Room");
                    mapName.Add("greatsea");
                    break;
                case "PShip2":
                    mapName.Add("Submarine - Unused Room");
                    mapName.Add("greatsea");
                    break;
                case "PShip3":
                    mapName.Add("Submarine - Unused Room 2");
                    mapName.Add("greatsea");
                    break;
                case "SubD51":
                    mapName.Add("Bomb Island - Early Cave");
                    mapName.Add("greatsea");
                    break;
                case "tincle":
                    mapName.Add("Tingle's Paint Room");
                    mapName.Add("greatsea");
                    break;
                //////UNUSED END\\\\\\
                default:
                    mapName.Add("Stage not defined");
                    mapName.Add("greatsea");
                    break;
            }

            return mapName.ToArray();
        }

    }
}