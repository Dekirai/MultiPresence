// English region labels joined from the installed game's data and localization tables.
// These identify a world/region; they do not infer a nearby room or camp.
// See SB-provenance.json and THIRD-PARTY-SB.txt.
using System.Collections.Frozen;

namespace MultiPresence.Models.SB
{
    public static class Locations
    {
        public static string GetName(string? mapName)
        {
            string name = NormalizeMapName(mapName);
            return Names.TryGetValue(name, out var english) ? english : name;
        }

        public static bool TryGetName(string? mapName, out string name)
        {
            return Names.TryGetValue(NormalizeMapName(mapName), out name!);
        }

        public static bool IsMenu(string? mapName)
        {
            string name = NormalizeMapName(mapName);
            return name.Equals("Lobby", StringComparison.OrdinalIgnoreCase) || name.Equals("LobbyWorld", StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
            {
                return string.Empty;
            }

            string name = mapName.Trim().Trim('\'');
            int slash = Math.Max(name.LastIndexOf('/'), name.LastIndexOf('\\'));
            if (slash >= 0)
            {
                name = name[(slash + 1)..];
            }

            if (name.EndsWith(".umap", StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^5];
            }

            int dot = name.LastIndexOf('.');
            if (dot >= 0)
            {
                name = name[(dot + 1)..];
            }

            return name;
        }

        private static readonly FrozenDictionary<string, string> Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "AbyssLab", "Abyss Levoire" },
            { "AbyssLab_P", "Abyss Levoire" },
            { "AltesLab", "Altess Levoire" },
            { "AltesLab_P", "Altess Levoire" },
            { "B07", "Spire 4" },
            { "Beach", "Star Descent" },
            { "C05", "Nest" },
            { "DEDA", "Eidos 9" },
            { "DrownedEidosDistrict", "Eidos 7" },
            { "E03_WasteLand_P", "Wasteland" },
            { "E04", "Xion" },
            { "E05_GreatDesert_P", "Great Desert" },
            { "F01", "Eidos 9" },
            { "F02", "Eidos 7" },
            { "G03", "Star Descent" },
            { "MatrixEleven", "Matrix 11" },
            { "MatrixEleven_Demo", "Matrix 11" },
            { "Matrix_XI", "Matrix 11" },
            { "Matrix_XI_Demo_P", "Matrix 11" },
            { "NKBoss", "Moonlit Valley" },
            { "NK_P", "Moonlit Valley" },
            { "Nest", "Nest" },
            { "SpaceElevator", "Spire 4" },
            { "WasteLandA", "Wasteland" },
            { "WasteLandB", "Great Desert" },
            { "Xion", "Xion" }
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }
}
