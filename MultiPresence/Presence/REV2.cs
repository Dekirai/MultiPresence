#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.REV2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class REV2
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        static int mission = 0;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1213180163446149121");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil Revelations 2.json");
            PresenceRuntime.Start(nameof(REV2), "rerev2", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("rerev2");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("rerev2"))
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
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Resident Evil Revelations 2"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
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
        }
    }
}