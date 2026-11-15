namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private const string EnemyCounterPattern = "A1 ?? ?? ?? ?? FF B0 ?? ?? ?? ?? FF 70 ?? E8 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? 6A 00 6A 00";
        private sealed record EnemyAddresses(uint Raid, uint Kills, uint Target);

        private static EnemyAddresses? ResolveEnemies(GameCode code)
        {
            int match = code.FindOptional(EnemyCounterPattern);
            return match < 0 ? null : new EnemyAddresses(code.Global(match, 1), code.Bytes[match + 13], code.Operand(match, 7));
        }

        private static GameSnapshot ReadEnemies(EnemyAddresses? found, GameSnapshot snapshot)
        {
            if (found == null || !Hypervisor.TryRead<uint>(found.Raid, out var raid, true) || raid == 0 ||
                !Hypervisor.TryRead<int>((ulong)raid + found.Target, out var target, true) || target <= 0 ||
                !Hypervisor.TryRead<int>((ulong)raid + found.Kills, out var kills, true) || kills < 0)
                return snapshot;

            return snapshot with { Kills = kills, EnemyTarget = target };
        }
    }
}
