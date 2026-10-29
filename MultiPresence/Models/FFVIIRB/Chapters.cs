using System;
using System.Collections.Generic;
using System.Globalization;

namespace MultiPresence.Models.FFVIIRB
{
    public static class Chapters
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 1, "Fall of a Hero" },
            { 10, "Watcher of the Vale" },
            { 11, "The Long Shadow of Shinra" },
            { 12, "A Golden Key" },
            { 13, "Where Angels Fear to Tread" },
            { 14, "End of the World" },
            { 2, "A New Journey Begins" },
            { 3, "Deeper into Darkness" },
            { 4, "Dawn of a New Era" },
            { 5, "Blood in the Water" },
            { 6, "Fool's Paradise" },
            { 7, "Those Left Behind" },
            { 8, "All That Glitters" },
            { 9, "The Planet Stirs" }
        };
        public static string GetName(int chapter)
        {
            return Names.TryGetValue(chapter, out var value) ? value : string.Empty;
        }
    }
}
