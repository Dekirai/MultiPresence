using System.Diagnostics.CodeAnalysis;

namespace MultiPresence.Models.CB4
{
    public static class Locations
    {
        private static readonly Dictionary<string, Level> Levels = new Dictionary<string, Level>(StringComparer.OrdinalIgnoreCase)
        {
            { "1-1", new Level("1-1", "Rude Awakening", "N.Sanity Island", "Present Day", "L134_MASTER", false) },
            { "1-2", new Level("1-2", "N.Sanity Peak", "N.Sanity Island", "Present Day", "L045_MASTER", false) },
            { "2-1", new Level("2-1", "A Real Grind", "The Hazardous Wastes", "2084", "L077_MASTER", false) },
            { "2-2", new Level("2-2", "Crash Compactor", "The Hazardous Wastes", "2084", "L062_MASTER", false) },
            { "2-3", new Level("2-3", "Hit the Road", "The Hazardous Wastes", "2084", "L078_MASTER", false) },
            { "2-3B", new Level("2-3B", "Truck Stopped", "The Hazardous Wastes", "2084", "L078_MASTER", true) },
            { "2-BOSS", new Level("2-BOSS", "Stage Dive", "The Hazardous Wastes", "2084", "L092_MASTER", false) },
            { "3-1", new Level("3-1", "Booty Calls", "Salty Wharf", "1717", "L057_MASTER", false) },
            { "3-1B", new Level("3-1B", "Thar He Blows!", "Salty Wharf", "1717", "L057_MASTER", true) },
            { "3-2", new Level("3-2", "Hook, Line, and Sinker", "Salty Wharf", "1717", "L074_MASTER", false) },
            { "3-3", new Level("3-3", "Jetboard Jetty", "Salty Wharf", "1717", "L079_MASTER", false) },
            { "4-1", new Level("4-1", "Give It a Spin", "Tranquility Falls", "1402", "L113_MASTER", false) },
            { "4-1B", new Level("4-1B", "Potion Commotion", "Tranquility Falls", "1402", "L113_MASTER", true) },
            { "4-2", new Level("4-2", "Draggin' On", "Tranquility Falls", "1402", "L071_MASTER", false) },
            { "4-3", new Level("4-3", "Off-Balance", "Tranquility Falls", "1402", "L080_Master", false) },
            { "4-BOSS", new Level("4-BOSS", "Trouble Brewing", "Tranquility Falls", "1402", "L093_MASTER", false) },
            { "5-1", new Level("5-1", "Off Beat", "Mosquito Marsh", "A Few Days Ago", "L127_MASTER", false) },
            { "5-2", new Level("5-2", "Home Cookin'", "Mosquito Marsh", "A Few Days Ago", "L068_MASTER", false) },
            { "5-3", new Level("5-3", "Run It Bayou", "Mosquito Marsh", "A Few Days Ago", "L082_MASTER", false) },
            { "5-3B", new Level("5-3B", "No Dillo Dallying", "Mosquito Marsh", "A Few Days Ago", "L082_MASTER", true) },
            { "6-1", new Level("6-1", "Snow Way Out", "The 11th Dimension", "1954", "L038_MASTER", false) },
            { "6-1B", new Level("6-1B", "Ship Happens", "The 11th Dimension", "1954", "L038_MASTER", true) },
            { "6-2", new Level("6-2", "Stay Frosty", "The 11th Dimension", "1954", "L083_MASTER", false) },
            { "6-3", new Level("6-3", "Bears Repeating", "The 11th Dimension", "1954", "L084_MASTER", false) },
            { "6-3B", new Level("6-3B", "Building Bridges", "The 11th Dimension", "1954", "L084_MASTER", true) },
            { "6-BOSS", new Level("6-BOSS", "4th Time's a Charm", "The 11th Dimension", "1954", "L094_MASTER", false) },
            { "7-1", new Level("7-1", "Blast to the Past", "Eggipus Dimension", "88 Million B.C.B", "L059_MASTER", false) },
            { "7-2", new Level("7-2", "Fossil Fueled", "Eggipus Dimension", "88 Million B.C.B", "L076_MASTER", false) },
            { "7-3", new Level("7-3", "Dino Dash", "Eggipus Dimension", "88 Million B.C.B", "L088_MASTER", false) },
            { "7-3B", new Level("7-3B", "Rock Blocked", "Eggipus Dimension", "88 Million B.C.B", "L088_MASTER", true) },
            { "8-1", new Level("8-1", "Out for Launch", "Bermugula's Orbit", "???", "L087_MASTER", false) },
            { "8-1B", new Level("8-1B", "Shipping Error", "Bermugula's Orbit", "???", "L087_MASTER", true) },
            { "8-2", new Level("8-2", "Stowing Away", "Bermugula's Orbit", "???", "L085_MASTER", false) },
            { "8-3", new Level("8-3", "Crash Landed", "Bermugula's Orbit", "???", "L086_MASTER", false) },
            { "8-BOSS", new Level("8-BOSS", "A Hole in Space", "Bermugula's Orbit", "???", "L095_MASTER", false) },
            { "9-1", new Level("9-1", "Food Run", "The Sn@xx Dimension", "3023", "L089_MASTER", false) },
            { "9-2", new Level("9-2", "Rush Hour", "The Sn@xx Dimension", "3023", "L090_MASTER", true) },
            { "9-3B", new Level("9-3B", "The Crate Escape", "The Sn@xx Dimension", "3023", "L047_MASTER", true) },
            { "10-1", new Level("10-1", "Nitro Processing", "Cortex Island", "1996", "L049_MASTER", false) },
            { "10-2", new Level("10-2", "Toxic Tunnels", "Cortex Island", "1996", "L036_MASTER", false) },
            { "10-3", new Level("10-3", "Cortex Castle", "Cortex Island", "1996", "L091_MASTER", false) },
            { "10-3B", new Level("10-3B", "Seeing Double", "Cortex Island", "1996", "L091_MASTER", true) },
            { "10-BOSS", new Level("10-BOSS", "The Past Unmasked", "Cortex Island", "1996", "L096_MASTER", false) },
            { "FB-1", new Level("FB-1", "Subject #218", "Flashback Tapes", "", "L172_MASTER", false) },
            { "FB-2", new Level("FB-2", "Navigational Awareness", "Flashback Tapes", "", "L104_MASTER", false) },
            { "FB-3", new Level("FB-3", "Wumpa Affinity Test", "Flashback Tapes", "", "L101_MASTER", false) },
            { "FB-4", new Level("FB-4", "Bandicoot vs Wombat Eval", "Flashback Tapes", "", "L102_MASTER", false) },
            { "FB-5", new Level("FB-5", "Outfit Durability Exam", "Flashback Tapes", "", "L105_MASTER", false) },
            { "FB-6", new Level("FB-6", "N. Brio Superior Study", "Flashback Tapes", "", "L142_MASTER", false) },
            { "FB-7", new Level("FB-7", "Diary Entry: 30th May, 1996", "Flashback Tapes", "", "L108_MASTER", false) },
            { "FB-8", new Level("FB-8", "Team Health Analysis", "Flashback Tapes", "", "L103_MASTER", false) },
            { "FB-9", new Level("FB-9", "Subject #272_final_b", "Flashback Tapes", "", "L110_MASTER", false) },
            { "FB-10", new Level("FB-10", "Countdown Crate Intro", "Flashback Tapes", "", "L145_MASTER", false) },
            { "FB-11", new Level("FB-11", "Cortex Vortex Pre-check", "Flashback Tapes", "", "L143_MASTER", false) },
            { "FB-12", new Level("FB-12", "Subject: Perameles Coco", "Flashback Tapes", "", "L139_MASTER", false) },
            { "FB-13", new Level("FB-13", "N. Gin N.Terview", "Flashback Tapes", "", "L144_MASTER", false) },
            { "FB-14", new Level("FB-14", "Nitro Bounce Crate Test", "Flashback Tapes", "", "L147_MASTER", false) },
            { "FB-15", new Level("FB-15", "N. Gin's N.Genious N.Bouncers", "Flashback Tapes", "", "L146_MASTER", false) },
            { "FB-16", new Level("FB-16", "Mammal Manipulation", "Flashback Tapes", "", "L109_MASTER", false) },
            { "FB-17", new Level("FB-17", "Replacement Considerations", "Flashback Tapes", "", "L106_MASTER", false) },
            { "FB-18", new Level("FB-18", "Overall Fitness Exam", "Flashback Tapes", "", "L149_MASTER", false) },
            { "FB-19", new Level("FB-19", "Tech Check", "Flashback Tapes", "", "L150_MASTER", false) },
            { "FB-20", new Level("FB-20", "Resilience Evaluation", "Flashback Tapes", "", "L148_MASTER", false) },
            { "FB-21", new Level("FB-21", "The Gauntlet", "Flashback Tapes", "", "L107_MASTER", false) }
        };

        public static bool TryGet(string id, [NotNullWhen(true)] out Level? level)
        {
            return Levels.TryGetValue(id ?? string.Empty, out level);
        }

        public static bool TryGetForMap(string map, bool buddy, [NotNullWhen(true)] out Level? level)
        {
            level = null;
            foreach (Level entry in Levels.Values)
            {
                if (string.Equals(entry.Map, map, StringComparison.OrdinalIgnoreCase) && entry.Buddy == buddy)
                {
                    level = entry;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetForMap(string map, [NotNullWhen(true)] out Level? level)
        {
            level = null;
            foreach (Level entry in Levels.Values)
            {
                if (string.Equals(entry.Map, map, StringComparison.OrdinalIgnoreCase))
                {
                    if (level != null)
                    {
                        level = null;
                        return false;
                    }
                    level = entry;
                }
            }
            return level != null;
        }

        public class Level
        {
            public string Id { get; }
            public string Name { get; }
            public string World { get; }
            public string WorldTime { get; }
            public string Map { get; }
            public bool Buddy { get; }

            public Level(string id, string name, string world, string worldTime, string map, bool buddy)
            {
                Id = id;
                Name = name;
                World = world;
                WorldTime = worldTime;
                Map = map;
                Buddy = buddy;
            }
        }
    }
}
