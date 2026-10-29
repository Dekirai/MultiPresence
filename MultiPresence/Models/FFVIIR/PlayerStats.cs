namespace MultiPresence.Models.FFVIIR
{
    internal static class PlayerStats
    {
        public static bool TryRead(ulong manager, int characterId, GameSnapshot snapshot, out GameSnapshot result)
        {
            result = snapshot;
            if (manager == 0 || characterId is < 0 or > 6)
            {
                return false;
            }

            var table = Pointer(manager + 0xB6D80);
            var records = table == 0 ? 0 : Pointer(table + 0x3E60);
            if (records == 0)
            {
                return false;
            }

            var record = records + (ulong)characterId * 0x40;
            var data = Hypervisor.ReadArray(record, 0x40, true);
            var level = data[0x20];
            var hp = BitConverter.ToInt32(data, 0x30);
            var maxHp = BitConverter.ToInt32(data, 0x34);
            var mp = BitConverter.ToInt32(data, 0x38);
            var maxMp = BitConverter.ToInt32(data, 0x3C);
            var totalExp = Integer(manager + 0x8C0 + (ulong)characterId * 0x40);
            if (level is < 1 or > 99 || hp is < 0 or > 10000000 || maxHp is < 1 or > 10000000 || mp is < 0 or > 1000000 || maxMp is < 0 or > 1000000 || totalExp < 0)
            {
                return false;
            }

            int? exp = null, maxExp = null, nextExp = null;
            var experience = table + 0x178 + (ulong)(characterId * 0x98 + level - 1) * 4;
            var experienceBase = Integer(experience);
            var experienceNext = level < 99 ? Integer(experience + 4) : experienceBase;
            if (level == 50)
            {
                exp = maxExp = nextExp = 0;
            }
            else if (experienceBase >= 0 && experienceNext >= experienceBase && totalExp >= experienceBase)
            {
                exp = totalExp - experienceBase;
                maxExp = experienceNext - experienceBase;
                nextExp = Math.Max(0, experienceNext - totalExp);
            }

            if (!ReadWeaponStats(manager, characterId, out var sp, out var weaponLevel))
            {
                return false;
            }
            int? maxSp = weaponLevel < 6 && Progression.TryGetWeaponThreshold(snapshot.CharacterKey, weaponLevel, out var threshold) ? threshold : null;
            if (table != Pointer(manager + 0xB6D80) || records != Pointer(table + 0x3E60) || level != Hypervisor.Read<byte>(record + 0x20, true))
            {
                return false;
            }

            result = snapshot with
            {
                Level = level,
                Hp = hp,
                MaxHp = maxHp,
                Mp = mp,
                MaxMp = maxMp,
                Sp = sp,
                MaxSp = maxSp,
                WeaponLevel = weaponLevel,
                Exp = exp,
                MaxExp = maxExp,
                TotalExp = totalExp,
                NextExp = nextExp
            };
            return true;
        }

        private static bool ReadWeaponStats(ulong manager, int characterId, out int sp, out int weaponLevel)
        {
            sp = weaponLevel = 0;
            var weapons = Pointer(manager + 0xB6DC0);
            if (weapons == 0)
            {
                return false;
            }

            var entries = Pointer(weapons + 0x10);
            var count = Integer(weapons + 0x18);
            var capacity = Integer(weapons + 0x1C);
            var bitCount = Integer(weapons + 0x38);
            var flags = Pointer(weapons + 0x30);
            if (entries == 0 || count < 0 || count > capacity || capacity > 256 || count > bitCount || flags == 0 && count > 128)
            {
                return false;
            }

            if (flags == 0)
            {
                flags = weapons + 0x20;
            }

            for (var index = 0; index < count; index++)
            {
                var allocated = Hypervisor.Read<uint>(flags + (ulong)(index >> 5) * 4, true);
                if ((allocated & (1U << (index & 31))) == 0)
                {
                    continue;
                }

                var entry = entries + (ulong)index * 0x1D8;
                if (Hypervisor.Read<byte>(entry + 8, true) != characterId)
                {
                    continue;
                }

                var points = Integer(entry + 0xA0);
                var level = Integer(entry + 0xB4);
                if (points is < 0 or > 1000000 || level is < 1 or > 99)
                {
                    continue;
                }

                if (weapons == Pointer(manager + 0xB6DC0) && entries == Pointer(weapons + 0x10) && count == Integer(weapons + 0x18))
                {
                    sp = points;
                    weaponLevel = level;
                    return true;
                }
                return false;
            }

            return false;
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static int Integer(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }
    }
}
