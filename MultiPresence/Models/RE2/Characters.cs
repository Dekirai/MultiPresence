// SurvivorType IDs: SpeedrunTooling/SRTPluginProviderRE2, commit
// e2a24e221e7a9afa1d2802035803c853671ec5b5, PlayerManager.cs.
// This is the controlled character, independent of the campaign scenario.
namespace MultiPresence.Models.RE2
{
    public static class Characters
    {
        public static string GetName(int id)
        {
            return id switch
            {
                -1 => "None",
                0 => "Leon",
                1 => "Claire",
                2 => "Ada",
                3 => "Sherry",
                4 => "Hunk",
                5 => "Tofu",
                6 => "Kendo",
                12 => "Ghost",
                20 => "Katherine",
                _ => $"Unknown character ({id})",
            };
        }

        // MainFlowManager.ScenarioType; independent of the currently controlled survivor.
        public static string GetScenario(int id)
        {
            return id switch
            {
                0 => "Leon A",
                1 => "Claire A",
                2 => "Leon B",
                3 => "Claire B",
                4 => "The 4th Survivor",
                5 => "The Tofu Survivor",
                12 => "None",
                _ => $"Unknown scenario ({id})",
            };
        }
    }
}
