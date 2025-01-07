namespace MultiPresence.Models.TPHD
{
    public class Stages
    {
        public static string MapName(string mapname)
        {
            string getmap;

            switch (mapname)
            {
                /*Dungeons*/
                case "D_MN01-R00":
                    getmap = "Lakebed Temple - Entrance";
                    break;
                case "D_MN01-R01":
                    getmap = "Lakebed Temple - Stalactite Room";
                    break;
                case "D_MN01-R02":
                    getmap = "Lakebed Temple - Central Room (Outside)";
                    break;
                case "D_MN01-R03":
                    getmap = "Lakebed Temple - Central Room";
                    break;
                case "D_MN01-R05":
                    getmap = "Lakebed Temple - Before Boss Key";
                    break;
                case "D_MN01-R06":
                    getmap = "Lakebed Temple - Boss Key";
                    break;
                case "D_MN01-R07":
                    getmap = "Lakebed Temple - Right Wing Upper";
                    break;
                case "D_MN01-R08":
                    getmap = "Lakebed Temple - Right Wing Lower";
                    break;
                case "D_MN01-R09":
                    getmap = "Lakebed Temple - Before Mini Boss";
                    break;
                case "D_MN01-R10":
                    getmap = "Lakebed Temple - Right Wing Water Supply";
                    break;
                case "D_MN01-R11":
                    getmap = "Lakebed Temple - Left Wing Upper";
                    break;
                case "D_MN01-R12":
                    getmap = "Lakebed Temple - Left Wing Lower";
                    break;
                case "D_MN01-R13":
                    getmap = "Lakebed Temple - Left Wing Water Supply";
                    break;
                case "D_MN01A-R50":
                    getmap = "Lakebed Temple - Boss";
                    break;
                case "D_MN01B-R51":
                    getmap = "Lakebed Temple - Mini Boss";
                    break;
                case "D_MN04-R01":
                    getmap = "Goron Mines - Entrance";
                    break;
                case "D_MN04-R03":
                    getmap = "Goron Mines - Magnet Room";
                    break;
                case "D_MN04-R04":
                    getmap = "Goron Mines - Roll Clipping";
                    break;
                case "D_MN04-R05":
                    getmap = "Goron Mines - Before 1st Elder";
                    break;
                case "D_MN04-R06":
                    getmap = "Goron Mines - Clawshot Switch";
                    break;
                case "D_MN04-R07":
                    getmap = "Goron Mines - Outside";
                    break;
                case "D_MN04-R09":
                    getmap = "Goron Mines - Before Mini Boss";
                    break;
                case "D_MN04-R11":
                    getmap = "Goron Mines - Bow";
                    break;
                case "D_MN04-R12":
                    getmap = "Goron Mines - Before Boss";
                    break;
                case "D_MN04-R13":
                    getmap = "Goron Mines - Bow-Magnet Shortcut Room";
                    break;
                case "D_MN04-R14":
                    getmap = "Goron Mines - 1st Elder";
                    break;
                case "D_MN04-R16":
                    getmap = "Goron Mines - 3rd Elder";
                    break;
                case "D_MN04-R17":
                    getmap = "Goron Mines - 2nd Elder";
                    break;
                case "D_MN04A-R50":
                    getmap = "Goron Mines - Boss";
                    break;
                case "D_MN04B-R51":
                    getmap = "Goron Mines - Mini Boss";
                    break;
                case "D_MN05-R00":
                    getmap = "Forest Temple - Main Room";
                    break;
                case "D_MN05-R01":
                    getmap = "Forest Temple - Right Wing";
                    break;
                case "D_MN05-R02":
                    getmap = "Forest Temple - 2nd Monkey";
                    break;
                case "D_MN05-R03":
                    getmap = "Forest Temple - Left Wing";
                    break;
                case "D_MN05-R04":
                    getmap = "Forest Temple - Outside";
                    break;
                case "D_MN05-R05":
                    getmap = "Forest Temple - 3rd Monkey";
                    break;
                case "D_MN05-R07":
                    getmap = "Forest Temple - 4th Monkey";
                    break;
                case "D_MN05-R09":
                    getmap = "Forest Temple - North Wing";
                    break;
                case "D_MN05-R10":
                    getmap = "Forest Temple - Final Monkey";
                    break;
                case "D_MN05-R11":
                    getmap = "Forest Temple - 6th Monkey";
                    break;
                case "D_MN05-R12":
                    getmap = "Forest Temple - Before Boss";
                    break;
                case "D_MN05-R19":
                    getmap = "Forest Temple - 7th Monkey";
                    break;
                case "D_MN05-R22":
                    getmap = "Forest Temple - Entrance";
                    break;
                case "D_MN05A-R50":
                    getmap = "Forest Temple - Boss";
                    break;
                case "D_MN05B-R51":
                    getmap = "Forest Temple - Mini Boss";
                    break;
                case "D_MN06-R00":
                    getmap = "Temple of Time - Entrance";
                    break;
                case "D_MN06-R01":
                    getmap = "Temple of Time - Staircase";
                    break;
                case "D_MN06-R02":
                    getmap = "Temple of Time - Turning Platform";
                    break;
                case "D_MN06-R03":
                    getmap = "Temple of Time - Statue Throws";
                    break;
                case "D_MN06-R04":
                    getmap = "Temple of Time - Second Staircase";
                    break;
                case "D_MN06-R05":
                    getmap = "Temple of Time - Scale Room";
                    break;
                case "D_MN06-R06":
                    getmap = "Temple of Time - Boss Key";
                    break;
                case "D_MN06-R07":
                    getmap = "Temple of Time - Third Staircase";
                    break;
                case "D_MN06-R08":
                    getmap = "Temple of Time - Before Boss";
                    break;
                case "D_MN06A-R50":
                    getmap = "Temple of Time - Boss";
                    break;
                case "D_MN06B-R51":
                    getmap = "Temple of Time - Mini Boss";
                    break;
                case "D_MN07-R00":
                    getmap = "City in the Sky - Entrance";
                    break;
                case "D_MN07-R01":
                    getmap = "City in the Sky - Before Main Room";
                    break;
                case "D_MN07-R02":
                    getmap = "City in the Sky - Main Room";
                    break;
                case "D_MN07-R03":
                    getmap = "City in the Sky - Right Wing Outside";
                    break;
                case "D_MN07-R04":
                    getmap = "City in the Sky - Right Wing Inside";
                    break;
                case "D_MN07-R05":
                    getmap = "City in the Sky - Before Mini Boss";
                    break;
                case "D_MN07-R06":
                    getmap = "City in the Sky - Left Wing Outside";
                    break;
                case "D_MN07-R07":
                    getmap = "City in the Sky - Right Wing Inside";
                    break;
                case "D_MN07-R08":
                    getmap = "City in the Sky - Right Wing Inside";
                    break;
                case "D_MN07-R10":
                    getmap = "City in the Sky - Left Wing Inside";
                    break;
                case "D_MN07-R11":
                    getmap = "City in the Sky - Big Baba Room";
                    break;
                case "D_MN07-R12":
                    getmap = "City in the Sky - After Big Baba Outside";
                    break;
                case "D_MN07-R13":
                    getmap = "City in the Sky - Before Boss Key Outside";
                    break;
                case "D_MN07-R14":
                    getmap = "City in the Sky - North Wing Outside";
                    break;
                case "D_MN07-R15":
                    getmap = "City in the Sky - North Wing Inside";
                    break;
                case "D_MN07-R16":
                    getmap = "City in the Sky - Oocca Shop";
                    break;
                case "D_MN07A-R50":
                    getmap = "City in the Sky - Boss";
                    break;
                case "D_MN07B-R51":
                    getmap = "City in the Sky - Mini Boss";
                    break;
                case "D_MN08-R00":
                    getmap = "Palace of Twilight - Entrance";
                    break;
                case "D_MN08-R01":
                    getmap = "Palace of Twilight - Left Wing";
                    break;
                case "D_MN08-R02":
                    getmap = "Palace of Twilight - Left Wing";
                    break;
                case "D_MN08-R04":
                    getmap = "Palace of Twilight - Right Wing";
                    break;
                case "D_MN08-R05":
                    getmap = "Palace of Twilight - Right Wing";
                    break;
                case "D_MN08-R07":
                    getmap = "Palace of Twilight - Double Sol";
                    break;
                case "D_MN08-R08":
                    getmap = "Palace of Twilight - Early Platform";
                    break;
                case "D_MN08-R09":
                    getmap = "Palace of Twilight - Messengers before Zant";
                    break;
                case "D_MN08-R10":
                    getmap = "Palace of Twilight - Beta Zant Room";
                    break;
                case "D_MN08-R11":
                    getmap = "Palace of Twilight - Boss Key";
                    break;
                case "D_MN08A-R10":
                    getmap = "Palace of Twilight - Zant Main Room";
                    break;
                case "D_MN08B-R51":
                    getmap = "Palace of Twilight - Phantom Zant";
                    break;
                case "D_MN08C-R52":
                    getmap = "Palace of Twilight - Phantom Zant";
                    break;
                case "D_MN08D-R50":
                    getmap = "Palace of Twilight - Zant Intro";
                    break;
                case "D_MN08D-R53":
                    getmap = "Palace of Twilight - Diababa Phase";
                    break;
                case "D_MN08D-R54":
                    getmap = "Palace of Twilight - Goron Mines Phase";
                    break;
                case "D_MN08D-R55":
                    getmap = "Palace of Twilight - Lakebed Phase";
                    break;
                case "D_MN08D-R56":
                    getmap = "Palace of Twilight - Ook Phase";
                    break;
                case "D_MN08D-R57":
                    getmap = "Palace of Twilight - Blizzeta Phase";
                    break;
                case "D_MN08D-R60":
                    getmap = "Palace of Twilight - Hyrule Phase";
                    break;
                case "D_MN09-R01":
                    getmap = "Hyrule Castle - Main Hall";
                    break;
                case "D_MN09-R02":
                    getmap = "Hyrule Castle - Darknut";
                    break;
                case "D_MN09-R03":
                    getmap = "Hyrule Castle - Left Wing";
                    break;
                case "D_MN09-R04":
                    getmap = "Hyrule Castle - Left Wing";
                    break;
                case "D_MN09-R05":
                    getmap = "Hyrule Castle - Right Wing";
                    break;
                case "D_MN09-R06":
                    getmap = "Hyrule Castle - Right Wing";
                    break;
                case "D_MN09-R08":
                    getmap = "Hyrule Castle - Treasure Room";
                    break;
                case "D_MN09-R09":
                    getmap = "Hyrule Castle - Graveyard";
                    break;
                case "D_MN09-R11":
                    getmap = "Hyrule Castle - Entrance";
                    break;
                case "D_MN09-R12":
                    getmap = "Hyrule Castle - Final Ascension";
                    break;
                case "D_MN09-R13":
                    getmap = "Hyrule Castle - Outside Left Wing";
                    break;
                case "D_MN09-R14":
                    getmap = "Hyrule Castle - Outside Right Wing";
                    break;
                case "D_MN09-R15":
                    getmap = "Hyrule Castle - Outside Boss Key";
                    break;
                case "D_MN09A-R50":
                    getmap = "Hyrule Castle - Ganondorf Fight Inside";
                    break;
                case "D_MN09A-R51":
                    getmap = "Hyrule Castle - Ganondorf Intro Outside";
                    break;
                case "D_MN09B-R00":
                    getmap = "Hyrule Castle - Ganondorf Field";
                    break;
                case "D_MN09C-R00":
                    getmap = "Hyrule Castle - Ganondorf Defeated";
                    break;
                case "D_MN10-R00":
                    getmap = "Arbiters Grounds - Entrance";
                    break;
                case "D_MN10-R01":
                    getmap = "Arbiters Grounds - Before Main Room";
                    break;
                case "D_MN10-R02":
                    getmap = "Arbiters Grounds - Main Poe Room";
                    break;
                case "D_MN10-R03":
                    getmap = "Arbiters Grounds - 2nd Poe";
                    break;
                case "D_MN10-R04":
                    getmap = "Arbiters Grounds - Before 3rd Poe";
                    break;
                case "D_MN10-R05":
                    getmap = "Arbiters Grounds - 3rd Poe";
                    break;
                case "D_MN10-R06":
                    getmap = "Arbiters Grounds - Left Wing";
                    break;
                case "D_MN10-R07":
                    getmap = "Arbiters Grounds - Left Wing";
                    break;
                case "D_MN10-R08":
                    getmap = "Arbiters Grounds - 4th Poe";
                    break;
                case "D_MN10-R09":
                    getmap = "Arbiters Grounds - Boss Key";
                    break;
                case "D_MN10-R10":
                    getmap = "Arbiters Grounds - Ooccoo";
                    break;
                case "D_MN10-R11":
                    getmap = "Arbiters Grounds - Before Death Sword";
                    break;
                case "D_MN10-R12":
                    getmap = "Arbiters Grounds - Before 4th Poe";
                    break;
                case "D_MN10-R13":
                    getmap = "Arbiters Grounds - Spinner Rom";
                    break;
                case "D_MN10-R14":
                    getmap = "Arbiters Grounds - After 3rd Poe";
                    break;
                case "D_MN10-R15":
                    getmap = "Arbiters Grounds - Right Wing";
                    break;
                case "D_MN10-R16":
                    getmap = "Arbiters Grounds - Big Turning Room";
                    break;
                case "D_MN10A-R50":
                    getmap = "Arbiters Grounds - Boss";
                    break;
                case "D_MN10B-R51":
                    getmap = "Arbiters Grounds - Mini Boss";
                    break;
                case "D_MN11-R00":
                    getmap = "Snowpeak Ruins - Entrance";
                    break;
                case "D_MN11-R01":
                    getmap = "Snowpeak Ruins - Yeta";
                    break;
                case "D_MN11-R02":
                    getmap = "Snowpeak Ruins - Yeto";
                    break;
                case "D_MN11-R03":
                    getmap = "Snowpeak Ruins - Ice Puzzle";
                    break;
                case "D_MN11-R04":
                    getmap = "Snowpeak Ruins - Courtyard";
                    break;
                case "D_MN11-R05":
                    getmap = "Snowpeak Ruins - Ordon Pumpkin";
                    break;
                case "D_MN11-R06":
                    getmap = "Snowpeak Ruins - 3rd Poe";
                    break;
                case "D_MN11-R07":
                    getmap = "Snowpeak Ruins - Double Freezard";
                    break;
                case "D_MN11-R08":
                    getmap = "Snowpeak Ruins - Double LJA";
                    break;
                case "D_MN11-R09":
                    getmap = "Snowpeak Ruins - Ice Cannon Room";
                    break;
                case "D_MN11-R11":
                    getmap = "Snowpeak Ruins - Boss Key";
                    break;
                case "D_MN11-R13":
                    getmap = "Snowpeak Ruins - Before Ordon Pumpkin";
                    break;
                case "D_MN11A-R50":
                    getmap = "Snowpeak Ruins - Boss";
                    break;
                case "D_MN11B-R49":
                    getmap = "Snowpeak Ruins - Beta Room";
                    break;
                case "D_MN11B-R51":
                    getmap = "Snowpeak Ruins - Mini Boss";
                    break;
                /*Caves*/
                case "D_SB00-R00":
                    getmap = "Lanayru Ice Puzzle Cave";
                    break;
                case "D_SB01-R00":
                    getmap = "Cave of Ordeals - Room 1";
                    break;
                case "D_SB01-R01":
                    getmap = "Cave of Ordeals - Room 2";
                    break;
                case "D_SB01-R02":
                    getmap = "Cave of Ordeals - Room 3";
                    break;
                case "D_SB01-R03":
                    getmap = "Cave of Ordeals - Room 4";
                    break;
                case "D_SB01-R04":
                    getmap = "Cave of Ordeals - Room 5";
                    break;
                case "D_SB01-R05":
                    getmap = "Cave of Ordeals - Room 6";
                    break;
                case "D_SB01-R06":
                    getmap = "Cave of Ordeals - Room 7";
                    break;
                case "D_SB01-R07":
                    getmap = "Cave of Ordeals - Room 8";
                    break;
                case "D_SB01-R08":
                    getmap = "Cave of Ordeals - Room 9";
                    break;
                case "D_SB01-R09":
                    getmap = "Cave of Ordeals - Room 10";
                    break;
                case "D_SB01-R10":
                    getmap = "Cave of Ordeals - Room 11";
                    break;
                case "D_SB01-R11":
                    getmap = "Cave of Ordeals - Room 12";
                    break;
                case "D_SB01-R12":
                    getmap = "Cave of Ordeals - Room 13";
                    break;
                case "D_SB01-R13":
                    getmap = "Cave of Ordeals - Room 14";
                    break;
                case "D_SB01-R14":
                    getmap = "Cave of Ordeals - Room 15";
                    break;
                case "D_SB01-R15":
                    getmap = "Cave of Ordeals - Room 16";
                    break;
                case "D_SB01-R16":
                    getmap = "Cave of Ordeals - Room 17";
                    break;
                case "D_SB01-R17":
                    getmap = "Cave of Ordeals - Room 18";
                    break;
                case "D_SB01-R18":
                    getmap = "Cave of Ordeals - Room 19";
                    break;
                case "D_SB01-R19":
                    getmap = "Cave of Ordeals - Room 20";
                    break;
                case "D_SB01-R20":
                    getmap = "Cave of Ordeals - Room 21";
                    break;
                case "D_SB01-R21":
                    getmap = "Cave of Ordeals - Room 22";
                    break;
                case "D_SB01-R22":
                    getmap = "Cave of Ordeals - Room 23";
                    break;
                case "D_SB01-R23":
                    getmap = "Cave of Ordeals - Room 24";
                    break;
                case "D_SB01-R24":
                    getmap = "Cave of Ordeals - Room 25";
                    break;
                case "D_SB01-R25":
                    getmap = "Cave of Ordeals - Room 26";
                    break;
                case "D_SB01-R26":
                    getmap = "Cave of Ordeals - Room 27";
                    break;
                case "D_SB01-R27":
                    getmap = "Cave of Ordeals - Room 28";
                    break;
                case "D_SB01-R28":
                    getmap = "Cave of Ordeals - Room 29";
                    break;
                case "D_SB01-R29":
                    getmap = "Cave of Ordeals - Room 30";
                    break;
                case "D_SB01-R30":
                    getmap = "Cave of Ordeals - Room 31";
                    break;
                case "D_SB01-R31":
                    getmap = "Cave of Ordeals - Room 32";
                    break;
                case "D_SB01-R32":
                    getmap = "Cave of Ordeals - Room 33";
                    break;
                case "D_SB01-R33":
                    getmap = "Cave of Ordeals - Room 34";
                    break;
                case "D_SB01-R34":
                    getmap = "Cave of Ordeals - Room 35";
                    break;
                case "D_SB01-R35":
                    getmap = "Cave of Ordeals - Room 36";
                    break;
                case "D_SB01-R36":
                    getmap = "Cave of Ordeals - Room 37";
                    break;
                case "D_SB01-R37":
                    getmap = "Cave of Ordeals - Room 38";
                    break;
                case "D_SB01-R38":
                    getmap = "Cave of Ordeals - Room 39";
                    break;
                case "D_SB01-R39":
                    getmap = "Cave of Ordeals - Room 40";
                    break;
                case "D_SB01-R40":
                    getmap = "Cave of Ordeals - Room 41";
                    break;
                case "D_SB01-R41":
                    getmap = "Cave of Ordeals - Room 42";
                    break;
                case "D_SB01-R42":
                    getmap = "Cave of Ordeals - Room 43";
                    break;
                case "D_SB01-R43":
                    getmap = "Cave of Ordeals - Room 44";
                    break;
                case "D_SB01-R44":
                    getmap = "Cave of Ordeals - Room 45";
                    break;
                case "D_SB01-R45":
                    getmap = "Cave of Ordeals - Room 46";
                    break;
                case "D_SB01-R46":
                    getmap = "Cave of Ordeals - Room 47";
                    break;
                case "D_SB01-R47":
                    getmap = "Cave of Ordeals - Room 48";
                    break;
                case "D_SB01-R48":
                    getmap = "Cave of Ordeals - Room 49";
                    break;
                case "D_SB01-R49":
                    getmap = "Cave of Ordeals - Room 50";
                    break;
                case "D_SB02-R00":
                    getmap = "Eldin Long Cave";
                    break;
                case "D_SB03-R00":
                    getmap = "Lake Hylia Long Cave";
                    break;
                case "D_SB04-R10":
                    getmap = "Eldin Goron Stockcave";
                    break;
                case "D_SB05-R00":
                case "D_SB06-R01":
                case "D_SB07-R02":
                case "D_SB08-R03":
                case "D_SB09-R04":
                    getmap = "Grotto";
                    break;
                case "D_SB10-R00":
                    getmap = "Faron Woods - Cave";
                    break;
                /*Locations*/
                case "F_SP00-R00":
                    getmap = "Ordon Ranch";
                    break;
                case "F_SP102-R00":
                    getmap = "Hyrule Field - Rescue Mission";
                    break;
                case "F_SP103-R00":
                    getmap = "Ordon - Village";
                    break;
                case "F_SP103-R01":
                    getmap = "Ordon - Outside Links House";
                    break;
                case "F_SP104-R01":
                    getmap = "Ordon - Woods";
                    break;
                case "F_SP108-R00":
                    getmap = "Faron Woods - South Entrance";
                    break;
                case "F_SP108-R01":
                    getmap = "Faron Woods - Spring";
                    break;
                case "F_SP108-R02":
                    getmap = "Faron Woods - Spring to Coro";
                    break;
                case "F_SP108-R03":
                    getmap = "Faron Woods - Gate before Coro";
                    break;
                case "F_SP108-R04":
                    getmap = "Faron Woods - Coro";
                    break;
                case "F_SP108-R05":
                    getmap = "Faron Woods - Mist";
                    break;
                case "F_SP108-R06":
                    getmap = "Faron Woods - North Faron";
                    break;
                case "F_SP108-R08":
                    getmap = "Faton Woods - Coro Shortcut to Mist";
                    break;
                case "F_SP108-R11":
                    getmap = "Faron Woods - Mist to Faron";
                    break;
                case "F_SP108-R14":
                    getmap = "Faron Woods - Small Key Cave";
                    break;
                case "F_SP109-R00":
                    getmap = "Kakariko Village";
                    break;
                case "F_SP110-R00":
                    getmap = "Death Mountain - Entrance";
                    break;
                case "F_SP110-R01":
                    getmap = "Death Mountain - Entrance to Mountain";
                    break;
                case "F_SP110-R02":
                    getmap = "Death Mountain - Entrance to Mountain";
                    break;
                case "F_SP110-R03":
                    getmap = "Death Mountain";
                    break;
                case "F_SP111-R00":
                    getmap = "Kakariko Graveyard";
                    break;
                case "F_SP112-R01":
                    getmap = "Zoras River";
                    break;
                case "F_SP113-R00":
                    getmap = "Zoras Domains - Inside";
                    break;
                case "F_SP113-R01":
                    getmap = "Zoras Domains - Outside";
                    break;
                case "F_SP114-R00":
                    getmap = "Snowpeak - First Half";
                    break;
                case "F_SP114-R01":
                    getmap = "Snowpeak - Second Half";
                    break;
                case "F_SP114-R02":
                    getmap = "Snowpeak - Transition Cave";
                    break;
                case "F_SP115-R00":
                    getmap = "Lake Hylia";
                    break;
                case "F_SP115-R01":
                    getmap = "Lake Hylia - Fountain";
                    break;
                case "F_SP116-R00":
                    getmap = "Castle Town - Central";
                    break;
                case "F_SP116-R01":
                    getmap = "Castle Town - North";
                    break;
                case "F_SP116-R02":
                    getmap = "Castle Town - West";
                    break;
                case "F_SP116-R03":
                    getmap = "Castle Town - South";
                    break;
                case "F_SP116-R04":
                    getmap = "Castle Town - East";
                    break;
                case "F_SP117-R01":
                    getmap = "Sacred Grove - Master Sword";
                    break;
                case "F_SP117-R02":
                    getmap = "Sacred Grove - Temple of Time";
                    break;
                case "F_SP117-R03":
                    getmap = "Sacred Grove - Lost Woods";
                    break;
                case "F_SP118-R00":
                    getmap = "Bublin Camp - Camp Geometry";
                    break;
                case "F_SP118-R01":
                    getmap = "Bublin Camp";
                    break;
                case "F_SP118-R02":
                    getmap = "Bublin Camp - Beta";
                    break;
                case "F_SP118-R03":
                    getmap = "Bublin Camp - Before Arbiters Grounds";
                    break;
                case "F_SP121-R00":
                case "F_SP121-R01":
                case "F_SP121-R02":
                case "F_SP121-R03":
                case "F_SP121-R04":
                case "F_SP121-R05":
                case "F_SP121-R06":
                case "F_SP121-R07":
                case "F_SP121-R09":
                case "F_SP121-R10":
                case "F_SP121-R11":
                case "F_SP121-R12":
                case "F_SP121-R13":
                case "F_SP121-R14":
                case "F_SP121-R15":
                    getmap = "Hyrule Field";
                    break;
                case "F_SP122-R08":
                    getmap = "Outside Castle Town - West Field";
                    break;
                case "F_SP122-R16":
                    getmap = "Outside Castle Town - South Field";
                    break;
                case "F_SP122-R17":
                    getmap = "Outside Castle Town - East Field";
                    break;
                case "F_SP123-R13":
                    getmap = "Bublin";
                    break;
                case "F_SP124-R00":
                    getmap = "Gerudo Desert";
                    break;
                case "F_SP125-R04":
                    getmap = "Mirror Chamber";
                    break;
                case "F_SP126-R00":
                    getmap = "Upper Zoras River";
                    break;
                case "F_SP127-R00":
                    getmap = "Fishing Pond";
                    break;
                case "F_SP128-R00":
                    getmap = "Hidden Village";
                    break;
                case "F_SP200-R00":
                    getmap = "Hidden Skill";
                    break;
                /*Rooms*/
                case "R_SP01-R00":
                    getmap = "Ordon Village - Bo House";
                    break;
                case "R_SP01-R01":
                    getmap = "Ordon Village - Shop House";
                    break;
                case "R_SP01-R02":
                    getmap = "Ordon Village - Shield House";
                    break;
                case "R_SP01-R04":
                    getmap = "Ordon Village - Links House";
                    break;
                case "R_SP01-R05":
                    getmap = "Ordon Village - Sword House";
                    break;
                case "R_SP01-R07":
                    getmap = "Ordon Village - Links House Storage";
                    break;
                case "R_SP107-R00":
                    getmap = "Hyrule Castle Sewers - Prison";
                    break;
                case "R_SP107-R01":
                    getmap = "Hyrule Castle Sewers - Sewers";
                    break;
                case "R_SP107-R02":
                    getmap = "Hyrule Castle Sewers - Rooftops";
                    break;
                case "R_SP107-R03":
                    getmap = "Hyrule Castle Sewers - Tower";
                    break;
                case "R_SP108-R00":
                    getmap = "Faron Woods - Coros House";
                    break;
                case "R_SP109-R00":
                    getmap = "Kakariko Village - Renardos Sanctuary";
                    break;
                case "R_SP109-R01":
                    getmap = "Kakariko Village - Barnes Shop";
                    break;
                case "R_SP109-R02":
                    getmap = "Kakariko Village - Hotel";
                    break;
                case "R_SP109-R03":
                    getmap = "Kakariko Village - Malos Shop";
                    break;
                case "R_SP109-R04":
                    getmap = "Kakariko Village - Top House";
                    break;
                case "R_SP109-R05":
                    getmap = "Kakariko Village - Bomb House";
                    break;
                case "R_SP109-R06":
                    getmap = "Kakariko Village - Bug House";
                    break;
                case "R_SP110-R00":
                    getmap = "Death Mountain - Goron Elder Cave";
                    break;
                case "R_SP116-R05":
                    getmap = "Telmas Bar";
                    break;
                case "R_SP116-R06":
                    getmap = "Jovani to Sewers";
                    break;
                case "R_SP127-R00":
                    getmap = "Fishing Pond - Henas House";
                    break;
                case "R_SP128-R00":
                    getmap = "Hidden Village - Impaz House";
                    break;
                case "R_SP160-R00":
                    getmap = "Castle Town - Malos Shop";
                    break;
                case "R_SP160-R01":
                    getmap = "Castle Town - Fortune Teller House";
                    break;
                case "R_SP160-R02":
                    getmap = "Castle Town - Doctor House";
                    break;
                case "R_SP160-R03":
                    getmap = "Castle Town - Agitha House";
                    break;
                case "R_SP160-R04":
                    getmap = "Castle Town - Goron Shops";
                    break;
                case "R_SP160-R05":
                    getmap = "Castle Town - Jovani House";
                    break;
                case "R_SP161-R07":
                    getmap = "Star Game";
                    break;
                case "R_SP209-R07":
                    getmap = "Kakariko Graveyard - Sanctuary Cave";
                    break;
                case "R_SP300-R00":
                    getmap = "Cutscene";
                    break;
                case "R_SP301-R00":
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
