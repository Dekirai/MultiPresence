using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace MultiPresence.Models.RE3
{
    public static class Difficulties
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 0, "Assisted" },
            { 1, "Standard" },
            { 2, "Hardcore" },
            { 3, "Nightmare" },
            { 4, "Inferno" }
        };
        public static string GetName(int id)
        {
            return Names.GetValueOrDefault(id) ?? string.Empty;
        }
    }
}
