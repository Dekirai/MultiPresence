using System.Globalization;

namespace MultiPresence.Models.SHF
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadObjective(GameSnapshot snapshot, ulong world, ulong controller, ulong pawn)
        {
            var stateField = Field(pawn, "PlayerState");
            if (stateField == 0) return snapshot;

            ulong state = Pointer(pawn + (ulong)stateField);
            var missionField = state == 0 ? 0 : Field(state, "LastMissionText");
            if (missionField == 0) return snapshot;

            ulong slot = state + (ulong)missionField;
            ulong text = Pointer(slot);
            if (text == 0 || !IsMissionStringTableText(text) ||
                !Hypervisor.TryRead<ulong>(text + 0x10, out var reference, true) || reference == 0 ||
                !Hypervisor.TryRead<ulong>(reference + 0x18, out var keyText, true) || keyText == 0 ||
                FName(reference + 0x10) != "/Game/StringTable/D_MissionHint_StringTable.D_MissionHint_StringTable")
                return snapshot;

            string key = Hypervisor.ReadString(keyText, 96, true, isUnicode: true);
            string objective = Missions.Get(key);
            if (objective.Length == 0 && Hypervisor.TryRead<ulong>(reference + 0x40, out var localized, true) && localized != 0 &&
                Hypervisor.TryRead<int>(localized + 0x10, out var length, true) && length is > 1 and <= 512 &&
                Hypervisor.TryRead<ulong>(localized + 8, out var value, true) && value != 0)
                objective = Hypervisor.ReadString(value, length * 2, true, isUnicode: true);

            if (objective.Length == 0 || Pointer(slot) != text || Pointer(pawn + (ulong)stateField) != state)
                return snapshot;

            string name = objective;
            int? current = null, maximum = null;
            int start = objective.LastIndexOf(" (", StringComparison.Ordinal);
            int slash = objective.LastIndexOf('/');
            if (start >= 0 && slash > start + 2 && objective.EndsWith(')') &&
                int.TryParse(objective.AsSpan(start + 2, slash - start - 2), NumberStyles.None, CultureInfo.InvariantCulture, out int count) &&
                int.TryParse(objective.AsSpan(slash + 1, objective.Length - slash - 2), NumberStyles.None, CultureInfo.InvariantCulture, out int target) &&
                count <= target && target > 0)
            {
                name = objective[..start];
                current = count;
                maximum = target;
            }
            return snapshot with { Objective = objective, ObjectiveName = name, ObjectiveCurrent = current, ObjectiveMax = maximum };
        }

        private static bool IsMissionStringTableText(ulong text)
        {
            // FTextHistory_StringTableEntry returns 11 from GetType(). Empty text uses a different history layout.
            return Hypervisor.TryRead<ulong>(text, out var vtable, true) && vtable != 0 &&
                Hypervisor.TryRead<ulong>(vtable + 0x58, out var getType, true) && getType != 0 &&
                Hypervisor.TryRead<uint>(getType, out var code, true) && (code & 0xFFFFFF) == 0xC30BB0;
        }
    }
}
