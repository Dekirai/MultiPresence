namespace MultiPresence.Models.TK8
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadScore(GameSnapshot snapshot, ulong world, ulong instance)
        {
            ulong battle = BattleManager("ScoreKey");
            ulong settings = BattleManager("StageKey");
            if (battle == 0 || settings == 0 || !Hypervisor.TryRead<int>(settings + 0x60, out int target, true) || target is < 1 or > 99 ||
                !Hypervisor.TryRead<int>(battle + 0x300, out int round, true) || round < 1)
                return snapshot;

            // The two player records carry rounds won. The active round number is stored separately.
            if (!Hypervisor.TryRead<int>(battle + 0x40, out int left, true) ||
                !Hypervisor.TryRead<int>(battle + 0x1A0, out int right, true) || left < 0 || left > target || right < 0 || right > target ||
                BattleManager("ScoreKey") != battle || Hypervisor.Read<int>(battle + 0x300, true) != round ||
                Hypervisor.Read<int>(battle + 0x40, true) != left || Hypervisor.Read<int>(battle + 0x1A0, true) != right)
                return snapshot;

            return snapshot with { LeftRounds = left, RightRounds = right, RoundsToWin = target, CurrentRound = round };
        }

        // The battle registry looks up native components by the integer key in their registered descriptor.
        private static ulong BattleManager(string descriptor)
        {
            ulong registry = Pointer(Address("BattleRegistry"));
            if (registry == 0) return 0;
            ulong map = Pointer(registry + 0x10);
            if (map == 0) return 0;
            ulong sentinel = Pointer(map + 0x100);
            ulong buckets = Pointer(map + 0x110);
            ulong mask = Hypervisor.Read<ulong>(map + 0x128, true);
            if (sentinel == 0 || buckets == 0 || mask > 65535 || (mask & (mask + 1)) != 0 ||
                Pointer(map + 0x118) != buckets + (mask + 1) * 16) return 0;

            uint key = Hypervisor.Read<uint>(Address(descriptor), true);
            ulong bucket = buckets + (mask & key) * 16;
            ulong last = Pointer(bucket);
            ulong node = Pointer(bucket + 8);
            for (int count = 0; node != 0 && node != sentinel && count < 256; count++)
            {
                if (Hypervisor.Read<uint>(node + 0x10, true) == key)
                {
                    ulong component = Pointer(node + 0x18);
                    return Pointer(Address("BattleRegistry")) == registry && Pointer(registry + 0x10) == map ? component : 0;
                }
                if (node == last) break;
                node = Pointer(node + 8);
            }
            return 0;
        }
    }
}
