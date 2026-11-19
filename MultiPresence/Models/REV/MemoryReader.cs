using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace MultiPresence.Models.REV
{
    internal static partial class MemoryReader
    {
        private sealed record Addresses(CampaignAddresses Campaign, PartyAddresses Party, RaidAddresses Raid, LocationAddresses? Location,
            ObjectiveAddresses? Objective);
        private static Addresses? addresses;
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            processId = 0;
            addresses = null;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            var found = ResolveAddresses();
            int? mode = ReadMode(found.Campaign);
            if (mode is not (0 or 1)) return new GameSnapshot();

            // sPlayer only exists while an episode or a raid stage is loaded; the menus run without it.
            uint players = Hypervisor.Read<uint>(found.Party.Players, true);
            if (players == 0) return mode == 1 ? ReadLobby(found.Raid, found.Party) : new GameSnapshot();
            var stage = mode == 0 ? ReadCampaign(found.Campaign) : ReadRaid(found.Raid);
            if (stage.State == "Loading") return stage;

            var snapshot = stage;
            uint player = FindControlledPlayer(found.Party);
            if (player != 0)
                snapshot = ReadParty(found.Party, player, snapshot) with { Location = ReadLocation(found.Location, found.Party, player) };
            if (mode == 0) snapshot = snapshot with { Objective = ReadObjective(found.Objective) };

            var current = mode == 0 ? ReadCampaign(found.Campaign) : ReadRaid(found.Raid);
            return ReadMode(found.Campaign) == mode && Hypervisor.Read<uint>(found.Party.Players, true) == players && current == stage
                ? snapshot : new GameSnapshot { State = "Loading" };
        }

        private static Addresses ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidOperationException("Revelations is not attached.");
            if (process.Id != processId)
            {
                Reset();
                processId = process.Id;
            }
            if (addresses != null) return addresses;
            if (DateTime.UtcNow < nextScan) throw new InvalidDataException("Waiting to retry Revelations patterns.");
            nextScan = DateTime.UtcNow.AddSeconds(10);

            var module = process.MainModule ?? throw new InvalidDataException("Missing Revelations module.");
            var code = new GameCode(module);
            var campaign = ResolveCampaign(code, module.FileName);
            var party = ResolveParty(code);
            addresses = new Addresses(campaign, party, ResolveRaid(code, campaign, party, module.FileName),
                ResolveLocation(code, module.FileName), ResolveObjective(code, module.FileName));
            return addresses;
        }

        private sealed class GameCode
        {
            private readonly ulong start;
            private readonly ulong end;
            public byte[] Bytes { get; }

            public GameCode(ProcessModule module)
            {
                start = (ulong)module.BaseAddress.ToInt64();
                end = start + (ulong)module.ModuleMemorySize;
                using var header = new MemoryStream(Hypervisor.ReadArray(start, 4096, true));
                var pe = new PEHeaders(header);
                var section = pe.SectionHeaders.FirstOrDefault(s => s.Name == ".text");
                if (pe.CoffHeader.Machine != Machine.I386 || section.VirtualAddress <= 0 || section.VirtualSize <= 0 ||
                    (long)section.VirtualAddress + section.VirtualSize > module.ModuleMemorySize ||
                    !section.SectionCharacteristics.HasFlag(SectionCharacteristics.MemExecute))
                    throw new InvalidDataException("Invalid Revelations code section.");
                Bytes = Hypervisor.ReadArray(start + (uint)section.VirtualAddress, section.VirtualSize, true);
            }

            public int FindOptional(string signature)
            {
                var pattern = SignaturePattern.Parse(signature);
                int match = pattern.IndexOf(Bytes);
                return match >= 0 && pattern.IndexOf(Bytes.AsSpan(match + 1)) < 0 ? match : -1;
            }

            public int Find(string signature)
            {
                int match = FindOptional(signature);
                return match >= 0 ? match : throw new InvalidDataException("Missing or ambiguous Revelations pattern.");
            }

            public bool Matches(int position, string signature)
            {
                var pattern = SignaturePattern.Parse(signature);
                return position >= 0 && position <= Bytes.Length - pattern.Length &&
                    pattern.IndexOf(Bytes.AsSpan(position, pattern.Length)) == 0;
            }

            public int Target(int match, int offset)
            {
                long target = match + offset + 4L + BitConverter.ToInt32(Bytes, match + offset);
                return target >= 0 && target < Bytes.Length ? (int)target : throw new InvalidDataException("Invalid Revelations call.");
            }

            public uint Operand(int match, int offset) => BitConverter.ToUInt32(Bytes, match + offset);

            public uint Global(int match, int offset)
            {
                uint address = Operand(match, offset);
                if (address < start || address + 4UL > end)
                    throw new InvalidDataException("Invalid Revelations address.");
                return address;
            }
        }
    }
}
