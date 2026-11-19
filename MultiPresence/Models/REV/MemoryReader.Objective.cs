namespace MultiPresence.Models.REV
{
    internal static partial class MemoryReader
    {
        private sealed record ObjectiveAddresses(uint View, byte Index, byte List, byte Count, uint Entries, byte Message, string[] Labels);

        private static ObjectiveAddresses? ResolveObjective(GameCode code, string executable)
        {
            int view = code.FindOptional("55 56 8B F1 57 8D AE ?? ?? ?? ?? 55 FF 15 ?? ?? ?? ?? A1 ?? ?? ?? ?? 85 C0 74 ?? 8B 0D ?? ?? ?? ?? 85 C9 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C 8B 0D ?? ?? ?? ?? E8");
            int list = code.FindOptional("8B 41 ?? 85 C0 74 ?? 8B 4C 24 04 3B 48 ?? 73 ?? 8B 80 ?? ?? ?? ?? 8B 04 88 85 C0 74 ?? 8B 50 ?? 8B 40 ?? 83 F8 FF 74 ?? 8B 0D ?? ?? ?? ?? 83 BC 81");
            if (view < 0 || list < 0) return null;
            uint purpose = code.Global(view, 19);
            int index = code.Target(view, 64);
            if (code.Operand(view, 29) != purpose || code.Operand(view, 59) != purpose || !code.Matches(index, "8B 41 ?? C3")) return null;

            return new ObjectiveAddresses(purpose, code.Bytes[index + 2], code.Bytes[list + 2], code.Bytes[list + 13],
                code.Operand(list, 18), code.Bytes[list + 31], GameMessages.ReadMessages(executable, "goal_eng.gmd"));
        }

        private static string ReadObjective(ObjectiveAddresses? found)
        {
            // sPurposeView stores the active purpose_list row. Each row points to a message in goal.gmd.
            if (found == null || !Hypervisor.TryRead(found.View, out uint view, true) || view == 0 ||
                !Hypervisor.TryRead(view + (ulong)found.Index, out int index, true) || index < 0 ||
                !Hypervisor.TryRead(view + (ulong)found.List, out uint list, true) || list == 0 ||
                !Hypervisor.TryRead(list + (ulong)found.Count, out int count, true) || index >= count ||
                !Hypervisor.TryRead(list + (ulong)found.Entries, out uint entries, true) || entries == 0 ||
                !Hypervisor.TryRead(entries + (ulong)index * 4, out uint entry, true) || entry == 0 ||
                !Hypervisor.TryRead(entry + (ulong)found.Message, out int message, true)) return "";
            return found.Labels.ElementAtOrDefault(message) ?? "";
        }
    }
}
