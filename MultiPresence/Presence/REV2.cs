using DiscordRPC;
using MultiPresence.Models.REV2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class REV2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        static int mission = 0;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1213180163446149121");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("rerev2")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("rerev2");
            if (game.Length > 0)
            {
                int stage_get = Hypervisor.Read<short>(0x115AACC);

                var stage = await Stages.GetStage(stage_get);

                if (stage[0] == "Raid Mode")
                {
                    if (stage[1] == "In Lobby")
                    {
                        mission = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x011DE690, [0x1E0, 0x4C, 0x3C, 0x14, 0x3C, 0x74, 0x7C4]), true) + 1;
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil Revelations 2", placeholders, "Lobby");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil Revelations 2", placeholders, "Ingame");
                    }
                }
                else
                {
                    discord.UpdateLargeAsset($"logo", $"Resident Evil Revelations 2");
                    discord.UpdateDetails($"");
                    discord.UpdateState($"");
                }

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int raid_character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x117ED54, [0x4A58]), true);
            int raid_chapter_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x117ED54, [0x30]), true);
            int raid_character_level = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0x117ED54, [0x4A59]), true);
            int raid_money = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x117D120, [0xBA08]), true);
            int stage_get = Hypervisor.Read<short>(0x115AACC);

            var stage = await Stages.GetStage(stage_get);
            var chapter = await Chapters.GetChapter(raid_chapter_get);

            var raid_character = await Characters.GetCharacter(raid_character_get);

            return new Dictionary<string, object>
            {
                { "character", raid_character },
                { "level", raid_character_level },
                { "money", raid_money },
                { "chapter", chapter },
                { "mission", mission }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
            discord.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}