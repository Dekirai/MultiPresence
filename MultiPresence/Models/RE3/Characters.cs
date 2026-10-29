using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace MultiPresence.Models.RE3
{
    public static class Characters
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 0, "Carlos" },
            { 2, "Jill" }
        };
        public static string GetName(int id)
        {
            return Names.GetValueOrDefault(id) ?? string.Empty;
        }
    }
}
