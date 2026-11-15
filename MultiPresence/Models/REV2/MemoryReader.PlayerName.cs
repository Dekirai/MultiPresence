namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private const string PlayerNamePattern = "8B 0D ?? ?? ?? ?? 6A 10 89 7C 24 28 E8 ?? ?? ?? ?? 50 8D 44 24 2C 50 E8";
        private readonly record struct PlayerNameAddresses(uint Profile, uint NameOffset);

        private static PlayerNameAddresses ResolvePlayerName(GameCode code)
        {
            int match = code.FindOptional(PlayerNamePattern);
            if (match < 0) return default;

            long target = match + 17L + BitConverter.ToInt32(code.Bytes, match + 13);
            if (target < 0 || target > code.Bytes.Length - 7) return default;
            int getter = (int)target;
            // The local profile getter returns its inline UTF-16 name with lea eax, [ecx+offset]; ret.
            if (code.Bytes[getter] != 0x8D || code.Bytes[getter + 1] != 0x81 || code.Bytes[getter + 6] != 0xC3)
                return default;

            return new PlayerNameAddresses(code.Global(match, 2), code.Operand(getter, 2));
        }

        private static string ReadPlayerName(PlayerNameAddresses addresses)
        {
            if (addresses.Profile == 0 || !Hypervisor.TryRead<uint>(addresses.Profile, out uint profile, true) || profile == 0)
                return "";

            try
            {
                return Hypervisor.ReadString((ulong)profile + addresses.NameOffset, 32, true, isUnicode: true);
            }
            catch (MemoryAccessException)
            {
                return "";
            }
        }
    }
}
