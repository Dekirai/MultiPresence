namespace MultiPresence.Models.BXP
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadFarm(GameSnapshot snapshot, ulong meta)
        {
            if (meta == 0 || ClassName(meta) != "MetaSaveData") return snapshot;
            ulong resources = Ptr(meta + 0x18);
            if (resources == 0 || ClassName(resources) != "Int32[]" || Value<ulong>(resources + 0x18) < 4) return snapshot;

            var amounts = Hypervisor.Read<int>(resources + 0x20, 4, true);
            if (amounts.Length != 4 || amounts.Any(amount => amount < 0) || Ptr(meta + 0x18) != resources) return snapshot;
            return snapshot with { Gold = amounts[0], Wheat = amounts[1], Wood = amounts[2], Stone = amounts[3] };
        }
    }
}
