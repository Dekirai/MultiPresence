using System;
using System.Collections.Generic;

namespace MultiPresence.Models.KH3
{
    public static class Locations
    {
        private static readonly Dictionary<string, string> WorldNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "bt", "Scala ad Caelum" },
            { "bx", "San Fransokyo" },
            { "ca", "The Caribbean" },
            { "co", "Castle Oblivion" },
            { "db", "Daybreak Town" },
            { "dh", "Station of Awakening" },
            { "di", "Destiny Islands" },
            { "dp", "The Land of Departure" },
            { "dw", "The Realm of Darkness" },
            { "ew", "The Final World" },
            { "fz", "Arendelle" },
            { "he", "Olympus" },
            { "kg", "The Keyblade Graveyard" },
            { "mi", "Monstropolis" },
            { "po", "The Hundred Acre Wood" },
            { "ra", "Kingdom of Corona" },
            { "rg", "Radiant Garden" },
            { "ts", "Toy Box" },
            { "tt", "Twilight Town" },
            { "wm", "World Map" },
            { "yt", "The Mysterious Tower" }
        };
        private static readonly Dictionary<string, string> MapNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "bt_01", "The Stairway to the Sky" },
            { "bt_07", "Breezy Quarter" },
            { "bx_01", "The Bridge" },
            { "bx_02", "The City" },
            { "bx_03", "Hiro's Garage" },
            { "ca_01", "Port Royal" },
            { "ca_02", "The High Seas" },
            { "ca_04", "Davy Jones' Locker" },
            { "ca_05", "Over the Edge" },
            { "dp_01", "The Land of Departure" },
            { "dw_01", "The Realm of Darkness" },
            { "ew_01", "The Final World" },
            { "fz_01", "The North Mountain" },
            { "fz_02", "The Labyrinth of Ice" },
            { "he_01", "Realm of the Gods" },
            { "he_02", "Mount Olympus" },
            { "he_03", "Thebes" },
            { "kg_01", "The Badlands" },
            { "kg_02", "The Skein of Severance" },
            { "mi_01", "Monsters, Inc." },
            { "mi_02", "The Factory" },
            { "mi_03", "The Power Plant" },
            { "mi_04", "The Door Vault" },
            { "po_01", "Rabbit's House" },
            { "ra_01", "The Forest" },
            { "ra_02", "The Kingdom" },
            { "rg_01", "Merlin's House" },
            { "rg_03", "Garden of Assemblage" },
            { "ts_01", "Andy's House" },
            { "ts_02", "Galaxy Toys" },
            { "tt_01", "The Neighborhood" },
            { "wm_01", "World Map" }
        };
        private static readonly Dictionary<string, string> AreaNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "bx_02_An_01", "North District" },
            { "bx_02_An_02", "Central District" },
            { "bx_02_An_03", "South District" },
            { "ca_01_An_01", "Docks" },
            { "ca_01_An_02", "Seaport" },
            { "ca_01_An_03", "Fort" },
            { "ca_01_An_04", "Settlement" },
            { "ca_01_An_05", "Dockside Path" },
            { "ca_02_An_01", "Port Royal Waters" },
            { "ca_02_An_02", "Northern Waters" },
            { "ca_02_An_03", "Southern Waters" },
            { "ca_02_An_04", "Huddled Isles" },
            { "ca_02_An_05", "Undersea Cavern" },
            { "ca_02_An_06", "Sandbar Isle" },
            { "ca_02_An_07", "Forsaken Isle" },
            { "ca_02_An_08", "Isla Verdemontaña" },
            { "ca_02_An_09", "Isla de los Mástiles" },
            { "fz_01_An_01", "Mountain Ridge" },
            { "fz_01_An_02", "Ice Palace" },
            { "fz_01_An_03", "Treescape" },
            { "fz_01_An_04", "Gorge" },
            { "fz_01_An_05", "Valley of Ice" },
            { "fz_01_An_06", "Frozen Wall" },
            { "fz_01_An_07", "Snowfield" },
            { "fz_01_An_08", "Fjord" },
            { "fz_02_An_01", "Lower Tier" },
            { "fz_02_An_02", "Middle Tier" },
            { "fz_02_An_03", "Upper Tier" },
            { "fz_05_An_01", "Foothills" },
            { "he_01_An_01", "Courtyard" },
            { "he_01_An_02", "Corridors" },
            { "he_01_An_03", "Secluded Forge" },
            { "he_01_An_04", "Cloud Ridge" },
            { "he_01_An_05", "Apex" },
            { "he_02_An_01", "Ravine" },
            { "he_02_An_02", "Cliff Ascent" },
            { "he_02_An_03", "Mountainside" },
            { "he_02_An_04", "Summit" },
            { "he_03_An_01", "Agora" },
            { "he_03_An_02", "Gardens" },
            { "he_03_An_03", "Overlook" },
            { "he_03_An_04", "The Big Olive" },
            { "he_03_An_05", "Alleyway" },
            { "kg_02_An_01", "Trail of Valediction" },
            { "kg_02_An_02", "Twist of Isolation" },
            { "kg_02_An_03", "Tower of Endings" },
            { "mi_01_An_01", "Lobby & Offices" },
            { "mi_01_An_02", "Laugh Floor" },
            { "mi_02_An_01", "Basement" },
            { "mi_02_An_02", "Ground Floor" },
            { "mi_02_An_03", "Second Floor" },
            { "mi_03_An_01", "Accessway" },
            { "mi_03_An_02", "Tank Yard" },
            { "mi_03_An_03", "Vault Passage" },
            { "mi_04_An_01", "Upper Level" },
            { "mi_04_An_02", "Lower Level" },
            { "mi_04_An_03", "Service Area" },
            { "ra_01_An_01", "Tower" },
            { "ra_01_An_02", "Hills" },
            { "ra_01_An_03", "Marsh" },
            { "ra_01_An_04", "Campsite" },
            { "ra_01_An_05", "Wetlands" },
            { "ra_01_An_06", "Wildflower Clearing" },
            { "ra_01_An_07", "Shore" },
            { "ra_02_An_01", "Thoroughfare" },
            { "ra_02_An_02", "Wharf" },
            { "ts_02_An_01", "Main Floor: 1F" },
            { "ts_02_An_02", "Main Floor: 2F" },
            { "ts_02_An_03", "Main Floor: 3F" },
            { "ts_02_An_04", "Babies & Toddlers: Dolls" },
            { "ts_02_An_05", "Babies & Toddlers: Outdoors" },
            { "ts_02_An_06", "Lower Vents" },
            { "ts_02_An_07", "Upper Vents" },
            { "ts_02_An_08", "Action Figures" },
            { "ts_02_An_09", "Video Games" },
            { "ts_02_An_10", "Kid Korral" },
            { "ts_02_An_11", "Rest Area" },
            { "tt_01_An_01", "Tram Common" },
            { "tt_01_An_02", "Underground Conduit" },
            { "tt_01_An_03", "The Woods" },
            { "tt_01_An_04", "The Old Mansion" }
        };
        private static readonly Dictionary<string, string> SubAreaNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
        };
        private static readonly Dictionary<string, string> AreaMaps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "bt_01_An_01", "bt_01" },
            { "bt_07_An_01", "bt_07" },
            { "bx_01_An_01", "bx_01" },
            { "bx_02_An_01", "bx_02" },
            { "bx_02_An_02", "bx_02" },
            { "bx_02_An_03", "bx_02" },
            { "bx_03_An_01", "bx_03" },
            { "ca_01_An_01", "ca_01" },
            { "ca_01_An_02", "ca_01" },
            { "ca_01_An_03", "ca_01" },
            { "ca_01_An_04", "ca_01" },
            { "ca_01_An_05", "ca_01" },
            { "ca_01_An_06", "ca_01" },
            { "ca_02_An_01", "ca_02" },
            { "ca_02_An_02", "ca_02" },
            { "ca_02_An_03", "ca_02" },
            { "ca_02_An_04", "ca_02" },
            { "ca_02_An_05", "ca_02" },
            { "ca_02_An_06", "ca_02" },
            { "ca_02_An_07", "ca_02" },
            { "ca_02_An_08", "ca_02" },
            { "ca_02_An_09", "ca_02" },
            { "ca_04_An_01", "ca_04" },
            { "ca_05_An_01", "ca_05" },
            { "dp_01_An_01", "dp_01" },
            { "dw_21_An_01", "dw_01" },
            { "dw_71_An_01", "dw_01" },
            { "ew_01_An_01", "ew_01" },
            { "fz_01_An_01", "fz_01" },
            { "fz_01_An_02", "fz_01" },
            { "fz_01_An_03", "fz_01" },
            { "fz_01_An_04", "fz_01" },
            { "fz_01_An_05", "fz_01" },
            { "fz_01_An_06", "fz_01" },
            { "fz_01_An_07", "fz_01" },
            { "fz_01_An_08", "fz_01" },
            { "fz_02_An_01", "fz_02" },
            { "fz_02_An_02", "fz_02" },
            { "fz_02_An_03", "fz_02" },
            { "fz_05_An_01", "fz_01" },
            { "he_01_An_01", "he_01" },
            { "he_01_An_02", "he_01" },
            { "he_01_An_03", "he_01" },
            { "he_01_An_04", "he_01" },
            { "he_01_An_05", "he_01" },
            { "he_02_An_01", "he_02" },
            { "he_02_An_02", "he_02" },
            { "he_02_An_03", "he_02" },
            { "he_02_An_04", "he_02" },
            { "he_03_An_01", "he_03" },
            { "he_03_An_02", "he_03" },
            { "he_03_An_03", "he_03" },
            { "he_03_An_04", "he_03" },
            { "he_03_An_05", "he_03" },
            { "kg_01_An_01", "kg_01" },
            { "kg_02_An_01", "kg_02" },
            { "kg_02_An_02", "kg_02" },
            { "kg_02_An_03", "kg_02" },
            { "mi_01_An_01", "mi_01" },
            { "mi_01_An_02", "mi_01" },
            { "mi_02_An_01", "mi_02" },
            { "mi_02_An_02", "mi_02" },
            { "mi_02_An_03", "mi_02" },
            { "mi_03_An_01", "mi_03" },
            { "mi_03_An_02", "mi_03" },
            { "mi_03_An_03", "mi_03" },
            { "mi_04_An_01", "mi_04" },
            { "mi_04_An_02", "mi_04" },
            { "mi_04_An_03", "mi_04" },
            { "po_01_An_01", "po_01" },
            { "ra_01_An_01", "ra_01" },
            { "ra_01_An_02", "ra_01" },
            { "ra_01_An_03", "ra_01" },
            { "ra_01_An_04", "ra_01" },
            { "ra_01_An_05", "ra_01" },
            { "ra_01_An_06", "ra_01" },
            { "ra_01_An_07", "ra_01" },
            { "ra_02_An_01", "ra_02" },
            { "ra_02_An_02", "ra_02" },
            { "rg_01_An_01", "rg_01" },
            { "rg_03_An_01", "rg_03" },
            { "ts_01_An_01", "ts_01" },
            { "ts_02_An_01", "ts_02" },
            { "ts_02_An_02", "ts_02" },
            { "ts_02_An_03", "ts_02" },
            { "ts_02_An_04", "ts_02" },
            { "ts_02_An_05", "ts_02" },
            { "ts_02_An_06", "ts_02" },
            { "ts_02_An_07", "ts_02" },
            { "ts_02_An_08", "ts_02" },
            { "ts_02_An_09", "ts_02" },
            { "ts_02_An_10", "ts_02" },
            { "ts_02_An_11", "ts_02" },
            { "tt_01_An_01", "tt_01" },
            { "tt_01_An_02", "tt_01" },
            { "tt_01_An_03", "tt_01" },
            { "tt_01_An_04", "tt_01" },
            { "wm_01_An_01", "wm_01" }
        };
        public static string GetWorld(string worldKey)
        {
            return Lookup(WorldNames, worldKey);
        }

        public static string GetMap(string mapKey)
        {
            return Lookup(MapNames, mapKey);
        }

        public static string GetArea(string areaKey)
        {
            return Lookup(AreaNames, areaKey);
        }

        public static string GetSubArea(string areaKey)
        {
            return Lookup(SubAreaNames, areaKey);
        }

        public static string GetAreaMapKey(string areaKey)
        {
            return Lookup(AreaMaps, areaKey);
        }

        public static bool IsCurrentArea(string areaKey, string rawMapKey)
        {
            string canonical = GetAreaMapKey(areaKey);
            if (canonical.Length == 0 || string.IsNullOrEmpty(rawMapKey))
            {
                return false;
            }

            return string.Equals(canonical, rawMapKey, StringComparison.OrdinalIgnoreCase) || areaKey.StartsWith(rawMapKey + "_An_", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetMapForArea(string areaKey, string rawMapKey)
        {
            return IsCurrentArea(areaKey, rawMapKey) ? GetMap(GetAreaMapKey(areaKey)) : GetMap(rawMapKey);
        }

        public static string GetLocation(string areaKey, string mapKey)
        {
            string map = GetMapForArea(areaKey, mapKey);
            if (!IsCurrentArea(areaKey, mapKey))
            {
                return map;
            }

            string area = GetArea(areaKey);
            string subarea = GetSubArea(areaKey);
            string result = Join(map, area);
            return Join(result, subarea);
        }

        private static string Join(string first, string second)
        {
            return string.IsNullOrEmpty(second) || first == second ? first : string.IsNullOrEmpty(first) ? second : first + " / " + second;
        }

        private static string Lookup(Dictionary<string, string> values, string key)
        {
            return key != null && values.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
