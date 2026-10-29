using System;
using System.Collections.Generic;

namespace MultiPresence.Models.FFVIIR
{
    public static class Chapters
    {
        private static readonly Dictionary<int, string> Names = new Dictionary<int, string>
        {
            { 1, "The Destruction of Mako Reactor 1" },
            { 2, "Fateful Encounters" },
            { 3, "Home Sweet Slum" },
            { 4, "Mad Dash" },
            { 5, "Dogged Pursuit" },
            { 6, "Light the Way" },
            { 7, "A Trap Is Sprung" },
            { 8, "Budding Bodyguard" },
            { 9, "The Town That Never Sleeps" },
            { 10, "Rough Waters" },
            { 11, "Haunted" },
            { 12, "Fight for Survival" },
            { 13, "A Broken World" },
            { 14, "In Search of Hope" },
            { 15, "The Day Midgar Stood Still" },
            { 16, "The Belly of the Beast" },
            { 17, "Deliverance from Chaos" },
            { 18, "Destiny's Crossroads" }
        };

        private static readonly Dictionary<int, string> IntermissionNames = new Dictionary<int, string>
        {
            { 1, "Wutai's Finest" },
            { 2, "Covert Ops" }
        };

        private static readonly Dictionary<int, int> NativeNumbers = new Dictionary<int, int>
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 5 },
            { 6, 6 },
            { 7, 7 },
            { 8, 8 },
            { 9, 9 },
            { 10, 10 },
            { 11, 11 },
            { 12, 12 },
            { 13, 13 },
            { 14, 14 },
            { 15, 15 },
            { 16, 16 },
            { 17, 17 },
            { 18, 18 },
            { 22, 1 },
            { 23, 2 }
        };

        private static readonly Dictionary<int, bool> NativeIntermission = new Dictionary<int, bool>
        {
            { 1, false },
            { 2, false },
            { 3, false },
            { 4, false },
            { 5, false },
            { 6, false },
            { 7, false },
            { 8, false },
            { 9, false },
            { 10, false },
            { 11, false },
            { 12, false },
            { 13, false },
            { 14, false },
            { 15, false },
            { 16, false },
            { 17, false },
            { 18, false },
            { 22, true },
            { 23, true }
        };

        public static string GetName(int chapter, bool intermission = false)
        {
            return (intermission ? IntermissionNames : Names).TryGetValue(chapter, out var value) ? value : string.Empty;
        }

        public static int GetNumber(int nativeId)
        {
            return NativeNumbers.TryGetValue(nativeId, out var value) ? value : 0;
        }

        public static bool IsIntermission(int nativeId)
        {
            return NativeIntermission.TryGetValue(nativeId, out var value) && value;
        }
    }
}
