using System;
using System.Collections.Generic;

namespace MultiPresence.Models.KH02
{
    public static class Locations
    {
        private static readonly Dictionary<string, string> WorldNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "di", "Destiny Islands" },
            { "dw", "Dark World" },
            { "yt", "The Mysterious Tower" }
        };
        private static readonly Dictionary<string, string> MapNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "dw_01", "Main Road" },
            { "dw_02", "Castle Town" },
            { "dw_03", "The World Within" },
            { "dw_08", "Forest of Thorns" },
            { "dw_09", "Depths of Darkness" },
            { "dw_10", "Destiny Islands" }
        };
        private static readonly Dictionary<string, string> AreaNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "dw_01_An_01", "Dark World" },
            { "dw_02_An_01", "Dark World" },
            { "dw_02_An_02", "Dark World" },
            { "dw_02_An_03", "Dark World" },
            { "dw_03_An_01", "Dark World" },
            { "dw_03_An_02", "Dark World" },
            { "dw_03_An_03", "Dark World" },
            { "dw_03_An_04", "Dark World" },
            { "dw_03_An_05", "Dark World" },
            { "dw_04_An_01", "Dark World" },
            { "dw_04_An_02", "Dark World" },
            { "dw_04_An_03", "Dark World" },
            { "dw_04_An_04", "Dark World" },
            { "dw_05_An_01", "Dark World" },
            { "dw_05_An_02", "Dark World" },
            { "dw_05_An_03", "Dark World" },
            { "dw_05_An_04", "Dark World" },
            { "dw_06_An_01", "Dark World" },
            { "dw_07_An_01", "Dark World" },
            { "dw_08_An_01", "Dark World" },
            { "dw_08_An_02", "Dark World" },
            { "dw_08_An_03", "Dark World" },
            { "dw_08_An_04", "Dark World" },
            { "dw_09_An_01", "Dark World" },
            { "dw_09_An_02", "Dark World" },
            { "dw_10_An_01", "Dark World" }
        };
        private static readonly Dictionary<string, string> SubAreaNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "dw_01_An_01", "Main Road" },
            { "dw_02_An_01", "Castle Town" },
            { "dw_02_An_02", "Castle Town" },
            { "dw_02_An_03", "Castle Town" },
            { "dw_03_An_01", "The World Within" },
            { "dw_03_An_02", "The World Within" },
            { "dw_03_An_03", "The World Within" },
            { "dw_03_An_04", "The World Within" },
            { "dw_03_An_05", "The World Within" },
            { "dw_04_An_01", "The World Within" },
            { "dw_04_An_02", "The World Within" },
            { "dw_04_An_03", "The World Within" },
            { "dw_04_An_04", "The World Within" },
            { "dw_05_An_01", "The World Within" },
            { "dw_05_An_02", "The World Within" },
            { "dw_05_An_03", "The World Within" },
            { "dw_05_An_04", "The World Within" },
            { "dw_06_An_01", "The World Within" },
            { "dw_07_An_01", "The World Within" },
            { "dw_08_An_01", "Uncertain Path" },
            { "dw_08_An_02", "Rocky Path" },
            { "dw_08_An_03", "Rocky Path" },
            { "dw_08_An_04", "Path's End" },
            { "dw_09_An_01", "Depths of Darkness" },
            { "dw_09_An_02", "Depths of Darkness" },
            { "dw_10_An_01", "Destiny Islands" }
        };
        private static readonly Dictionary<string, string> AreaMaps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "dw_01_An_01", "dw_01" },
            { "dw_02_An_01", "dw_02" },
            { "dw_02_An_02", "dw_02" },
            { "dw_02_An_03", "dw_02" },
            { "dw_03_An_01", "dw_03" },
            { "dw_03_An_02", "dw_03" },
            { "dw_03_An_03", "dw_03" },
            { "dw_03_An_04", "dw_03" },
            { "dw_03_An_05", "dw_03" },
            { "dw_04_An_01", "dw_03" },
            { "dw_04_An_02", "dw_03" },
            { "dw_04_An_03", "dw_03" },
            { "dw_04_An_04", "dw_03" },
            { "dw_05_An_01", "dw_03" },
            { "dw_05_An_02", "dw_03" },
            { "dw_05_An_03", "dw_03" },
            { "dw_05_An_04", "dw_03" },
            { "dw_06_An_01", "dw_03" },
            { "dw_07_An_01", "dw_03" },
            { "dw_08_An_01", "dw_08" },
            { "dw_08_An_02", "dw_08" },
            { "dw_08_An_03", "dw_08" },
            { "dw_08_An_04", "dw_08" },
            { "dw_09_An_01", "dw_09" },
            { "dw_09_An_02", "dw_09" },
            { "dw_10_An_01", "dw_10" }
        };
        private static readonly Dictionary<string, string> MapAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "dw_04", "dw_03" },
            { "dw_05", "dw_03" },
            { "dw_06", "dw_03" },
            { "dw_07", "dw_03" }
        };
        public static string GetWorld(string worldKey)
        {
            return Lookup(WorldNames, worldKey);
        }

        public static string GetWorldIcon(string worldKey)
        {
            return string.Equals(worldKey, "dw", StringComparison.OrdinalIgnoreCase) ? "darkworld" : "logo";
        }

        public static string GetMap(string mapKey)
        {
            string alias = Lookup(MapAliases, mapKey);
            return Lookup(MapNames, alias.Length == 0 ? mapKey : alias);
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
            mapKey ??= string.Empty;
            string map = GetMapForArea(areaKey, mapKey);
            if (map.Length == 0)
            {
                map = mapKey;
            }

            string world = mapKey.Length >= 2 ? GetWorld(mapKey[..2]) : string.Empty;
            var parts = new List<string>();
            Add(parts, world);
            if (IsCurrentArea(areaKey, mapKey))
            {
                Add(parts, GetArea(areaKey));
            }

            Add(parts, map);
            if (IsCurrentArea(areaKey, mapKey))
            {
                Add(parts, GetSubArea(areaKey));
            }

            return string.Join(" / ", parts);
        }

        private static void Add(List<string> parts, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            foreach (string part in parts)
            {
                if (string.Equals(part, value, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            parts.Add(value);
        }

        private static string Lookup(Dictionary<string, string> values, string key)
        {
            return key != null && values.TryGetValue(key, out var value) ? value : string.Empty;
        }
    }
}
