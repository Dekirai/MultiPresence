using System;
using System.Collections.Generic;

namespace MultiPresence.Models.RE0
{
    public static class Locations
    {
        private static readonly string[] ScenePrefixes = { "tr", "sh", "dm", "ch", "bt", "pl" };
        private static readonly int[] SceneStarts = { 0, 36, 86, 99, 132, 141, 175 };
        private static readonly Dictionary<string, string> AreaNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "tr", "Train" },
            { "sh", "Training Facility" },
            { "dm", "Facility Basement" },
            { "ch", "Laboratory" },
            { "bt", "Factory" },
            { "pl", "Treatment Plant" }
        };
        private static readonly Dictionary<string, byte[]> Floors = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "bt00", new byte[] { 8, 8, 8, 8, 9 } },
            { "bt01", new byte[] { 8, 8, 8, 8, 8 } },
            { "bt02", new byte[] { 9, 9, 9, 8, 8, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 } },
            { "bt03", new byte[] { 8, 8, 8, 8, 8 } },
            { "bt04", new byte[] { 9, 9, 9, 9, 9, 255, 255, 255, 255, 255, 255, 255, 255 } },
            { "bt05", new byte[] { 8, 8, 8, 8, 9 } },
            { "bt06", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 } },
            { "bt07", new byte[] { 5, 5 } },
            { "bt08", new byte[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 9, 9, 9, 8, 8 } },
            { "ch00", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 } },
            { "ch01", new byte[] { 9, 9, 9, 9, 9 } },
            { "ch02", new byte[] { 10, 10 } },
            { "ch03", new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7 } },
            { "ch04", new byte[] { 7, 7, 7, 7, 7, 7, 8, 8, 8, 8 } },
            { "ch05", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "ch06", new byte[] { 8, 8, 8, 8, 8 } },
            { "ch07", new byte[] { 7, 7, 7 } },
            { "ch08", new byte[] { 8, 8, 8, 8, 8, 8 } },
            { "ch09", new byte[] { 8, 8, 8, 8 } },
            { "ch0a", new byte[] { 8, 8, 8, 8, 8 } },
            { "ch0b", new byte[] { 8, 8, 8, 8, 8, 8 } },
            { "ch0c", new byte[] { 7, 7 } },
            { "ch0d", new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 } },
            { "ch0e", new byte[] { 7, 7, 7, 7, 7 } },
            { "ch0f", new byte[] { 9, 9 } },
            { "ch10", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 } },
            { "ch11", new byte[] { 9, 9, 9, 9, 9, 9 } },
            { "ch13", new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7 } },
            { "ch14", new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7 } },
            { "ch15", new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7 } },
            { "ch17", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "ch18", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "ch19", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "ch1b", new byte[] { 8, 8, 8, 8, 8, 8, 8 } },
            { "ch1c", new byte[] { 7, 7, 7, 7, 7, 7 } },
            { "ch1d", new byte[] { 7, 7, 7, 7, 7, 7 } },
            { "ch1f", new byte[] { 9, 9, 9, 9, 9 } },
            { "ch20", new byte[] { 9 } },
            { "dm00", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "dm01", new byte[] { 8, 8, 8, 8 } },
            { "dm02", new byte[] { 8, 8, 8, 8 } },
            { "dm03", new byte[] { 8, 8, 8 } },
            { "dm04", new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 } },
            { "dm05", new byte[] { 7, 7 } },
            { "dm06", new byte[] { 7, 7, 7, 7 } },
            { "dm07", new byte[] { 7, 7 } },
            { "dm08", new byte[] { 6, 6, 6 } },
            { "dm09", new byte[] { 6, 6, 6, 6, 6, 6 } },
            { "dm0a", new byte[] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 } },
            { "dm0b", new byte[] { 8, 8, 8, 8, 8 } },
            { "dm0c", new byte[] { 8, 8, 8, 8 } },
            { "pl00", new byte[] { 4, 4 } },
            { "pl01", new byte[] { 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 4 } },
            { "pl02", new byte[] { 4, 4, 4, 4, 4 } },
            { "pl03", new byte[] { 3, 3, 3, 3, 3, 3 } },
            { "pl04", new byte[] { 3, 3, 3, 3, 3 } },
            { "pl05", new byte[] { 3, 3, 3, 3 } },
            { "pl06", new byte[] { 3, 3 } },
            { "pl07", new byte[] { 3, 3, 3, 3 } },
            { "pl09", new byte[] { 3, 3, 3, 3 } },
            { "pl0a", new byte[] { 2, 2, 2, 2 } },
            { "pl0b", new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2 } },
            { "pl0c", new byte[] { 2, 2, 2, 2, 2, 2, 2 } },
            { "pl0d", new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 } },
            { "pl0e", new byte[] { 2, 2 } },
            { "pl0f", new byte[] { 1, 1, 1, 1 } },
            { "pl10", new byte[] { 1, 1, 1 } },
            { "pl11", new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 } },
            { "pl12", new byte[] { 1, 1, 1, 1, 1 } },
            { "pl13", new byte[] { 0, 0, 0, 0, 0, 0, 0 } },
            { "pl14", new byte[] { 0, 0, 0, 0, 0 } },
            { "pl15", new byte[] { 0, 0, 0, 0, 0 } },
            { "pl16", new byte[] { 0, 0, 0 } },
            { "pl17", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { "pl18", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 } },
            { "pl19", new byte[] { 4, 4 } },
            { "pl1a", new byte[] { 4, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 4 } },
            { "pl1b", new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 255, 255 } },
            { "pl1d", new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 } },
            { "pl1e", new byte[] { 0, 0, 0, 0, 0 } },
            { "pl1f", new byte[] { 0, 0, 0, 0 } },
            { "pl20", new byte[] { 0, 0, 0, 0 } },
            { "pl21", new byte[] { 0, 0, 0, 0 } },
            { "sh00", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 9, 9 } },
            { "sh01", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 } },
            { "sh02", new byte[] { 9, 9, 9 } },
            { "sh03", new byte[] { 9, 9, 9, 9, 9 } },
            { "sh04", new byte[] { 9, 9, 9, 9 } },
            { "sh05", new byte[] { 9, 9, 9, 9 } },
            { "sh06", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9 } },
            { "sh07", new byte[] { 11, 11, 11, 11, 11, 11, 11 } },
            { "sh08", new byte[] { 9, 9, 9, 9 } },
            { "sh09", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 11, 11, 11, 11, 11, 9 } },
            { "sh0a", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 9, 9 } },
            { "sh0b", new byte[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } },
            { "sh0c", new byte[] { 10, 10, 10, 10 } },
            { "sh0d", new byte[] { 10, 10, 10, 10, 10 } },
            { "sh0e", new byte[] { 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 10 } },
            { "sh0f", new byte[] { 10, 10, 10, 10 } },
            { "sh10", new byte[] { 10, 10, 10, 10 } },
            { "sh11", new byte[] { 10, 10, 10, 10, 10 } },
            { "sh12", new byte[] { 10, 10, 10, 10, 10 } },
            { "sh13", new byte[] { 10, 10, 10, 10 } },
            { "sh14", new byte[] { 10, 10, 10, 10 } },
            { "sh15", new byte[] { 10, 10, 10 } },
            { "sh16", new byte[] { 10, 10, 10, 10 } },
            { "sh17", new byte[] { 10, 10, 10 } },
            { "sh18", new byte[] { 10, 10, 10, 10, 10 } },
            { "sh19", new byte[] { 10, 10, 10, 10, 10 } },
            { "sh1a", new byte[] { 10, 10, 10, 10, 10, 10, 10 } },
            { "sh1b", new byte[] { 10, 10, 10, 10, 10 } },
            { "sh1c", new byte[] { 10, 10, 10, 10, 10, 10, 10, 10 } },
            { "sh1d", new byte[] { 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 } },
            { "sh1e", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "sh1f", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "sh20", new byte[] { 10, 10, 10, 10, 10, 10, 10 } },
            { "sh22", new byte[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } },
            { "sh23", new byte[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } },
            { "sh24", new byte[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 } },
            { "sh25", new byte[] { 10, 10, 10, 10 } },
            { "sh26", new byte[] { 10, 10, 10, 10 } },
            { "sh27", new byte[] { 10, 10, 10, 10, 10, 10, 10, 10 } },
            { "sh28", new byte[] { 8, 8, 8, 8, 8, 8, 8, 8, 8 } },
            { "sh29", new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 9, 9 } },
            { "sh2a", new byte[] { 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 255 } },
            { "sh2e", new byte[] { 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 10 } },
            { "sh2f", new byte[] { 10, 10, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 } },
            { "sh30", new byte[] { 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 10 } },
            { "sh31", new byte[] { 10, 10, 10, 10, 10, 10, 11, 11, 11, 11, 11, 10 } },
            { "tr00", new byte[] { 9, 9, 9, 9 } },
            { "tr01", new byte[] { 9, 9, 9 } },
            { "tr02", new byte[] { 9, 9, 9, 9, 9, 9, 9 } },
            { "tr03", new byte[] { 9, 9 } },
            { "tr04", new byte[] { 9, 9 } },
            { "tr05", new byte[] { 9, 9 } },
            { "tr06", new byte[] { 9, 9, 9, 9 } },
            { "tr07", new byte[] { 9, 9, 9, 9 } },
            { "tr08", new byte[] { 9, 9, 9, 9, 9 } },
            { "tr09", new byte[] { 9, 9, 9 } },
            { "tr0a", new byte[] { 9, 9 } },
            { "tr0b", new byte[] { 10, 10, 10, 10, 10, 10 } },
            { "tr0c", new byte[] { 10, 10, 10, 10, 10, 10 } },
            { "tr0d", new byte[] { 10, 10 } },
            { "tr0e", new byte[] { 11, 11, 10, 10 } },
            { "tr0f", new byte[] { 10, 10, 10 } },
            { "tr10", new byte[] { 9, 9, 9, 9, 9, 9, 9 } },
            { "tr11", new byte[] { 11, 11, 11, 10, 10 } },
            { "tr15", new byte[] { 9, 9, 9, 9, 9 } },
            { "tr16", new byte[] { 10, 10, 10, 10, 10, 10 } },
            { "tr18", new byte[] { 10, 10, 10, 10, 10, 10 } },
            { "tr1a", new byte[] { 9, 9 } },
            { "tr1b", new byte[] { 9, 9, 9, 9, 9, 9, 9 } },
            { "tr1c", new byte[] { 9, 9, 9, 9, 9 } },
            { "tr1d", new byte[] { 9, 9 } },
            { "tr1e", new byte[] { 9, 9, 9, 9 } },
            { "tr1f", new byte[] { 9, 9, 9, 9 } },
            { "tr20", new byte[] { 10, 10, 10, 10, 10, 10, 10 } },
            { "tr21", new byte[] { 10, 10, 10 } },
            { "tr22", new byte[] { 9, 9, 9, 9, 9 } }
        };

        public static string GetScene(int roomid)
        {
            for (int stage = 0; stage < ScenePrefixes.Length; stage++)
            {
                if (roomid >= SceneStarts[stage] && roomid < SceneStarts[stage + 1])
                    return ScenePrefixes[stage] + (roomid - SceneStarts[stage]).ToString("x2");
            }

            return string.Empty;
        }

        public static string GetArea(string scene)
        {
            if (string.IsNullOrEmpty(scene) || scene.Length < 2)
                return string.Empty;

            return AreaNames.TryGetValue(scene.Substring(0, 2), out string? area) ? area : string.Empty;
        }

        public static string GetRoom(int roomid)
        {
            return roomid switch
            {
                4 => "Cabin",
                36 => "Hall",
                47 => "Conference Room",
                89 => "Observation Room",
                91 => "Basement 2F",
                104 => "Machine Room",
                112 => "Library",
                114 => "Church",
                136 => "Panel Room",
                142 => "Control Room",
                148 => "Lodgings",
                173 => "Storage",
                _ => string.Empty
            };
        }

        public static string GetFloor(string scene, int cut)
        {
            if (string.IsNullOrEmpty(scene) || !Floors.TryGetValue(scene, out byte[]? floors) || cut < 0 || cut >= floors.Length)
                return string.Empty;

            int floor = floors[cut];
            if (floor < 9)
                return "B" + (9 - floor);
            if (floor < 12)
                return (floor - 8) + "F";
            if (floor == 12)
                return "Roof";

            return string.Empty;
        }
    }
}
