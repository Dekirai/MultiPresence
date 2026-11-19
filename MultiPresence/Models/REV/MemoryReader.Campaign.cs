namespace MultiPresence.Models.REV
{
    internal static partial class MemoryReader
    {
        private sealed record CampaignAddresses(uint Holder, byte Data, byte Mode, uint Rank, byte Difficulty, string[] Titles);

        private static CampaignAddresses ResolveCampaign(GameCode code, string executable)
        {
            int holder = code.Find("A1 ?? ?? ?? ?? 85 C0 74 ?? 8B 0D ?? ?? ?? ?? 85 C9 74 ?? 8B 15 ?? ?? ?? ?? 85 D2 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 83 C4 0C A1 ?? ?? ?? ?? 8B 48 ?? 83 79 ?? 01 75 ?? 6A 01 E8 ?? ?? ?? ?? 8B 40 ?? 83 C4 04 C3");
            int rank = code.Find("A1 ?? ?? ?? ?? 85 C0 75 ?? 68 ?? ?? ?? ?? 6A 3E 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? 8B 41 ?? 83 C4 0C C3 8B 15 ?? ?? ?? ?? 8B 42 ?? C3");
            uint game = code.Global(holder, 11);
            uint difficulty = code.Global(rank, 1);
            if (code.Operand(holder, 21) != game || code.Operand(holder, 50) != game ||
                code.Operand(rank, 28) != difficulty || code.Bytes[rank + 34] != code.Bytes[rank + 47])
                throw new InvalidDataException("Invalid Revelations campaign pattern.");

            return new CampaignAddresses(game, code.Bytes[holder + 56], code.Bytes[holder + 59], difficulty, code.Bytes[rank + 34],
                GameMessages.ReadMessages(executable, "campaign_eng.gmd"));
        }

        private static int? ReadMode(CampaignAddresses found)
        {
            uint holder = Hypervisor.Read<uint>(found.Holder, true);
            uint data = holder == 0 ? 0 : Hypervisor.Read<uint>(holder + (ulong)found.Data, true);
            return data == 0 ? null : Hypervisor.Read<int>(data + (ulong)found.Mode, true);
        }

        private static GameSnapshot ReadCampaign(CampaignAddresses found)
        {
            uint holder = Hypervisor.Read<uint>(found.Holder, true);
            uint data = holder == 0 ? 0 : Hypervisor.Read<uint>(holder + (ulong)found.Data, true);
            if (data == 0) return new GameSnapshot { State = "Loading" };

            // cSystemData<Game> keeps the loaded area jump at +8. Its id encodes the episode like the episode title screen expects.
            uint area = Hypervisor.Read<uint>(data + 8UL, true);
            if (area == 0) return new GameSnapshot { State = "Loading" };
            uint id = Hypervisor.Read<uint>(area + 4UL, true);
            int episode = id >> 24 == 0x92 ? (int)(id >> 16 & 0xFF) : 0;

            uint rank = Hypervisor.Read<uint>(found.Rank, true);
            int difficulty = rank == 0 ? -1 : Hypervisor.Read<int>(rank + (ulong)found.Difficulty, true);
            return new GameSnapshot
            {
                State = "Campaign",
                Episode = episode is >= 1 and <= 12 ? episode : null,
                EpisodeTitle = episode is >= 1 and <= 12 ? found.Titles.ElementAtOrDefault(12 + episode) ?? "" : "",
                DifficultyId = difficulty is >= 0 and <= 2 ? difficulty : null
            };
        }
    }
}
