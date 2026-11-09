namespace MultiPresence.Models.LOKD
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadPlayer(GameSnapshot snapshot)
        {
            var player = Hypervisor.Read<ulong>(Address("PlayerData"), true);
            var actor = Hypervisor.Read<ulong>(Address("Actor"), true);
            if (player == 0 || actor == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var attributes = Hypervisor.Read<ulong>(actor + 0x310, true);
            if (attributes == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var data = Hypervisor.ReadArray(player, 0xB04, true);
            var healthTier = BitConverter.ToInt16(data, 0x3C4);
            var health = BitConverter.ToSingle(data, 0x3C8);
            var healthConfig = Hypervisor.ReadArray(attributes + 0x118, 8, true);
            var maximum = (long)BitConverter.ToInt32(healthConfig) + (long)healthTier * BitConverter.ToInt32(healthConfig, 4);
            var totalXp = BitConverter.ToInt32(data, 0xB00);
            var thresholds = Hypervisor.Read<ushort>(attributes + 0x550, 5, true);
            var realm = BitConverter.ToInt32(data, 0x410);

            if (Hypervisor.Read<ulong>(Address("PlayerData"), true) != player ||
                Hypervisor.Read<ulong>(Address("Actor"), true) != actor ||
                Hypervisor.Read<ulong>(actor + 0x310, true) != attributes || data[0xAEA] > 1 || realm is not (1 or 2))
            {
                return new GameSnapshot() { State = "Loading" };
            }

            var validHealth = float.IsFinite(health) && maximum is > 0 and <= int.MaxValue;
            int level = -1;
            int xp = -1;
            if (totalXp >= 0 && thresholds.All(value => value > 0))
            {
                Array.Sort(thresholds);
                level = thresholds.Count(value => totalXp >= value);
                xp = level == 0 ? totalXp : totalXp - thresholds[level - 1];
                if (level == thresholds.Length) xp = 0;
            }

            return snapshot with
            {
                Character = data[0xAEA] == 1 ? "Raziel" : "Kain",
                Realm = realm == 2 ? "Spectral Realm" : "Material Realm",
                Reaver = GetReaver(BitConverter.ToInt32(data, 0x400)),
                Health = validHealth ? (int)Math.Clamp(Math.Round((double)health), 0, maximum) : -1,
                MaxHealth = validHealth ? (int)maximum : -1,
                ReaverXp = xp,
                ReaverLevel = level
            };
        }

        private static string GetReaver(int id) => id switch
        {
            1 => "Spectral Reaver",
            2 => "Material Reaver",
            3 => "Dark Reaver",
            4 => "Light Reaver",
            5 => "Air Reaver",
            6 => "Fire Reaver",
            7 => "Water Reaver",
            8 => "Earth Reaver",
            9 or 15 => "Spirit Reaver",
            10 => "Dimension Reaver",
            11 => "Flame Reaver",
            12 => "Time Reaver",
            13 => "Lightning Reaver",
            14 => "Blood Reaver",
            16 => "Amplified Reaver",
            17 => "Sound Reaver",
            18 => "Golden Reaver",
            _ => ""
        };
    }
}
