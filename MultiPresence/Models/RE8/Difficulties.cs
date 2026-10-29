using System;
using System.Collections.Generic;

namespace MultiPresence.Models.RE8
{
    public static class Difficulties
    {
        private static readonly Dictionary<uint, string> Names = new Dictionary<uint, string>
        {
            { 1948795948u, "Standard" },
            { 2081395632u, "Casual" },
            { 3214231311u, "Village of Shadows" },
            { 4054003385u, "Hardcore" }
        };
        public static string GetName(uint difficultyHash)
        {
            return Names.GetValueOrDefault(difficultyHash) ?? string.Empty;
        }
    }
}
