namespace MultiPresence.Models.CROC2
{
    public static class Locations
    {
        public static string GetMapId(int world, int level, int type, int map)
        {
            if (world < 0 || level < 0 || type is < 0 or > 3 || map is < 0 or > 999)
            {
                return "";
            }

            return $"T{world}{"LBSI"[type]}{level}M{map:D3}";
        }

        public static string GetName(int world, int level, int type, int map)
        {
            switch (GetMapId(world, level, type, map))
            {
                case "T0L1M001": return "Bride of the Dungeon of Defright";
                case "T0L1M002": return "The Village Masher";
                case "T0L1M003": return "Burning Down the Forest";
                case "T0L1M004": return "Find the Wheels in the Jungle!";
                case "T0L1M005": return "HEY! He Stole my lunch!";
                case "T0L1M006": return "Save the Ice Trapped Gobbos!";
                case "T0L1M007": return "Find 3 Lost Treasure Chests";
                case "T1B1M001": return "Soveena The Squid";
                case "T1B1M002": return "Soveena The Squid";
                case "T1B2M001": return "Cannon Boat Keith";
                case "T1I1M001": return "Main Game Intro";
                case "T1I1M002": return "Sailor Village";
                case "T1I2M001": return "Cannon Boat Keith";
                case "T1L1M001": return "Sailor Village";
                case "T1L1M004": return "Swap Meet Pete's General Store";
                case "T1L1M010": return "AAAGH Mad Gobbo !!!";
                case "T1L2M001": return "Find the Key! Save the Gobbo!";
                case "T1L3M001": return "Find 5 Lost Treasure Chests";
                case "T1L4M001": return "Get the Gobbo's Sandwich!";
                case "T1L5M001": return "Croc Vs Dantini Boat Race";
                case "T1L6M001": return "Save the Bird from the Thief";
                case "T1S1M001": return "Sailor Tribe - Secret Mine";
                case "T2B1M001": return "Flavio the Thermal Fish";
                case "T2B2M001": return "Lava Lamp Larry";
                case "T2I2M001": return "Lava Lamp Larry";
                case "T2L1M001": return "Cossack Village";
                case "T2L1M004": return "Swap Meet Pete's General Store";
                case "T2L2M001": return "Save the Ice Trapped Gobbos!";
                case "T2L3M001": return "Chase the Choo Choo Train";
                case "T2L5M001": return "It's just Snowball Madness!";
                case "T2L6M001": return "Hang Glider Valley";
                case "T2L7M001": return "KaBooom! It's Roger Red Ant";
                case "T2S1M001": return "Cossack Tribe - Secret Ice Cap";
                case "T3B1M001": return "Venus Fly Von-Trappe";
                case "T3B1M002": return "Venus Fly Von-Trappe";
                case "T3B2M001": return "The Village Masher";
                case "T3B2M002": return "The Village Masher";
                case "T3I1M001": return "The Village Masher";
                case "T3L1M001": return "Caveman Village";
                case "T3L1M004": return "Swap Meet Pete's General Store";
                case "T3L2M001": return "Find the Wheels in the Jungle!";
                case "T3L3M001": return "Find the Wheels in the Mine!";
                case "T3L4M001": return "Race Day at Goldrock";
                case "T3L5M001": return "Climb The Devil's Tower";
                case "T3L7M001": return "Save 50 Trapped Gobbos!";
                case "T3S1M001": return "Caveman Tribe - Secret Mine";
                case "T4B1M001": return "Dante's Peak";
                case "T4B2M001": return "Dante's Final Fight";
                case "T4I1M001": return "Dante's Peak";
                case "T4L1M001": return "Inca Village";
                case "T4L1M004": return "Swap Meet Pete's General Store";
                case "T4L2M001": return "Save 30 Gobbo babies!";
                case "T4L3M001": return "Up the Waterfall";
                case "T4L5M001": return "Bride of the Dungeon of Defright";
                case "T4L6M001": return "Goo Man Chu's Tower";
                case "T4S1M001": return "Inca Tribe - Secret Maze";
                case "T5L2M001": return "Secret Sailor Village";
                case "T5L2M002": return "Sailor Secret 1";
                case "T5L2M003": return "Sailor Secret 2";
                case "T5L2M004": return "Sailor Secret 3";
                case "T5L2M005": return "Sailor Secret 4";
                case "T5L2M006": return "Sailor Secret 5";
                case "T5L3M001": return "Secret Cossack Village";
                case "T5L3M002": return "Cossack Secret 1";
                case "T5L3M003": return "Cossack Secret 2";
                case "T5L3M004": return "Cossack Secret 3";
                case "T5L3M005": return "Cossack Secret 4";
                case "T5L3M006": return "Cossack Secret 5";
                case "T5L4M001": return "Secret Caveman Village";
                case "T5L4M002": return "Caveman Secret 1";
                case "T5L4M003": return "Caveman Secret 2";
                case "T5L4M004": return "Caveman Secret 3";
                case "T5L4M005": return "Caveman Secret 4";
                case "T5L4M006": return "Caveman Secret 5";
                default: return "Unknown Location";
            }
        }

        public static string GetWorld(int world, int level)
        {
            if (world == 5)
            {
                switch (level)
                {
                    case 2: return "Secret Sailor Village";
                    case 3: return "Secret Cossack Village";
                    case 4: return "Secret Caveman Village";
                    default: return "";
                }
            }

            switch (world)
            {
                case 1: return "Sailor Village";
                case 2: return "Cossack Village";
                case 3: return "Caveman Village";
                case 4: return "Inca Village";
                default: return "";
            }
        }

        public static bool IsOverworld(int world, int level, int type, int map)
        {
            return type == 0 && map == 1 &&
                ((world is >= 1 and <= 4 && level == 1) || (world == 5 && level is >= 2 and <= 4));
        }

        public static bool IsShop(int world, int level, int type, int map)
        {
            return world is >= 1 and <= 4 && level == 1 && type == 0 && map == 4;
        }
    }
}
