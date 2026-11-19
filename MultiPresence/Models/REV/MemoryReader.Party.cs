namespace MultiPresence.Models.REV
{
    internal static partial class MemoryReader
    {
        private sealed record PartyAddresses(uint Players, byte First, byte Last, byte Next, byte Player, uint Coop, uint CoopState,
            uint CoopIndex, uint Index, byte Flags, uint Stage);

        private static PartyAddresses ResolveParty(GameCode code)
        {
            int match = code.Find("8D 4C 24 ?? 51 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B F0 85 F6 0F 84 ?? ?? ?? ?? 83 BE ?? ?? ?? ?? 00 75 ?? 68 ?? ?? ?? ?? 6A 30 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C 8B 8E ?? ?? ?? ?? 6A 00");
            int control = code.Target(match, 12);
            if (!code.Matches(control, "53 55 8B E9 8B 5D ?? 56 57 3B 5D ?? 74 ?? 8B FF A1 ?? ?? ?? ?? 8B 73 ?? 85 C0 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C 8B 0D ?? ?? ?? ?? 8B BE ?? ?? ?? ?? E8 ?? ?? ?? ?? 3B C7 75 ?? 8B 46 ?? 83 E0 07 83 F8 02 74 ?? 83 F8 01 74 ?? 8B 5B ??"))
                throw new InvalidDataException("Invalid Revelations player pattern.");
            int local = code.Target(control, 61);
            uint coop = code.Global(control, 17);
            if (!code.Matches(local, "8B 81 ?? ?? ?? ?? 48 83 F8 01 77 ?? 8B 81 ?? ?? ?? ?? C3") ||
                code.Operand(control, 50) != coop || code.Operand(match, 28) != code.Operand(match, 57))
                throw new InvalidDataException("Invalid Revelations player pattern.");

            return new PartyAddresses(code.Global(match, 7), code.Bytes[control + 6], code.Bytes[control + 11], code.Bytes[control + 87],
                code.Bytes[control + 23], coop, code.Operand(local, 2), code.Operand(local, 14), code.Operand(control, 56),
                code.Bytes[control + 71], code.Operand(match, 28));
        }

        private static uint FindControlledPlayer(PartyAddresses found)
        {
            uint manager = Hypervisor.Read<uint>(found.Players, true);
            uint coop = Hypervisor.Read<uint>(found.Coop, true);
            if (manager == 0 || coop == 0) return 0;

            // sPlayer selects the actor that belongs to the local session slot (slot 0 outside of co-op).
            int state = Hypervisor.Read<int>(coop + (ulong)found.CoopState, true);
            int slot = state is 1 or 2 ? Hypervisor.Read<int>(coop + (ulong)found.CoopIndex, true) : 0;
            foreach (uint player in ReadPlayers(found, manager))
            {
                if (IsActive(found, player) && Hypervisor.Read<int>(player + (ulong)found.Index, true) == slot) return player;
            }
            return 0;
        }

        private static GameSnapshot ReadParty(PartyAddresses found, uint player, GameSnapshot snapshot)
        {
            string partner = "";
            uint manager = Hypervisor.Read<uint>(found.Players, true);
            foreach (uint other in ReadPlayers(found, manager))
            {
                if (other == player || !IsActive(found, other)) continue;
                partner = ReadCharacter(other);
                if (partner.Length > 0) break;
            }
            return snapshot with { Character = ReadCharacter(player), Partner = partner };
        }

        private static List<uint> ReadPlayers(PartyAddresses found, uint manager)
        {
            var players = new List<uint>();
            if (manager == 0) return players;
            uint last = Hypervisor.Read<uint>(manager + (ulong)found.Last, true);
            uint node = Hypervisor.Read<uint>(manager + (ulong)found.First, true);
            while (node != 0 && node != last && players.Count < 8)
            {
                uint player = Hypervisor.Read<uint>(node + (ulong)found.Player, true);
                if (player != 0) players.Add(player);
                node = Hypervisor.Read<uint>(node + (ulong)found.Next, true);
            }
            return players;
        }

        private static bool IsActive(PartyAddresses found, uint player) =>
            (Hypervisor.Read<int>(player + (ulong)found.Flags, true) & 7) is 1 or 2;

        private static string ReadCharacter(uint actor)
        {
            try
            {
                // The MT Framework type getter returns the actor's type descriptor (mov eax, descriptor; ret).
                uint methods = Hypervisor.Read<uint>(actor, true);
                uint getter = Hypervisor.Read<uint>((ulong)methods + 0x10, true);
                byte[] instructions = Hypervisor.ReadArray(getter, 6, true);
                if (instructions[0] != 0xB8 || instructions[5] != 0xC3) return "";

                uint descriptor = BitConverter.ToUInt32(instructions, 1);
                uint name = Hypervisor.Read<uint>((ulong)descriptor + 4, true);
                return Characters.GetName(Hypervisor.ReadString(name, 32, true));
            }
            catch (MemoryAccessException)
            {
                return "";
            }
        }
    }
}
