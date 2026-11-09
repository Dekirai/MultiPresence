namespace MultiPresence.Models.SHF
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadVitals(GameSnapshot snapshot, ulong pawn)
        {
            var stateField = Field(pawn, "PlayerState");
            if (stateField == 0) return snapshot;

            ulong playerState = Pointer(pawn + (ulong)stateField);
            if (playerState == 0) return snapshot;

            var setField = Field(playerState, "PlayerAttributeSet");
            if (setField == 0) return snapshot;

            ulong attributes = Pointer(playerState + (ulong)setField);
            if (attributes == 0 || ClassName(attributes) != "NocePlayerAttributeSet") return snapshot;

            var health = ReadVital(attributes, "Health", "MaxHealth");
            var stamina = ReadVital(attributes, "Stamina", "MaxStamina");
            if (Pointer(pawn + (ulong)stateField) != playerState || Pointer(playerState + (ulong)setField) != attributes)
                return snapshot;

            return snapshot with
            {
                Health = health.Current,
                MaxHealth = health.Maximum,
                Stamina = stamina.Current,
                MaxStamina = stamina.Maximum
            };
        }

        private static (float? Current, float? Maximum) ReadVital(ulong attributes, string currentName, string maximumName)
        {
            var currentField = Field(attributes, currentName);
            var maximumField = Field(attributes, maximumName);
            const int currentValue = 0xC;
            if (currentField == 0 || maximumField == 0 ||
                !Hypervisor.TryRead<float>(attributes + (ulong)currentField + currentValue, out float current, true) ||
                !Hypervisor.TryRead<float>(attributes + (ulong)maximumField + currentValue, out float maximum, true) ||
                !float.IsFinite(current) || !float.IsFinite(maximum) || current < 0 || maximum <= 0 || current > maximum)
                return (null, null);

            return (current, maximum);
        }
    }
}
