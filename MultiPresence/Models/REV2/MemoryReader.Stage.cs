namespace MultiPresence.Models.REV2
{
    internal static partial class MemoryReader
    {
        private sealed record StageAddresses(uint Game, byte Mode, uint Info, uint Stage, uint Raid, byte Chapter, byte Mission,
            uint TableManager, uint Tables, CampaignStageAddresses CampaignEpisode);

        private static StageAddresses ResolveStage(GameCode code)
        {
            int game = code.Find("A1 ?? ?? ?? ?? 56 83 78 ?? 05 8B F1 0F 85 ?? ?? ?? ?? 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0");
            int info = code.Find("8B 0D ?? ?? ?? ?? 81 B9 ?? ?? ?? ?? D0 07 00 00 0F 85 ?? ?? ?? ?? 6A 01 83 C1 20 E8 ?? ?? ?? ?? 84 C0");
            int selector = code.Find("8B 0D ?? ?? ?? ?? 50 89 79 ?? 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? A1 ?? ?? ?? ?? 89 70 ?? A1 ?? ?? ?? ?? C7 40");
            int mission = code.Find("8B 44 24 04 89 41 ?? 99 56 BE E8 03 00 00 F7 FE B8 1F 85 EB 51");
            int manager = code.FindOptional("8B 0D ?? ?? ?? ?? 8D 47 FF 50 E8 ?? ?? ?? ?? FF 74 24 18 8B 74 24 18");
            int tables = code.FindOptional("83 F8 64 7D ?? 8D 04 40 8D 34 81 8D 8E ?? ?? ?? ?? E8 ?? ?? ?? ?? 85 C0 74 ?? 6A 10");
            return new StageAddresses(code.Global(game, 1), code.Bytes[game + 8], code.Global(info, 2), code.Operand(info, 8),
                code.Global(selector, 2), code.Bytes[selector + 9], code.Bytes[mission + 6],
                manager >= 0 ? code.Global(manager, 2) : 0, tables >= 0 ? code.Operand(tables, 13) : 0,
                ResolveCampaignEpisode(code, code.Global(info, 2)));
        }

        private static GameSnapshot ReadStage(StageAddresses found)
        {
            uint game = Hypervisor.Read<uint>(found.Game, true);
            if (game == 0) return new GameSnapshot();
            int mode = Hypervisor.Read<int>(game + found.Mode, true);
            if (mode is < 0 or > 5) return new GameSnapshot();

            uint info = Hypervisor.Read<uint>(found.Info, true);
            if (info == 0) return new GameSnapshot { State = "Loading" };
            int stage = Hypervisor.Read<int>(info + found.Stage, true);
            if (mode != 5) return ReadCampaign(info, found.CampaignEpisode, stage, mode);
            if (stage == 2000) return new GameSnapshot { State = "Lobby", StageId = stage };
            if (stage is 0 or -1 or 65535) return new GameSnapshot { State = "Loading" };
            if (!Stages.IsRaidMission(stage)) return new GameSnapshot();

            var snapshot = new GameSnapshot { State = "Ingame", StageId = stage };
            uint raid = Hypervisor.Read<uint>(found.Raid, true);
            if (raid == 0) return snapshot;
            int chapter = Hypervisor.Read<int>(raid + found.Chapter, true);
            int missionCode = Hypervisor.Read<int>(raid + found.Mission, true);
            if (missionCode is < 1000 or >= 100000) return snapshot;
            int mission = missionCode % 100;
            int difficulty = missionCode / 100 % 10;
            int? recommended = ReadRecommendedLevel(found, missionCode, stage);
            if (chapter < 1 && !recommended.HasValue) return snapshot;
            return snapshot with
            {
                ChapterId = chapter is >= 1 and <= 10 ? chapter : null,
                Mission = mission > 0 ? mission : null,
                DifficultyId = difficulty <= 3 ? difficulty : null,
                RecommendedLevel = recommended
            };
        }

        private static int? ReadRecommendedLevel(StageAddresses found, int missionCode, int stage)
        {
            int tableId = missionCode / 1000;
            if (tableId is < 1 or >= 100 || found.TableManager == 0 || found.Tables == 0 ||
                !Hypervisor.TryRead(found.TableManager, out uint manager, true) || manager == 0 ||
                !Hypervisor.TryRead(manager + found.Tables + (uint)tableId * 12UL + 8, out uint table, true) || table == 0 ||
                !Hypervisor.TryRead(table + 0x78, out uint rows, true) || rows == 0 ||
                !Hypervisor.TryRead(table + 0x7C, out int count, true) || count is < 1 or > 256) return null;

            int first = 0, last = count - 1;
            while (first <= last)
            {
                int index = (first + last) / 2;
                ulong row = rows + (uint)index * 12UL;
                if (!Hypervisor.TryRead(row + 4, out uint cells, true) || cells == 0 ||
                    !Hypervisor.TryRead(row + 8, out int columns, true) || columns < 5 ||
                    !Hypervisor.TryRead(cells + 8, out int id, true)) return null;

                if (id < missionCode) first = index + 1;
                else if (id > missionCode) last = index - 1;
                else
                {
                    if (Hypervisor.TryRead(cells + 20, out int missionStage, true) && missionStage == stage &&
                        Hypervisor.TryRead(cells + 56, out int level, true) && level is >= 1 and <= 100) return level;
                    return null;
                }
            }
            return null;
        }
    }
}
