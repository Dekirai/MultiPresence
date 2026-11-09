namespace MultiPresence.Models.LOKD
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadLocation(GameSnapshot snapshot)
        {
            int chapter = Hypervisor.Read<int>(Address("ChapterIndex"), true);
            if (chapter is >= 0 and < 13)
            {
                snapshot = snapshot with { Chapter = chapter + 1 };
            }

            ulong slot = Address("CurrentLevel");
            if (!Hypervisor.TryRead<ulong>(slot, out var level, true) || level == 0 ||
                !Hypervisor.TryRead<int>(level + 0x90, out var nameId, true) ||
                !Hypervisor.TryRead<ulong>(level + 0x88, out var name, true) || name == 0)
            {
                return snapshot;
            }

            string unit = Hypervisor.ReadString(name, 32, true);
            string location = GameText.English(nameId);
            if (!Hypervisor.TryRead<ulong>(slot, out var current, true) || current != level)
            {
                return snapshot;
            }

            int separator = location.IndexOf(" - ", StringComparison.Ordinal);
            string area = separator < 0 ? location : location[..separator];
            string room = separator < 0 ? "" : location[(separator + 3)..];
            return snapshot with { AreaId = unit, Area = area, Room = room, Location = room.Length == 0 ? area : area + " / " + room };
        }
    }
}
