namespace MultiPresence.Models.CROC2
{
    internal static partial class MemoryReader
    {
        private static void ReadPlayerStats(GameSnapshot snapshot)
        {
            var data = Bytes(statsAddress, 0xD4);
            var mode = BitConverter.ToInt32(data, 0x60);
            snapshot.MaxHealth = BitConverter.ToInt32(data, 0);
            snapshot.Health = BitConverter.ToInt32(data, 4);
            snapshot.BonusHearts = mode == 2 ? BitConverter.ToInt32(data, 8) : 0;
            snapshot.Crystals = BitConverter.ToInt32(data, 0x14);
            snapshot.LevelCrystals = Int(levelCrystalsAddress);
            snapshot.GoldenGobbos = BitConverter.ToInt32(data, 0xA8);
            snapshot.Keys = BitConverter.ToInt32(data, 0xBC);
            snapshot.ClockworkGobbos = BitConverter.ToInt32(data, 0xD0);
            snapshot.GameplayMode = mode switch
            {
                0 => "Classic 1999",
                1 => "Classic 2000",
                2 => "Remastered",
                _ => throw Invalid("gameplay mode")
            };

            if (snapshot.MaxHealth is < 1 or > 9 || snapshot.Health < 0 || snapshot.Health > snapshot.MaxHealth || snapshot.BonusHearts is < 0 or > 5)
            {
                throw Invalid("health");
            }

            if (snapshot.Crystals is < 0 or > 9999 || snapshot.LevelCrystals is < 0 or > 9999 || snapshot.GoldenGobbos is < 0 or > 1000 ||
                snapshot.Keys is < 0 or > 1000 || snapshot.ClockworkGobbos is < 0 or > 1000)
            {
                throw Invalid("inventory");
            }
        }
    }
}
