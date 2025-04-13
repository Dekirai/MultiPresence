namespace MultiPresence.Models.VS
{
    public class Stages
    {
        public static async Task<string> GetStages(int stage)
        {
            string getstage;

            switch (stage)
            {
                case 0:
                    getstage = "Mad Forest";
                    break;
                case 1:
                    getstage = "Moongolow";
                    break;
                case 3:
                    getstage = "Inlaid Library";
                    break;
                case 7:
                    getstage = "Green Acres";
                    break;
                case 8:
                    getstage = "Dairy Plant";
                    break;
                case 9:
                    getstage = "Il Molise";
                    break;
                case 10:
                    getstage = "Gallo Tower";
                    break;
                case 11:
                    getstage = "The Bone Zone";
                    break;
                case 12:
                    getstage = "Holy Forbidden";
                    break;
                case 13:
                    getstage = "Cappella Magna";
                    break;
                case 14:
                    getstage = "Boss Rash";
                    break;
                case 15:
                    getstage = "Eudaimonia M.";
                    break;
                case 17:
                    getstage = "Tiny Bridge";
                    break;
                case 20:
                    getstage = "Mt.Moonspell";
                    break;
                case 21:
                    getstage = "Bat Country";
                    break;
                case 22:
                    getstage = "Astral Stair";
                    break;
                case 23:
                    getstage = "Lake Foscari";
                    break;
                case 24:
                    getstage = "Abyss Foscari";
                    break;
                case 25:
                    getstage = "Whiteout";
                    break;
                case 26:
                    getstage = "Polus Replica";
                    break;
                case 27:
                    getstage = "Space 54";
                    break;
                case 28:
                    getstage = "Neo Galuga";
                    break;
                case 29:
                    getstage = "Hectic Highway";
                    break;
                case 30:
                    getstage = "Room 1665";
                    break;
                case 31:
                    getstage = "Laborratory";
                    break;
                case 33:
                    getstage = "Carlo Cart";
                    break;
                case 1000:
                    getstage = "Planar Bazaar";
                    break;
                case 1001:
                    getstage = "Castle Grounds";
                    break;
                case 1002:
                    getstage = "Shrine Valley";
                    break;
                case 1003:
                    getstage = "Forbidden Peaks";
                    break;
                case 1004:
                    getstage = "Hidden Cave";
                    break;
                case 1005:
                    getstage = "Frozen Lake";
                    break;
                case 1006:
                    getstage = "Tanuki Forest";
                    break;
                case 1007:
                    getstage = "Mt. Moonspell";
                    break;
                case 1011:
                    getstage = "Old Mad Forest";
                    break;
                case 1012:
                    getstage = "Rough Awakening";
                    break;
                case 1013:
                    getstage = "Risky Woods";
                    break;
                case 1014:
                    getstage = "Rotten Orchard";
                    break;
                case 1015:
                    getstage = "Garlic Paradise";
                    break;
                case 1016:
                    getstage = "Retirement";
                    break;
                case 1021:
                    getstage = "Deep Lore";
                    break;
                case 1022:
                    getstage = "Inlaid Library";
                    break;
                case 1023:
                    getstage = "Mad Forest";
                    break;
                case 1024:
                    getstage = "Gallo Tower";
                    break;
                case 1025:
                    getstage = "Astral Stair";
                    break;
                case 1026:
                    getstage = "Cappella Magna";
                    break;
                case 1031:
                    getstage = "Meeting Called";
                    break;
                case 1032:
                    getstage = "Suspect Security";
                    break;
                case 1033:
                    getstage = "Ejected";
                    break;
                case 1034:
                    getstage = "Don't Get Ghosted";
                    break;
                case 1035:
                    getstage = "Space Rocks";
                    break;
                case 1036:
                    getstage = "Emergency Meating";
                    break;
                case 1041:
                    getstage = "Attack Aggressively";
                    break;
                case 1042:
                    getstage = "Bullet Biters";
                    break;
                case 1043:
                    getstage = "Neo City Chaos";
                    break;
                case 1044:
                    getstage = "Operation: Gun City";
                    break;
                case 1045:
                    getstage = "Run & Gun";
                    break;
                case 1046:
                    getstage = "Alien Revengeance";
                    break;
                case 1047:
                    getstage = "Unused Stage";
                    break;
                case 1048:
                    getstage = "Ode to Castlevania";
                    break;
                case 1052:
                    getstage = "Ice Age";
                    break;
                case 1053:
                    getstage = "Ice Cream";
                    break;
                case 1054:
                    getstage = "A Fiery Apparition";
                    break;
                case 1055:
                    getstage = "A Pure Heart";
                    break;
                case 1056:
                    getstage = "The Rebirth, The Return";
                    break;
                case 1058:
                    getstage = "The World Eater";
                    break;
                case 1059:
                    getstage = "The Guardian of the Lake";
                    break;
                case 1060:
                    getstage = "The Sly Swashbuckler";
                    break;
                case 1061:
                    getstage = "The Undefeated Champion";
                    break;
                case 1062:
                    getstage = "The Star Pupil";
                    break;
                case 1063:
                    getstage = "Tides of the Foscari";
                    break;

                default:
                    getstage = "Unknown";
                    break;
            }
            return getstage;
        }
    }
}