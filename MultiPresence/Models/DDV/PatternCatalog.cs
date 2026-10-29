namespace MultiPresence.Models.DDV
{
    internal sealed record PatternDefinition(string Pattern, int Operand, int Length);

    internal static class PatternCatalog
    {
        internal static readonly Dictionary<string, PatternDefinition> All = new Dictionary<string, PatternDefinition>
        {
            { "SystemRoot", new PatternDefinition("48 8B 0D ?? ?? ?? ?? 83 B9 E4 00 00 00 00 75 ?? E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 89 7C 24 70 0F 29 74 24 50 0F 29 7C 24 40", 3, 7) },
            { "BaseUiRoot", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 90 B8 00 00 00 48 8B 0A 48 85 C9 74 ?? 48 8B 11 48 8B 82 78 06 00 00", 3, 7) },
            { "NewStartMenu", new PatternDefinition("48 8B 0D ?? ?? ?? ?? 83 B9 E4 00 00 00 00 75 ?? E8 ?? ?? ?? ?? 33 C9 E8 ?? ?? ?? ?? 48 85 C0 74 ?? 48 8B 80 48 01 00 00 48 85 C0 74 ?? 48 8B 40 50", 3, 7) },
            { "StartMenu", new PatternDefinition("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 09 48 85 C9 0F 84 ?? ?? ?? ?? 45 33 C0 33 D2 E8 ?? ?? ?? ?? 48 8B 5F 70", 3, 7) },
        };
    }
}
