namespace MultiPresence.Models.LOKD
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new();
        private static int processId, moduleSize;
        private static ulong moduleBase;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            GameText.Reset();
            processId = moduleSize = 0;
            moduleBase = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            int state = Hypervisor.Read<int>(Address("GameState"), true);
            if (state != 1)
            {
                return new GameSnapshot { State = state == 7 ? "Default" : "Loading" };
            }

            ulong actor = Hypervisor.Read<ulong>(Address("Actor"), true);
            ulong player = Hypervisor.Read<ulong>(Address("PlayerData"), true);
            if (actor == 0 || player == 0)
            {
                return new GameSnapshot { State = "Loading" };
            }

            var snapshot = ReadPlayer(new GameSnapshot { State = "Ingame" });
            if (snapshot.State != "Ingame")
            {
                return snapshot;
            }
            snapshot = ReadLocation(snapshot);
            snapshot = ReadObjective(snapshot);
            if (Hypervisor.Read<int>(Address("GameState"), true) != state ||
                Hypervisor.Read<ulong>(Address("Actor"), true) != actor ||
                Hypervisor.Read<ulong>(Address("PlayerData"), true) != player)
            {
                return new GameSnapshot { State = "Loading" };
            }
            return snapshot;
        }

        private static ulong Address(string key) => addresses[key];
        private static uint ReadUInt(ulong address) => Hypervisor.Read<uint>(address, true);
        private static InvalidDataException Invalid(string field) => new("Defiance Remastered: " + field + ".");
    }
}
