#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.SA2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SA2
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("982562008228560986");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Sonic Adventure 2.json");
            PresenceRuntime.Start(nameof(SA2), "sonic2app", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("sonic2app");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("sonic2app"))
            {
                int stage = Hypervisor.Read<byte>(0x1534B70);
                int character = Hypervisor.Read<byte>(0x1534B80);
                int lives = Hypervisor.Read<byte>(0x134B024);
                int rings = Hypervisor.Read<byte>(0x134B028);
                int p1_costume = Hypervisor.Read<byte>(0x134B015);
                int islevel = Hypervisor.Read<byte>(0x15420FF);

                var stage_name = await Stages.GetStage(stage);
                var character_name = await Characters.GetCharacter(character);

                if (islevel == 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Sonic Adventure 2", placeholders);
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Sonic Adventure 2", placeholders, "Ingame");
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
            int stage = Hypervisor.Read<byte>(0x1534B70);
            int character = Hypervisor.Read<byte>(0x1534B80);
            int lives = Hypervisor.Read<byte>(0x134B024);
            int rings = Hypervisor.Read<byte>(0x134B028);
            int p1_costume = Hypervisor.Read<byte>(0x134B015);

            var stage_name = await Stages.GetStage(stage);
            var character_name = await Characters.GetCharacter(character);
            if (p1_costume > 0)
            {
                return new Dictionary<string, object>
                {
                    { "level", stage_name },
                    { "character_icon", character_name[3] },
                    { "character", character_name[2] },
                    { "lives", lives },
                    { "rings", rings }
                };
            }
            else
            {
                return new Dictionary<string, object>
                {
                    { "level", stage_name },
                    { "character_icon", character_name[1] },
                    { "character", character_name[0] },
                    { "lives", lives },
                    { "rings", rings }
                };
            }
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}