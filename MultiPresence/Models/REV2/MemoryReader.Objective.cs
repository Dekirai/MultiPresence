namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private sealed record ObjectiveAddresses(uint Save, uint Main, uint Struggle, uint LittleMiss, Dictionary<int, string> Labels);

        private static ObjectiveAddresses? ResolveObjective(GameCode code, string executable)
        {
            int match = code.FindOptional("A1 ?? ?? ?? ?? 8B 49 08 89 88 ?? ?? ?? ?? 33 C0 C2 08 00 83 F8 03 75 ?? A1 ?? ?? ?? ?? 8B 49 08 89 88 ?? ?? ?? ?? 33 C0 C2 08 00 83 F8 04 75 ?? A1 ?? ?? ?? ?? 8B 49 08 89 88 ?? ?? ?? ?? 33 C0 C2 08 00");
            if (match < 0) return null;
            uint save = code.Global(match, 1);
            if (code.Operand(match, 25) != save || code.Operand(match, 49) != save) return null;
            return new ObjectiveAddresses(save, code.Operand(match, 10), code.Operand(match, 34), code.Operand(match, 58),
                GameMessages.ReadLabels(executable, "m38_sys_destination_eng.gmd", "38_camp_"));
        }

        private static string ReadObjective(ObjectiveAddresses? found, int? mode)
        {
            if (found == null || !Hypervisor.TryRead(found.Save, out uint save, true) || save == 0) return "";
            uint field = mode switch { 3 => found.Struggle, 4 => found.LittleMiss, _ => found.Main };
            return Hypervisor.TryRead((ulong)save + field, out int id, true) ? found.Labels.GetValueOrDefault(id, "") : "";
        }
    }
}
