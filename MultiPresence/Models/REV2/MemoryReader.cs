using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private sealed record Addresses(StageAddresses Stage, ProfileAddresses Profile, PlayerNameAddresses PlayerName,
            EnemyAddresses? Enemies, CampaignPartyAddresses? CampaignParty, ObjectiveAddresses? Objective,
            Dictionary<int, string> Locations);
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
            var stage = ReadStage(found.Stage);
            GameSnapshot snapshot;
            if (stage.State == "Campaign")
            {
                snapshot = ReadCampaignParty(found.CampaignParty, stage) with
                {
                    Location = CampaignLocations.GetName(stage.StageId, found.Locations),
                    Objective = ReadObjective(found.Objective, stage.CampaignMode)
                };
            }
            else if (stage.State is "Lobby" or "Ingame")
            {
                snapshot = ReadProfile(found.Profile, stage) with { PlayerName = ReadPlayerName(found.PlayerName) };
                if (snapshot.State == "Ingame") snapshot = ReadEnemies(found.Enemies, snapshot);
            }
            else return stage;
            var current = ReadStage(found.Stage);
            return stage == current ? snapshot : new GameSnapshot { State = "Loading" };
        }

        private static Addresses ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidOperationException("Revelations 2 is not attached.");
            if (process.Id != processId)
            {
                Reset();
                processId = process.Id;
            }
            if (addresses != null) return addresses;
            if (DateTime.UtcNow < nextScan) throw new InvalidDataException("Waiting to retry Revelations 2 patterns.");
            nextScan = DateTime.UtcNow.AddSeconds(10);

            var module = process.MainModule ?? throw new InvalidDataException("Missing Revelations 2 module.");
            var code = new GameCode(module);
            addresses = new Addresses(ResolveStage(code), ResolveProfile(code), ResolvePlayerName(code), ResolveEnemies(code),
                ResolveCampaignParty(code), ResolveObjective(code, module.FileName),
                GameMessages.ReadLabels(module.FileName, "m25_sys_area_eng.gmd", "25_area_"));
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
                    throw new InvalidDataException("Invalid Revelations 2 code section.");
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
                return match >= 0 ? match : throw new InvalidDataException("Missing or ambiguous Revelations 2 pattern.");
            }

            public uint Operand(int match, int offset) => BitConverter.ToUInt32(Bytes, match + offset);

            public uint Global(int match, int offset)
            {
                uint address = Operand(match, offset);
                if (address < start || address + 4UL > end)
                    throw new InvalidDataException("Invalid Revelations 2 address.");
                return address;
            }
        }
    }
}
