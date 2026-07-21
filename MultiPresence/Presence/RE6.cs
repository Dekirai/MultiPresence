#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.RE6;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE6
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212349543463518268");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 6.json");
            PresenceRuntime.Start(nameof(RE6), "BH6", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("BH6");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("BH6"))
            {
                int state_get = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x1466884, [0xA4228]), true);

                if (state_get == 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 6", placeholders, "Booting");
                }
                else if (state_get > 1 && state_get < 9)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 6", placeholders, "Ingame");
                }
                else if (state_get == 20)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 6", placeholders, "Main_Menu");
                }
                else if (state_get == 10)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 6", placeholders, "Saving");
                }
                else if (state_get > 20)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 6", placeholders, "Cutscene");
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
            int stage_get = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x1466884, [0xA422C]), true);
            var stagevalue = await Stages.GetStage(stage_get);

            string[] stage = stagevalue.Split(':');

            return new Dictionary<string, object>
            {
                { "chapter", stage[0] },
                { "room", stage[1] }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}