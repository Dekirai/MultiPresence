namespace MultiPresence.Models.TPHD
{
    public class Stages_Old
    {
        public static async Task<string> MapName(string mapname)
        {
            string getmap;

            switch (mapname)
            {
                /*Dungeons*/
                case "D_MN01":
                    getmap = "Dungeon - Lakebed Temple";
                    break;
                case "D_MN01A":
                    getmap = "Dungeon - Lakebed Temple (Boss)";
                    break;
                case "D_MN01B":
                    getmap = "Dungeon - Lakebed Temple (Mini Boss)";
                    break;
                case "D_MN04":
                    getmap = "Dungeon - Goron Mines";
                    break;
                case "D_MN04A":
                    getmap = "Dungeon - Goron Mines (Boss)";
                    break;
                case "D_MN04B":
                    getmap = "Dungeon - Goron Mines (Mini Boss)";
                    break;
                case "D_MN05":
                    getmap = "Dungeon - Forest Temple";
                    break;
                case "D_MN05A":
                    getmap = "Dungeon - Forest Temple (Boss)";
                    break;
                case "D_MN05B":
                    getmap = "Dungeon - Forest Temple (Mini Boss)";
                    break;
                case "D_MN06":
                    getmap = "Dungeon - Temple of Time";
                    break;
                case "D_MN06A":
                    getmap = "Dungeon - Temple of Time (Boss)";
                    break;
                case "D_MN06B":
                    getmap = "Dungeon - Temple of Time (Mini Boss)";
                    break;
                case "D_MN07":
                    getmap = "Dungeon - City in the Sky";
                    break;
                case "D_MN07A":
                    getmap = "Dungeon - City in the Sky (Boss)";
                    break;
                case "D_MN07B":
                    getmap = "Dungeon - City in the Sky (Mini Boss)";
                    break;
                case "D_MN08":
                    getmap = "Dungeon - Palace of Twilight";
                    break;
                case "D_MN08A":
                    getmap = "Dungeon - Palace of Twilight (Boss)";
                    break;
                case "D_MN08B":
                    getmap = "Dungeon - Palace of Twilight (Mini Boss)";
                    break;
                case "D_MN08C":
                    getmap = "Dungeon - Palace of Twilight (Mini Boss)";
                    break;
                case "D_MN08D":
                    getmap = "Dungeon - Palace of Twilight (Boss)";
                    break;
                case "D_MN09":
                    getmap = "Dungeon - Hyrule Castle";
                    break;
                case "D_MN09A":
                    getmap = "Dungeon - Hyrule Castle (Boss)";
                    break;
                case "D_MN09B":
                    getmap = "Dungeon - Hyrule Castle (Boss)";
                    break;
                case "D_MN09C":
                    getmap = "Dungeon - Hyrule Castle (Boss)";
                    break;
                case "D_MN10":
                    getmap = "Dungeon - Arbiters Grounds";
                    break;
                case "D_MN10A":
                    getmap = "Dungeon - Arbiters Grounds (Boss)";
                    break;
                case "D_MN10B":
                    getmap = "Dungeon - Arbiters Grounds (Mini Boss)";
                    break;
                case "D_MN11":
                    getmap = "Dungeon - Snowpeak Ruins";
                    break;
                case "D_MN11A":
                    getmap = "Dungeon - Snowpeak Ruins (Boss)";
                    break;
                case "D_MN11B":
                    getmap = "Dungeon - Snowpeak Ruins (Mini Boss)";
                    break;
                /*Caves*/
                case "D_SB00":
                    getmap = "Lanayru Ice Puzzle Cave";
                    break;
                case "D_SB01":
                    getmap = "Cave of Ordeals";
                    break;
                case "D_SB02":
                    getmap = "Eldin Long Cave";
                    break;
                case "D_SB03":
                    getmap = "Lake Hylia Long Cave";
                    break;
                case "D_SB04":
                    getmap = "Eldin Goron Stockcave";
                    break;
                case "D_SB05":
                case "D_SB06":
                case "D_SB07":
                case "D_SB08":
                case "D_SB09":
                    getmap = "Grotto";
                    break;
                case "D_SB10":
                    getmap = "Faron Woods Cave";
                    break;
                /*Locations*/
                case "F_SP00":
                    getmap = "Ordon Ranch";
                    break;
                case "F_SP102":
                    getmap = "Title Screen";
                    break;
                case "F_SP103":
                    getmap = "Ordon Village";
                    break;
                case "F_SP104":
                    getmap = "Ordon Woods";
                    break;
                case "F_SP108":
                    getmap = "Faron Woods";
                    break;
                case "F_SP109":
                    getmap = "Kakariko Village";
                    break;
                case "F_SP110":
                    getmap = "Death Mountain";
                    break;
                case "F_SP111":
                    getmap = "Kakariko Graveyard";
                    break;
                case "F_SP112":
                    getmap = "Zoras River";
                    break;
                case "F_SP113":
                    getmap = "Zoras Domains";
                    break;
                case "F_SP114":
                    getmap = "Snowpeak";
                    break;
                case "F_SP115":
                    getmap = "Lake Hylia";
                    break;
                case "F_SP116":
                    getmap = "Castle Town";
                    break;
                case "F_SP117":
                    getmap = "Sacred Grove";
                    break;
                case "F_SP118":
                    getmap = "Bublin Camp";
                    break;
                case "F_SP121":
                    getmap = "Hyrule Field";
                    break;
                case "F_SP122":
                    getmap = "Outside Castle Town";
                    break;
                case "F_SP123":
                    getmap = "Bublin";
                    break;
                case "F_SP124":
                    getmap = "Gerudo Desert";
                    break;
                case "F_SP125":
                    getmap = "Mirror Chamber";
                    break;
                case "F_SP126":
                    getmap = "Upper Zoras River";
                    break;
                case "F_SP127":
                    getmap = "Fishing Pond";
                    break;
                case "F_SP128":
                    getmap = "Hidden Village";
                    break;
                case "F_SP200":
                    getmap = "Hidden Skill";
                    break;
                /*Rooms*/
                case "R_SP01":
                    getmap = "Ordon Village - Interior";
                    break;
                case "R_SP107":
                    getmap = "Hyrule Castle Sewers";
                    break;
                case "R_SP108":
                    getmap = "Faron Woods - Interior";
                    break;
                case "R_SP109":
                    getmap = "Kakariko Village - Interior";
                    break;
                case "R_SP110":
                    getmap = "Death Mountain - Interior";
                    break;
                case "R_SP116":
                    getmap = "Telmas Bar";
                    break;
                case "R_SP127":
                    getmap = "Fishing Pond";
                    break;
                case "R_SP128":
                    getmap = "Hidden Village - Interior";
                    break;
                case "R_SP160":
                    getmap = "Castle Town - Interior";
                    break;
                case "R_SP161":
                    getmap = "Star Game";
                    break;
                case "R_SP209":
                    getmap = "Kakariko Graveyard - Interior";
                    break;
                case "R_SP300":
                    getmap = "Cutscene";
                    break;
                case "R_SP301":
                    getmap = "Cutscene";
                    break;
                /*Unknown*/
                case "S_MV000":
                    getmap = "Title Screen Trailer Theatre";
                    break;

                default:
                    getmap = "Undefined Location";
                    break;
            }

            return getmap;
        }
    }
}
