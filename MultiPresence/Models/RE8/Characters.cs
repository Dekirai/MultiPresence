using System;
using System.Collections.Generic;

namespace MultiPresence.Models.RE8
{
    public static class Characters
    {
        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>
        {
            { "pl1000", "Ethan Winters" },
            { "pl2000", "Chris Redfield" },
            { "pl2001", "Chris Redfield" },
            { "pl3000", "Rosemary Winters" },
            { "pl4000", "Karl Heisenberg" },
            { "pl6000", "Alcina Dimitrescu" }
        };
        public static string GetName(string pawnId)
        {
            return pawnId != null && Names.TryGetValue(pawnId, out var name) ? name : string.Empty;
        }
    }
}
