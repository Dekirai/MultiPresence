using System;
using System.Collections.Generic;

namespace MultiPresence.Models.RE8
{
    public static class MercenaryStages
    {
        private static readonly Dictionary<uint, string> Names = new Dictionary<uint, string>
        {
            { 1011774909u, "The Castle II" },
            { 1366709057u, "The Village II" },
            { 1963986137u, "The Factory II" },
            { 2123825732u, "The Bloody River II" },
            { 2603586502u, "The Mad Village" },
            { 2749924090u, "The Mad Village II" },
            { 2795128765u, "The Village" },
            { 3038060965u, "The Bloody River" },
            { 3767146325u, "The Bloody Village II" },
            { 3841871310u, "The Castle" },
            { 4137088851u, "The Factory" },
            { 607658620u, "The Bloody Village" }
        };
        private static readonly Dictionary<string, string> Keys = new Dictionary<string, string>
        {
            { "Stage_01", "The Village" },
            { "Stage_02", "The Castle" },
            { "Stage_03", "The Factory" },
            { "Stage_04", "The Mad Village" },
            { "Stage_05", "The Village II" },
            { "Stage_06", "The Castle II" },
            { "Stage_07", "The Factory II" },
            { "Stage_08", "The Mad Village II" },
            { "Stage_09", "The Bloody Village" },
            { "Stage_10", "The Bloody River" },
            { "Stage_11", "The Bloody Village II" },
            { "Stage_12", "The Bloody River II" }
        };
        public static string GetName(uint stageHash)
        {
            return Names.GetValueOrDefault(stageHash) ?? string.Empty;
        }

        public static string GetName(string stageKey)
        {
            return stageKey != null && Keys.TryGetValue(stageKey, out var name) ? name : string.Empty;
        }
    }
}
