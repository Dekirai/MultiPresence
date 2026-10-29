namespace MultiPresence.Models.LR
{
    internal sealed record PatternDefinition(string Pattern, int Operand, int Length);

    internal static class PatternCatalog
    {
        internal static readonly Dictionary<string, PatternDefinition> All = new Dictionary<string, PatternDefinition>
        {
            { "PlayerSave", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 0F 11 41 24 89 51 34 48 85 F6 0F 84 ?? ?? ?? ?? 83 7E 40 02", 3, 7) },
            { "LobbyManager", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 09 48 85 C9 74 ?? 48 8B 89 F8 00 00 00 48 85 C9", 3, 7) },
            { "GameManager", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01 48 85 C0 0F 84 ?? ?? ?? ?? 48 8B 40 38 48 85 C0 0F 84 ?? ?? ?? ?? 83 78 18 01", 3, 7) },
            { "GameValues", new PatternDefinition("48 8B 0D ?? ?? ?? ?? 48 8B 81 B8 00 00 00 4C 8B 00 4D 85 C0 0F 84 ?? ?? ?? ?? 49 8B 50 38 48 85 D2 0F 84 ?? ?? ?? ?? 49 8B 48 40", 3, 7) },
            { "NetworkManager", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 49 10 48 85 C9 74 ?? 33 D2 E8 ?? ?? ?? ?? 33 C9", 3, 7) },
            { "SceneLoader", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 19 33 C9 E8 ?? ?? ?? ?? 48 85 DB 74 ?? 45 33 C0 48 8B D0 48 8B CB E8 ?? ?? ?? ?? 48 8B 74 24 38", 3, 7) },
            { "ObjectiveManager", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 01 48 85 C0 0F 84 ?? ?? ?? ?? 48 8B 48 50 48 85 C9 0F 84 ?? ?? ?? ?? 48 8B 15 ?? ?? ?? ?? E8 ?? ?? ?? ??", 3, 7) },
        };
    }
}
