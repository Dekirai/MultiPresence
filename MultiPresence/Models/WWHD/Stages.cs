namespace MultiPresence.Models.WWHD
{
    public class Stages
    {
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
    }
}