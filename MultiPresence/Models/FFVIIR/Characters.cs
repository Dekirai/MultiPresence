using System;
using System.Collections.Generic;

namespace MultiPresence.Models.FFVIIR
{
    public static class Characters
    {
        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Aerith", "Aerith" },
            { "Aerith%02d", "Aerith" },
            { "Aerith_Toad", "Aerith" },
            { "Barret", "Barret" },
            { "Barret%02d", "Barret" },
            { "Barret_Toad", "Barret" },
            { "Cloud", "Cloud" },
            { "Cloud%02d", "Cloud" },
            { "Cloud_Toad", "Cloud" },
            { "PC0000_00_Cloud_Standard", "Cloud" },
            { "PC0000_00_Cloud_Standard_C", "Cloud" },
            { "PC0000_01_Cloud_PoorDress", "Cloud" },
            { "PC0000_01_Cloud_PoorDress_C", "Cloud" },
            { "PC0000_02_Cloud_OrdinaryDress", "Cloud" },
            { "PC0000_02_Cloud_OrdinaryDress_C", "Cloud" },
            { "PC0000_03_Cloud_GorgeousDress", "Cloud" },
            { "PC0000_03_Cloud_GorgeousDress_C", "Cloud" },
            { "PC0000_04_Cloud_ZackCostume", "Cloud" },
            { "PC0000_04_Cloud_ZackCostume_C", "Cloud" },
            { "PC0001_00_Barret_Standard", "Barret" },
            { "PC0001_00_Barret_Standard_C", "Barret" },
            { "PC0002_00_Tifa_Standard", "Tifa" },
            { "PC0002_00_Tifa_Standard_C", "Tifa" },
            { "PC0002_01_Tifa_PurpleDress", "Tifa" },
            { "PC0002_01_Tifa_PurpleDress_C", "Tifa" },
            { "PC0002_02_Tifa_ChinaDress", "Tifa" },
            { "PC0002_02_Tifa_ChinaDress_C", "Tifa" },
            { "PC0002_03_Tifa_WutaiDress", "Tifa" },
            { "PC0002_03_Tifa_WutaiDress_C", "Tifa" },
            { "PC0003_00_Aerith_Standard", "Aerith" },
            { "PC0003_00_Aerith_Standard_C", "Aerith" },
            { "PC0003_01_Aerith_CheapDress", "Aerith" },
            { "PC0003_01_Aerith_CheapDress_C", "Aerith" },
            { "PC0003_02_Aerith_OrdinaryDress", "Aerith" },
            { "PC0003_02_Aerith_OrdinaryDress_C", "Aerith" },
            { "PC0003_03_Aerith_SexyDress", "Aerith" },
            { "PC0003_03_Aerith_SexyDress_C", "Aerith" },
            { "PC0003_04_Aerith_Dirty", "Aerith" },
            { "PC0003_04_Aerith_Dirty_C", "Aerith" },
            { "PC0004_00_RedXIII_Standard", "Red XIII" },
            { "PC0004_00_RedXIII_Standard_C", "Red XIII" },
            { "PC0005_00_Yuffie_Standard", "Yuffie" },
            { "PC0005_00_Yuffie_Standard_C", "Yuffie" },
            { "PC0005_01_Yuffie_Moogle", "Yuffie" },
            { "PC0005_01_Yuffie_Moogle_C", "Yuffie" },
            { "PC0006_00_Sonon_Standard", "Sonon" },
            { "PC0006_00_Sonon_Standard_C", "Sonon" },
            { "PC0099_01_Toad_Cloud", "Cloud" },
            { "PC0099_01_Toad_Cloud_C", "Cloud" },
            { "PC0099_02_Toad_Barret", "Barret" },
            { "PC0099_02_Toad_Barret_C", "Barret" },
            { "PC0099_03_Toad_Tifa", "Tifa" },
            { "PC0099_03_Toad_Tifa_C", "Tifa" },
            { "PC0099_04_Toad_Aerith", "Aerith" },
            { "PC0099_04_Toad_Aerith_C", "Aerith" },
            { "PC0099_05_Toad_Yuffie", "Yuffie" },
            { "PC0099_05_Toad_Yuffie_C", "Yuffie" },
            { "PC0099_06_Toad_Sonon", "Sonon" },
            { "PC0099_06_Toad_Sonon_C", "Sonon" },
            { "RedXIII", "Red XIII" },
            { "RedXIII%02d", "Red XIII" },
            { "Sonon", "Sonon" },
            { "Sonon%02d", "Sonon" },
            { "Sonon_Toad", "Sonon" },
            { "Tifa", "Tifa" },
            { "Tifa%02d", "Tifa" },
            { "Tifa_Toad", "Tifa" },
            { "Yuffie", "Yuffie" },
            { "Yuffie%02d", "Yuffie" },
            { "Yuffie_Toad", "Yuffie" },
            { "pt_Aerith", "Aerith" },
            { "pt_Barret", "Barret" },
            { "pt_Cloud", "Cloud" },
            { "pt_RedXIII", "Red XIII" },
            { "pt_Sonon", "Sonon" },
            { "pt_Tifa", "Tifa" },
            { "pt_Yuffie", "Yuffie" }
        };

        private static readonly Dictionary<string, string> ParameterKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Aerith", "Aerith" },
            { "Aerith%02d", "Aerith" },
            { "Aerith_Toad", "Aerith" },
            { "Barret", "Barret" },
            { "Barret%02d", "Barret" },
            { "Barret_Toad", "Barret" },
            { "Cloud", "Cloud" },
            { "Cloud%02d", "Cloud" },
            { "Cloud_Toad", "Cloud" },
            { "PC0000_00_Cloud_Standard", "Cloud" },
            { "PC0000_00_Cloud_Standard_C", "Cloud" },
            { "PC0000_01_Cloud_PoorDress", "Cloud" },
            { "PC0000_01_Cloud_PoorDress_C", "Cloud" },
            { "PC0000_02_Cloud_OrdinaryDress", "Cloud" },
            { "PC0000_02_Cloud_OrdinaryDress_C", "Cloud" },
            { "PC0000_03_Cloud_GorgeousDress", "Cloud" },
            { "PC0000_03_Cloud_GorgeousDress_C", "Cloud" },
            { "PC0000_04_Cloud_ZackCostume", "Cloud" },
            { "PC0000_04_Cloud_ZackCostume_C", "Cloud" },
            { "PC0001_00_Barret_Standard", "Barret" },
            { "PC0001_00_Barret_Standard_C", "Barret" },
            { "PC0002_00_Tifa_Standard", "Tifa" },
            { "PC0002_00_Tifa_Standard_C", "Tifa" },
            { "PC0002_01_Tifa_PurpleDress", "Tifa" },
            { "PC0002_01_Tifa_PurpleDress_C", "Tifa" },
            { "PC0002_02_Tifa_ChinaDress", "Tifa" },
            { "PC0002_02_Tifa_ChinaDress_C", "Tifa" },
            { "PC0002_03_Tifa_WutaiDress", "Tifa" },
            { "PC0002_03_Tifa_WutaiDress_C", "Tifa" },
            { "PC0003_00_Aerith_Standard", "Aerith" },
            { "PC0003_00_Aerith_Standard_C", "Aerith" },
            { "PC0003_01_Aerith_CheapDress", "Aerith" },
            { "PC0003_01_Aerith_CheapDress_C", "Aerith" },
            { "PC0003_02_Aerith_OrdinaryDress", "Aerith" },
            { "PC0003_02_Aerith_OrdinaryDress_C", "Aerith" },
            { "PC0003_03_Aerith_SexyDress", "Aerith" },
            { "PC0003_03_Aerith_SexyDress_C", "Aerith" },
            { "PC0003_04_Aerith_Dirty", "Aerith" },
            { "PC0003_04_Aerith_Dirty_C", "Aerith" },
            { "PC0004_00_RedXIII_Standard", "RedXIII" },
            { "PC0004_00_RedXIII_Standard_C", "RedXIII" },
            { "PC0005_00_Yuffie_Standard", "Yuffie" },
            { "PC0005_00_Yuffie_Standard_C", "Yuffie" },
            { "PC0005_01_Yuffie_Moogle", "Yuffie" },
            { "PC0005_01_Yuffie_Moogle_C", "Yuffie" },
            { "PC0006_00_Sonon_Standard", "Sonon" },
            { "PC0006_00_Sonon_Standard_C", "Sonon" },
            { "PC0099_01_Toad_Cloud", "Cloud" },
            { "PC0099_01_Toad_Cloud_C", "Cloud" },
            { "PC0099_02_Toad_Barret", "Barret" },
            { "PC0099_02_Toad_Barret_C", "Barret" },
            { "PC0099_03_Toad_Tifa", "Tifa" },
            { "PC0099_03_Toad_Tifa_C", "Tifa" },
            { "PC0099_04_Toad_Aerith", "Aerith" },
            { "PC0099_04_Toad_Aerith_C", "Aerith" },
            { "PC0099_05_Toad_Yuffie", "Yuffie" },
            { "PC0099_05_Toad_Yuffie_C", "Yuffie" },
            { "PC0099_06_Toad_Sonon", "Sonon" },
            { "PC0099_06_Toad_Sonon_C", "Sonon" },
            { "RedXIII", "RedXIII" },
            { "RedXIII%02d", "RedXIII" },
            { "Sonon", "Sonon" },
            { "Sonon%02d", "Sonon" },
            { "Sonon_Toad", "Sonon" },
            { "Tifa", "Tifa" },
            { "Tifa%02d", "Tifa" },
            { "Tifa_Toad", "Tifa" },
            { "Yuffie", "Yuffie" },
            { "Yuffie%02d", "Yuffie" },
            { "Yuffie_Toad", "Yuffie" },
            { "pt_Aerith", "Aerith" },
            { "pt_Barret", "Barret" },
            { "pt_Cloud", "Cloud" },
            { "pt_RedXIII", "RedXIII" },
            { "pt_Sonon", "Sonon" },
            { "pt_Tifa", "Tifa" },
            { "pt_Yuffie", "Yuffie" }
        };

        private static readonly Dictionary<int, string> NativeKeys = new Dictionary<int, string>
        {
            { 0, "Cloud" },
            { 1, "Barret" },
            { 2, "Tifa" },
            { 3, "Aerith" },
            { 4, "RedXIII" },
            { 5, "Yuffie" },
            { 6, "Sonon" }
        };

        public static string GetName(string key)
        {
            return key != null && Names.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public static string GetParameterKey(string key)
        {
            return key != null && ParameterKeys.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public static string GetParameterKey(int nativeId)
        {
            return NativeKeys.TryGetValue(nativeId, out var value) ? value : string.Empty;
        }

        public static string GetName(int nativeId)
        {
            return GetName(GetParameterKey(nativeId));
        }
    }
}
