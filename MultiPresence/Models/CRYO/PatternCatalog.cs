namespace MultiPresence.Models.CRYO
{
    internal sealed record PatternDefinition(string Pattern, int Operand, int Length);

    internal static class PatternCatalog
    {
        internal static readonly Dictionary<string, PatternDefinition> All = new Dictionary<string, PatternDefinition>
        {
            { "Global", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01 48 85 C0 74 ?? 4C 8B 05 ?? ?? ?? ?? 4C 8B 08", 3, 7) },
            { "LevelManager", new PatternDefinition("48 8B 0D ?? ?? ?? ?? 48 8B 89 B8 00 00 00 E8 ?? ?? ?? ?? 33 C9 E8 ?? ?? ?? ?? F3 0F 11 83 90 00 00 00", 3, 7) },
            { "LobbyManager", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01 48 85 C0 74 ?? 0F B6 80 A0 00 00 00 48 83 C4 20", 3, 7) },
            { "LobbyValuesContainer", new PatternDefinition("48 8B 15 ?? ?? ?? ?? 48 8B 82 B8 00 00 00 4C 8B 00 4D 85 C0 74 ?? 41 83 78 68 00 76 ?? 84 C9 75 ??", 3, 7) },
            { "GlobalValues", new PatternDefinition("48 8B 0D ?? ?? ?? ?? 33 ED 89 AC 24 A8 00 00 00 39 A9 E4 00 00 00 75 ?? E8 ?? ?? ?? ?? 33 D2 48 8B CF", 3, 7) },
            { "ClientData", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01 48 85 C0 0F 84 ?? ?? ?? ?? 48 85 DB 0F 84 ?? ?? ?? ??", 3, 7) },
            { "ObjectiveManager", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 09 48 85 C9 74 ?? 48 8B 53 20 45 33 C0 E8 ?? ?? ?? ??", 3, 7) },
            { "NetworkManager", new PatternDefinition("48 8B 0D ?? ?? ?? ?? 83 B9 E4 00 00 00 00 75 ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 74 24 48 48 8B 88 B8 00 00 00", 3, 7) },
        };
    }
}
