namespace MultiPresence.Models.REV
{
    internal static partial class MemoryReader
    {
        private sealed record RaidAddresses(uint Coop, uint Stage, byte Stages, uint Difficulty, uint Level, string[] Names);

        private static RaidAddresses ResolveRaid(GameCode code, CampaignAddresses campaign, PartyAddresses party, string executable)
        {
            int raid = code.Find("8B 0D ?? ?? ?? ?? A1 ?? ?? ?? ?? 55 8B A8 ?? ?? ?? ?? 85 C9 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C 8B 15 ?? ?? ?? ?? 8B BA ?? ?? ?? ?? 83 FF ?? 7D");
            int level = code.Find("A1 ?? ?? ?? ?? 56 33 F6 85 C0 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C 8B 0D ?? ?? ?? ?? 8B 51 ?? 83 7A ?? 01 0F 85 ?? ?? ?? ?? 39 74 24 08 75 ?? A1 ?? ?? ?? ?? 85 C0 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C 8B 15 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? 8B B1");
            uint coop = code.Global(raid, 2);
            if (coop != party.Coop || code.Operand(raid, 7) != coop || code.Operand(raid, 44) != coop ||
                code.Operand(level, 1) != campaign.Holder || code.Bytes[level + 40] != campaign.Data || code.Bytes[level + 43] != campaign.Mode ||
                code.Operand(level, 58) != coop || code.Operand(level, 88) != coop || code.Operand(level, 94) != coop)
                throw new InvalidDataException("Invalid Revelations raid pattern.");

            return new RaidAddresses(coop, code.Operand(raid, 50), code.Bytes[raid + 56], code.Operand(raid, 14), code.Operand(level, 100),
                GameMessages.ReadMessages(executable, "Coop_eng.gmd"));
        }

        private static GameSnapshot ReadRaid(RaidAddresses found)
        {
            uint coop = Hypervisor.Read<uint>(found.Coop, true);
            if (coop == 0) return new GameSnapshot { State = "Loading" };
            int stage = Hypervisor.Read<int>(coop + (ulong)found.Stage, true);
            if (stage < 0 || stage > found.Stages) return new GameSnapshot { State = "Loading" };

            int difficulty = Hypervisor.Read<int>(coop + (ulong)found.Difficulty, true);
            return new GameSnapshot
            {
                State = "Ingame",
                Stage = found.Names.ElementAtOrDefault(stage) ?? "",
                StageName = found.Names.ElementAtOrDefault(found.Stages + 2 + stage) ?? "",
                DifficultyId = difficulty is >= 0 and <= 2 ? difficulty : null,
                Level = ReadLevel(found, coop)
            };
        }

        private static GameSnapshot ReadLobby(RaidAddresses found, PartyAddresses party)
        {
            uint coop = Hypervisor.Read<uint>(found.Coop, true);
            if (coop == 0) return new GameSnapshot();
            int state = Hypervisor.Read<int>(coop + (ulong)party.CoopState, true);
            if (state < 0) return new GameSnapshot();

            // The raid menu keeps the selected character class per session slot at +0xE4.
            int slot = state is 1 or 2 ? Hypervisor.Read<int>(coop + (ulong)party.CoopIndex, true) : 0;
            uint character = slot is >= 0 and < 2 ? Hypervisor.Read<uint>(coop + 0xE4UL + (uint)slot * 4, true) : 0;
            return new GameSnapshot { State = "Lobby", Character = Characters.GetName(character), Level = ReadLevel(found, coop) };
        }

        private static int? ReadLevel(RaidAddresses found, uint coop)
        {
            int level = Hypervisor.Read<int>(coop + (ulong)found.Level, true);
            return level is >= 0 and < 100 ? level + 1 : null;
        }
    }
}
