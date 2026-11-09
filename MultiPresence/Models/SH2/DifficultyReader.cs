namespace MultiPresence.Models.SH2
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadDifficulties(GameSnapshot snapshot, ulong instance)
        {
            ulong manager = FindSaveManager(instance);
            ulong provider = ObjectProperty(manager, "_Provider");
            ulong save = ObjectProperty(provider, "CurrentGameData");
            if (!IsA(save, "SHSaveGame")) return snapshot;

            int offset = Field(save, "Data");
            if (offset == 0) return snapshot;

            // Both SHGameplayStatics difficulty getters read these adjacent bytes in FSHSaveData.
            if (!Hypervisor.TryRead<ushort>(save + (ulong)offset + 0x118, out ushort settings, true) ||
                ObjectProperty(provider, "CurrentGameData") != save) return snapshot;

            return snapshot with
            {
                CombatDifficulty = DifficultyName(settings & 0xFF),
                PuzzleDifficulty = DifficultyName(settings >> 8)
            };
        }

        private static ulong FindSaveManager(ulong instance)
        {
            if (instance == 0) return 0;

            // UGameInstance's subsystem collection maps each subsystem class to its instance.
            ulong map = instance + 0x108;
            int slots = Hypervisor.Read<int>(map + 8, true);
            int capacity = Hypervisor.Read<int>(map + 12, true);
            int bits = Hypervisor.Read<int>(map + 0x28, true);
            if (slots is < 1 or > 256 || slots > capacity || bits != slots) return 0;
            ulong entries = Pointer(map);
            ulong flags = Pointer(map + 0x20);
            if (entries == 0 || (flags == 0 && slots > 128)) return 0;
            if (flags == 0) flags = map + 0x10;

            var occupied = Hypervisor.ReadArray(flags, (slots + 7) / 8, true);
            var pairs = Hypervisor.ReadArray(entries, slots * 24, true);
            for (int index = 0; index < slots; index++)
            {
                if ((occupied[index / 8] & (1 << (index % 8))) == 0) continue;
                ulong type = BitConverter.ToUInt64(pairs, index * 24);
                if (type == 0 || Name(type) != "SaveManager") continue;
                ulong manager = BitConverter.ToUInt64(pairs, index * 24 + 8);
                if (manager != 0 && Pointer(manager + 0x10) == type && Pointer(manager + 0x20) == instance) return manager;
            }
            return 0;
        }

        private static string DifficultyName(int value) => value switch
        {
            0 => "Light",
            1 => "Standard",
            2 => "Hard",
            _ => ""
        };
    }
}
