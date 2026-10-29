using System;
using System.Collections.Generic;

namespace MultiPresence.Models.RE8
{
    public static class Modes
    {
        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>
        {
            { "Chapter", "Main Story" },
            { "DLCRoot_GE/DLC05/DLC05_0/ChapterTPSRoot/ChapterTPS", "Main Story" },
            { "DLCRoot_GE/DLC05/DLC05_1/MercenariesPlusGE", "The Mercenaries" },
            { "DLCRoot_GE/DLC05/DLC05_2/Chapter10Boot", "Shadows of Rose" },
            { "DLCRoot_GE/DLC05/DLC05_2/Chapter10BootGameIntent", "Shadows of Rose" },
            { "DLCRoot_GE/DLC05/DLC05_2/Chapter10Chapter", "Shadows of Rose" },
            { "MercenariesPlus", "The Mercenaries" }
        };
        public static string GetName(string contextKey)
        {
            return contextKey != null && Names.TryGetValue(contextKey, out var name) ? name : string.Empty;
        }
    }
}
