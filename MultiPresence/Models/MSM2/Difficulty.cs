namespace MultiPresence.Models.MSM2
{
    internal static partial class MemoryReader
    {
        private static void ReadDifficulty(GameSnapshot snapshot)
        {
            var save = FindSaveData(0x56B5C745);
            int difficulty = Int(save + 0xB0);
            if (difficulty < 0 || difficulty > 4)
            {
                throw Invalid("invalid difficulty");
            }

            snapshot.DifficultyId = difficulty;
            snapshot.Difficulty = GameText.GetDifficulty(difficulty);
        }

        private static bool ReadNewGamePlus()
        {
            var save = FindSaveData(0x117DE3E8);
            byte value = Byte(save + 0x182);
            if (value > 1)
            {
                throw Invalid("invalid New Game Plus state");
            }

            return value != 0;
        }

        private static ulong FindSaveData(uint key)
        {
            var manager = Pointer(saveManagerSlot);
            if (manager == 0 || Int(manager + 0x98) == 0)
            {
                throw Invalid("save data is unavailable");
            }

            var image = Pointer(manager + 0x30);
            if (image == 0)
            {
                throw Invalid("save image is unavailable");
            }

            var keys = Pointer(image + 0x28);
            var entries = Pointer(image + 0x30);
            int capacity = Int(image + 0x3C);
            int stride = Int(image + 0x44);
            if (keys == 0 || entries == 0 || capacity < 1 || capacity > 4096 || stride < 0x40 || stride > 0x400)
            {
                throw Invalid("invalid save table");
            }

            for (int index = 0; index < capacity; index++)
            {
                if (UInt(keys + (ulong)index * 4) != key)
                {
                    continue;
                }

                var save = Pointer(entries + (ulong)(index * stride) + 0x38);
                if (save == 0)
                {
                    throw Invalid("save data is unavailable");
                }

                return save;
            }

            throw Invalid("save data was not found");
        }
    }
}
