namespace MultiPresence.Models.LOKD
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadObjective(GameSnapshot snapshot)
        {
            var slot = Address("StoryData");
            if (snapshot.State != "Ingame" || !Hypervisor.TryRead<ulong>(slot, out var story, true) || story == 0 ||
                !Hypervisor.TryRead<ulong>(story + 0x744, out var goals, true) ||
                !Hypervisor.TryRead<ulong>(slot, out var current, true) || current != story)
            {
                return snapshot;
            }

            string primary = GameText.English((int)(goals & uint.MaxValue));
            string secondary = GameText.English((int)(goals >> 32));
            string objective = primary;
            if (primary.Length == 0)
            {
                objective = secondary;
            }
            else if (secondary.Length != 0 && primary != secondary)
            {
                objective = primary + " - " + secondary;
            }

            return snapshot with { Objective = objective, PrimaryObjective = primary, SecondaryObjective = secondary };
        }
    }
}
