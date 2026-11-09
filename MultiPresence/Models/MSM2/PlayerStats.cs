namespace MultiPresence.Models.MSM2
{
    internal static partial class MemoryReader
    {
        private static void ReadPlayerStats(GameSnapshot snapshot, ulong data)
        {
            var health = Pointer(data + 0x88);
            var inventory = Pointer(data + 0xC8);
            var configManager = Pointer(data + 0x80);
            var config = Pointer(Pointer(configManager + 0x48) + 0x1278);
            if (health == 0 || inventory == 0 || config == 0)
            {
                throw Invalid("player stats");
            }

            snapshot.Health = Float(health + 0xD0);
            snapshot.MaxHealth = Float(health + 0xA0);
            if (snapshot.Health < 0 || snapshot.MaxHealth is <= 0 or > 100000 || snapshot.Health > snapshot.MaxHealth + 1)
            {
                throw Invalid("health");
            }

            var generalInventory = ResolveHandle(UInt(inventory + 0x2108));
            if (generalInventory == 0)
            {
                throw Invalid("general inventory");
            }

            snapshot.TotalExperience = ReadItemCount(generalInventory, Pointer(config + 0x6C));
            snapshot.SkillPoints = ReadItemCount(generalInventory, Pointer(config + 0x84));
            ReadLevel(snapshot, config);
        }

        private static void ReadLevel(GameSnapshot snapshot, ulong config)
        {
            var normalLevels = Int(config + 0x90);
            var count = Int(config + 0xA0);
            var table = Pointer(config + 0x98);
            if ((table & 0x8000000000000000) != 0)
            {
                table = checked((ulong)((long)config + 0x98 + unchecked((int)table)));
            }

            if (normalLevels is < 1 or > 256 || count < normalLevels || count > 256 || table == 0)
            {
                throw Invalid("level table");
            }

            snapshot.NewGamePlus = ReadNewGamePlus();
            count = snapshot.NewGamePlus ? count : normalLevels;
            var rows = Bytes(table, count * 24);
            var current = -1;
            var previousLevel = 0;
            var previousExperience = -1;
            for (var index = 0; index < count; index++)
            {
                var level = BitConverter.ToInt32(rows, index * 24 + 8);
                var experience = BitConverter.ToInt32(rows, index * 24 + 12);
                if (level <= previousLevel || level > 1000 || experience < 0 || experience <= previousExperience)
                {
                    throw Invalid("level progression");
                }

                if (experience <= snapshot.TotalExperience)
                {
                    current = index;
                }

                previousLevel = level;
                previousExperience = experience;
            }

            if (current < 0)
            {
                throw Invalid("experience");
            }

            snapshot.Level = BitConverter.ToInt32(rows, current * 24 + 8);
            snapshot.UltimateLevel = current >= normalLevels ? snapshot.Level - normalLevels : 0;
            var threshold = BitConverter.ToInt32(rows, current * 24 + 12);
            snapshot.Experience = snapshot.TotalExperience - threshold;
            snapshot.NextLevelExperience = current + 1 < count
                ? BitConverter.ToInt32(rows, (current + 1) * 24 + 12) - threshold
                : 0;
        }

        private static int ReadItemCount(ulong inventory, ulong key)
        {
            var map = inventory + 0x2140;
            var bits = Int(map + 0x10);
            var buckets = Pointer(map + 0x20);
            if (bits is < 0 or > 16 || buckets == 0 || key == 0)
            {
                throw Invalid("inventory map");
            }

            var index = (uint)(key & ((1UL << bits) - 1));
            var visited = new HashSet<uint>();
            var total = 0;
            while (index != uint.MaxValue)
            {
                if (index > 100000 || visited.Count >= 1024 || !visited.Add(index))
                {
                    throw Invalid("inventory buckets");
                }

                var bucket = Bytes(buckets + index * 256UL, 256);
                var count = BitConverter.ToInt32(bucket, 0xF8);
                if (count is < 0 or > 15)
                {
                    throw Invalid("inventory entries");
                }

                for (var entry = 0; entry < count; entry++)
                {
                    if (BitConverter.ToUInt64(bucket, entry * 16) != key)
                    {
                        continue;
                    }

                    var item = BitConverter.ToUInt64(bucket, entry * 16 + 8);
                    var properties = Bytes(item + 0x130, 12);
                    var amount = BitConverter.ToInt32(properties, 8);
                    if (BitConverter.ToUInt64(properties) != key || amount < 0)
                    {
                        throw Invalid("inventory item");
                    }

                    total = checked(total + amount);
                }

                index = BitConverter.ToUInt32(bucket, 0xF0);
            }

            return total;
        }
    }
}
