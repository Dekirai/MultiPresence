namespace MultiPresence.Models.CCFFVII
{
    internal static class PlayerStats
    {
        public static bool TryRead(ulong saveStatus, ulong battleStatus, ulong uiManager, GameSnapshot snapshot, out GameSnapshot result)
        {
            result = snapshot;
            if (saveStatus == 0 || battleStatus == 0 || uiManager == 0)
            {
                return false;
            }

            var hud = Hypervisor.Read<ulong>(uiManager + 0x88, true);
            var status = Hypervisor.Read<ulong>(uiManager + 0xB8, true);
            if (hud == 0 || status == 0)
            {
                return false;
            }

            var source = Hypervisor.Read<byte>(hud + 0x77B, true);
            var flags = Hypervisor.ReadArray(status, 2, true);
            if (source > 1 || flags[0] > 1 || flags[1] > 1)
            {
                return false;
            }

            var saved = source != 0 || flags[0] != 0 && flags[1] == 0;
            if (!saved && Hypervisor.Read<ulong>(hud + 0x88, true) != battleStatus)
            {
                return false;
            }

            var data = Hypervisor.ReadArray(saveStatus, 0xD8, true);
            var level = data[0x36];
            var difficulty = data[0xD7];
            var hp = BitConverter.ToInt32(data, 0x10);
            var maxHp = BitConverter.ToInt32(data, 0x14);
            var mp = BitConverter.ToInt32(data, 0x1C);
            var maxMp = BitConverter.ToInt32(data, 0x20);
            var ap = BitConverter.ToInt32(data, 0x28);
            var maxAp = BitConverter.ToInt32(data, 0x2C);
            var sp = BitConverter.ToInt32(data, 0x0C);
            var gil = BitConverter.ToInt32(data, 0xB0);
            if (!saved)
            {
                var battle = Hypervisor.ReadArray(battleStatus + 0x1E8, 0x18, true);
                hp = BitConverter.ToInt32(battle, 0);
                maxHp = BitConverter.ToInt32(battle, 4);
                mp = BitConverter.ToInt32(battle, 8);
                maxMp = BitConverter.ToInt32(battle, 12);
                ap = BitConverter.ToInt32(battle, 16);
                maxAp = BitConverter.ToInt32(battle, 20);
            }

            if (level is < 1 or > 99 || difficulty > 1 || hp is < 0 or > 10000000 || maxHp is < 1 or > 10000000 || mp is < 0 or > 1000000 || maxMp is < 1 or > 1000000 || ap is < 0 or > 1000000 || maxAp is < 1 or > 1000000 || sp is < 0 or > 99999999 || gil is < 0 or > 999999999)
            {
                return false;
            }

            if (hud != Hypervisor.Read<ulong>(uiManager + 0x88, true) || status != Hypervisor.Read<ulong>(uiManager + 0xB8, true) || source != Hypervisor.Read<byte>(hud + 0x77B, true) || flags[0] != Hypervisor.Read<byte>(status, true) || flags[1] != Hypervisor.Read<byte>(status + 1, true) || level != Hypervisor.Read<byte>(saveStatus + 0x36, true))
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
                Ap = ap,
                MaxAp = maxAp,
                Sp = sp,
                Gil = gil,
                Difficulty = difficulty == 1 ? "Hard" : "Normal"
            };
            return true;
        }
    }
}
