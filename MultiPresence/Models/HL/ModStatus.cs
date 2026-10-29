namespace MultiPresence.Models.HL
{
    internal static class ModStatus
    {
        public const string OfficialPattern = "48 83 EC 28 48 8B 05 ?? ?? ?? ?? 48 85 C0 75 0C E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 80 78 28 00 74 25 48 8B 40 40";
        public const string UnofficialPattern = "80 3D ?? ?? ?? ?? 00 48 8B F9 0F 85 ?? ?? ?? ?? 48 89 58 08 48 89 70 10 4C 89 70 18 45 33 F6";

        public static bool? Read(ulong managerSlot, ulong unofficialAddress)
        {
            var official = ReadOfficial(managerSlot);
            bool? unofficial = null;
            if (unofficialAddress != 0 && Hypervisor.TryRead<byte>(unofficialAddress, out var flag, true) && flag <= 1)
            {
                unofficial = flag != 0;
            }

            if (official == true || unofficial == true)
            {
                return true;
            }

            return official == false && unofficial == false ? false : null;
        }

        private static bool? ReadOfficial(ulong managerSlot)
        {
            if (managerSlot == 0 || !Hypervisor.TryRead<ulong>(managerSlot, out var manager, true) || !ValidPointer(manager) ||
                !Hypervisor.TryRead<byte>(manager + 0x28, out var enabled, true) || enabled > 1)
            {
                return null;
            }

            if (enabled == 0)
            {
                return Hypervisor.TryRead<ulong>(managerSlot, out var current, true) && current == manager &&
                    Hypervisor.TryRead<byte>(manager + 0x28, out var currentFlag, true) && currentFlag == enabled ? false : null;
            }

            if (!Hypervisor.TryRead<ulong>(manager + 0x40, out var state, true) || !ValidPointer(state) ||
                !Hypervisor.TryRead<ulong>(state + 0x10, out var container, true) || !ValidPointer(container) ||
                !Hypervisor.TryRead<int>(container + 0x30, out var count, true) || count < 0 || count > 4096)
            {
                return null;
            }

            if (!Hypervisor.TryRead<ulong>(managerSlot, out var currentManager, true) || currentManager != manager ||
                !Hypervisor.TryRead<byte>(manager + 0x28, out var currentEnabled, true) || currentEnabled != enabled ||
                !Hypervisor.TryRead<ulong>(manager + 0x40, out var currentState, true) || currentState != state ||
                !Hypervisor.TryRead<ulong>(state + 0x10, out var currentContainer, true) || currentContainer != container)
            {
                return null;
            }

            return count != 0;
        }

        private static bool ValidPointer(ulong value)
        {
            return value >= 0x10000 && value <= 0x00007FFFFFFEFFFF && (value & 7) == 0;
        }
    }
}
