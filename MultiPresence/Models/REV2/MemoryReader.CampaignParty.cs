namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private sealed record CampaignPartyAddresses(uint Manager, byte Slots, uint Control, uint Partner);

        private static CampaignPartyAddresses? ResolveCampaignParty(GameCode code)
        {
            int manager = code.FindOptional("53 56 57 8B F9 C7 07 ?? ?? ?? ?? FF 15 ?? ?? ?? ?? C7 05 ?? ?? ?? ?? 00 00 00 00 8D B7 ?? ?? ?? ?? BB 18 00 00 00");
            int player = code.FindOptional("33 C0 8D 51 ?? 56 8B 32 85 F6 74 ?? 83 BE ?? ?? ?? ?? 00 74 ?? 40 83 C2 04 83 F8 08");
            int partner = code.FindOptional("8B 89 ?? ?? ?? ?? 85 C9 74 ?? 83 B9 ?? ?? ?? ?? 00 75 ?? E8 ?? ?? ?? ?? 84 C0 74 ?? 32 C0 C3 B0 01 C3");
            if (manager < 0 || player < 0 || partner < 0 || code.Operand(player, 14) != code.Operand(partner, 12)) return null;

            return new CampaignPartyAddresses(code.Global(manager, 19), code.Bytes[player + 4],
                code.Operand(player, 14), code.Operand(partner, 2));
        }

        private static GameSnapshot ReadCampaignParty(CampaignPartyAddresses? found, GameSnapshot snapshot)
        {
            if (found == null || !Hypervisor.TryRead(found.Manager, out uint manager, true) || manager == 0) return snapshot;
            for (uint slot = 0; slot < 8; slot++)
            {
                if (!Hypervisor.TryRead(manager + (ulong)found.Slots + slot * 4, out uint player, true) || player == 0 ||
                    !Hypervisor.TryRead(player + (ulong)found.Control, out int control, true) || control != 0) continue;

                string character = ReadCampaignActor(player, snapshot.CampaignMode);
                string partner = Hypervisor.TryRead(player + (ulong)found.Partner, out uint other, true) && other != player
                    ? ReadCampaignActor(other, snapshot.CampaignMode) : "";
                return snapshot with { CampaignCharacter = character, Partner = partner };
            }
            return snapshot;
        }

        private static string ReadCampaignActor(uint actor, int? campaignMode)
        {
            if (actor == 0) return "";
            try
            {
                // The MT Framework type getter returns the model's type descriptor (mov eax, descriptor; ret).
                uint methods = Hypervisor.Read<uint>(actor, true);
                uint getter = Hypervisor.Read<uint>((ulong)methods + 0x10, true);
                byte[] instructions = Hypervisor.ReadArray(getter, 6, true);
                if (instructions[0] != 0xB8 || instructions[5] != 0xC3) return "";

                uint descriptor = BitConverter.ToUInt32(instructions, 1);
                uint name = Hypervisor.Read<uint>((ulong)descriptor + 4, true);
                string model = Hypervisor.ReadString(name, 8, true);
                if (campaignMode == 3 && model is "uPl3300" or "uPl3310") return "Evgeny";
                if (campaignMode == 4 && model == "uPl2440") return "Dark Natalia";
                if (model.Length != 7 || !model.StartsWith("uPl2", StringComparison.Ordinal) ||
                    !char.IsAsciiDigit(model[5]) || !char.IsAsciiDigit(model[6])) return "";

                return model[4] switch
                {
                    '1' => "Barry Burton",
                    '2' => "Claire Redfield",
                    '3' => "Moira Burton",
                    '4' => "Natalia Korda",
                    _ => ""
                };
            }
            catch (MemoryAccessException)
            {
                return "";
            }
        }
    }
}
