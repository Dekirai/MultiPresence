namespace MultiPresence.Models.FFXVI
{
    internal static class PlayerStats
    {
        public static bool TryRead(ulong profile, ulong statsManager, ulong currency, ulong scenario, GameSnapshot snapshot, out GameSnapshot result)
        {
            result = snapshot;
            if (profile == 0 || statsManager == 0 || currency == 0 || scenario == 0)
            {
                return false;
            }

            var data = Hypervisor.ReadArray(statsManager + 0x8AF8, 0x28, true);
            if (data[0x20] != 1)
            {
                return false;
            }

            var character = BitConverter.ToUInt32(data, 4);
            var hp = BitConverter.ToInt32(data, 8);
            var maxHp = BitConverter.ToInt32(data, 12);
            var limitBreak = BitConverter.ToInt32(data, 20);
            var maxLimitBreak = BitConverter.ToInt32(data, 24);
            var level = BitConverter.ToInt32(data, 28);
            var difficulty = Hypervisor.Read<int>(profile + 0xCB50, true);
            if (level is < 1 or > 200 || difficulty is < 0 or > 3 || hp is < 0 or > 100000000 || maxHp is < 1 or > 100000000 || limitBreak is < 0 or > 1000000 || maxLimitBreak is < 0 or > 1000000)
            {
                return false;
            }

            var scenarioHandle = Hypervisor.Read<ulong>(scenario + 0x38, true);
            if (!TryReadAlternateCurrency(scenarioHandle, out var alternateCurrency) ||
                !TryReadCurrency(currency + (alternateCurrency ? 0xA928UL : 0x20UL), 1, out var gil) ||
                !TryReadCurrency(currency + 0x20, 3, out var abilityPoints))
            {
                return false;
            }

            if (character != Hypervisor.Read<uint>(statsManager + 0x8AFC, true) || data[0x20] != Hypervisor.Read<byte>(statsManager + 0x8B18, true) || scenarioHandle != Hypervisor.Read<ulong>(scenario + 0x38, true) || difficulty != Hypervisor.Read<int>(profile + 0xCB50, true))
            {
                return false;
            }

            result = snapshot with
            {
                Character = (character & 0xFF000000) == 0x01000000 ? Characters.GetName(character & 0x00FFFFFF) : "",
                Level = level,
                Hp = hp,
                MaxHp = maxHp,
                Gil = gil,
                AbilityPoints = abilityPoints,
                LimitBreak = limitBreak,
                LimitBreakBars = limitBreak / 8400,
                MaxLimitBreakBars = maxLimitBreak / 8400,
                Difficulty = difficulty switch
                {
                    0 => "Easy",
                    1 => "Normal",
                    2 => "Final Fantasy",
                    3 => "Ultimaniac",
                    _ => ""
                }
            };
            return true;
        }

        private static bool TryReadAlternateCurrency(ulong handle, out bool alternateCurrency)
        {
            alternateCurrency = false;
            if (handle == 0)
            {
                return true;
            }

            var kind = Hypervisor.Read<ulong>(handle, true);
            if (kind == 0 || (kind & 0x8000000000000000) != 0)
            {
                return true;
            }

            if (kind > 3)
            {
                return false;
            }

            var row = Hypervisor.Read<ulong>(handle + 8, true);
            if (row == 0)
            {
                return false;
            }

            var offset = Hypervisor.Read<int>(row + (kind == 1 ? 4UL : kind == 2 ? 8UL : 16UL), true);
            var record = (long)row + offset;
            if (record <= 0)
            {
                return false;
            }

            var value = Hypervisor.Read<byte>((ulong)record + 0x35, true);
            if (value > 1 || kind != Hypervisor.Read<ulong>(handle, true) || row != Hypervisor.Read<ulong>(handle + 8, true))
            {
                return false;
            }

            alternateCurrency = value != 0;
            return true;
        }

        private static bool TryReadCurrency(ulong address, int id, out int amount)
        {
            amount = 0;
            var count = Hypervisor.Read<ulong>(address, true);
            if (count > 300)
            {
                return false;
            }

            var data = Hypervisor.ReadArray(address + 8, (int)count * 16, true);
            for (var index = 0; index < (int)count; index++)
            {
                if (BitConverter.ToInt32(data, index * 16) == id)
                {
                    amount = BitConverter.ToInt32(data, index * 16 + 4);
                    break;
                }
            }

            return amount is >= 0 and <= 999999999 && count == Hypervisor.Read<ulong>(address, true);
        }
    }
}
