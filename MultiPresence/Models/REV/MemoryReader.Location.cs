namespace MultiPresence.Models.REV
{
    internal static partial class MemoryReader
    {
        private sealed record LocationAddresses(uint Room, uint Block, Dictionary<(int Room, int Block), string> Areas);

        private static LocationAddresses? ResolveLocation(GameCode code, string executable)
        {
            int room = code.FindOptional("8B 44 24 0C 8B 94 81 ?? ?? ?? ?? 56 8B 74 24 08 89 16 8B 84 81 ?? ?? ?? ?? 8B 4C 24 0C 89 01");
            int table = code.FindOptional("56 8B 35 ?? ?? ?? ?? 83 C8 FF 33 D2 85 F6 74 ?? 53 8B 5C 24 0C 57 8B 7C 24 14 B9 ?? ?? ?? ?? 55 0F BF 29 3B EB 75 ?? 0F BE 69 02 3B EF 74 ?? 42 83 C1 ??");
            int names = code.FindOptional("48 83 F8 19 75 ?? B8 14 00 00 00 8B 14 85 ?? ?? ?? ?? 8B 86 ?? ?? ?? ?? 6A 03");
            if (room < 0 || table < 0 || names < 0) return null;

            int count = Hypervisor.Read<int>(code.Global(table, 3), true);
            int stride = code.Bytes[table + 50];
            string[] messages = GameMessages.ReadMessages(executable, "cockpit_eng.gmd");
            if (count is < 1 or > 1024 || stride < 8 || messages.Length == 0) return null;

            // The map window resolves the current room to one of 21 ship areas and shows its cockpit message.
            int[] ids = Hypervisor.Read<int>(code.Global(names, 14), 21, true);
            byte[] rows = Hypervisor.ReadArray(code.Global(table, 27), count * stride, true);
            var areas = new Dictionary<(int Room, int Block), string>();
            for (int offset = 0; offset < rows.Length; offset += stride)
            {
                int area = rows[offset + 4];
                int index = area == 26 ? 20 : area - 1;
                if (index is < 0 or > 20 || (uint)ids[index] >= messages.Length || messages[ids[index]].Length == 0) continue;
                areas.TryAdd((BitConverter.ToInt16(rows, offset), (sbyte)rows[offset + 2]), messages[ids[index]]);
            }
            return new LocationAddresses(code.Operand(room, 7), code.Operand(room, 21), areas);
        }

        private static string ReadLocation(LocationAddresses? found, PartyAddresses party, uint player)
        {
            if (found == null || !Hypervisor.TryRead(player + (ulong)party.Stage, out uint stage, true) || stage == 0 ||
                !Hypervisor.TryRead(stage + (ulong)found.Room, out int room, true) ||
                !Hypervisor.TryRead(stage + (ulong)found.Block, out int block, true)) return "";
            return found.Areas.TryGetValue((room, block), out var area) ? area : CampaignLocations.GetStage(room);
        }
    }
}
