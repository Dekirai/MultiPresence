namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private sealed record CampaignStageAddresses(uint Episode, uint LoadedStages);

        private static CampaignStageAddresses ResolveCampaignEpisode(GameCode code, uint info)
        {
            int match = code.FindOptional("8B 0D ?? ?? ?? ?? 53 56 8B B1 ?? ?? ?? ?? 57 8B B9 ?? ?? ?? ?? 56 6A 00 83 C1 20");
            int stages = code.FindOptional("33 FF BB ?? ?? ?? ?? 88 44 24 13 89 7C 24 1C 89 5C 24 18 EB ?? 8D 49 00 83 FF 05");
            uint episode = match >= 0 && code.Operand(match, 2) == info ? code.Operand(match, 10) : 0;
            return new CampaignStageAddresses(episode, stages >= 0 ? code.Global(stages, 3) : 0);
        }

        private static GameSnapshot ReadCampaign(uint info, CampaignStageAddresses found, int stage, int mode)
        {
            bool campaignStage = mode switch
            {
                3 => stage is >= 600 and < 700,
                4 => stage is >= 700 and < 800,
                _ => stage is >= 100 and <= 501
            };
            if (!campaignStage) return new GameSnapshot();
            if (found.LoadedStages == 0) return new GameSnapshot { State = "Loading" };

            // The saved stage survives returning to the title screen. The loaded camera areas are cleared.
            byte[] areas = Hypervisor.ReadArray(found.LoadedStages, 5 * 16, true);
            for (int offset = 0; offset < areas.Length; offset += 16)
            {
                if (BitConverter.ToInt32(areas, offset) != stage) continue;
                var snapshot = new GameSnapshot { State = "Campaign", StageId = stage, CampaignMode = mode };
                if (mode is 3 or 4) return snapshot;
                return found.Episode != 0 && Hypervisor.TryRead((ulong)info + found.Episode, out int episode, true) &&
                    episode is >= 1 and <= 4 ? snapshot with { Episode = episode } : new GameSnapshot { State = "Loading" };
            }
            return new GameSnapshot();
        }
    }
}
