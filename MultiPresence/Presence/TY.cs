#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.TY;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TY
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static string[] levelvalue = null;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("983292674863943720");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/TY the Tasmanian Tiger.json");
            PresenceRuntime.Start(nameof(TY), "TY", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("TY");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("TY"))
            {
                int health = Hypervisor.Read<byte>(0x2737CC);
                int level = Hypervisor.Read<byte>(0x28903C);
                int opals = Hypervisor.Read<int>(0x2888B0);

                levelvalue = await Levels.GetLevel(level);

                if (level == 4 || level == 5 || level == 6 || level == 8 || level == 9 || level == 10 || level == 12 || level == 13 || level == 14)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "TY The Tasmanian Tiger", placeholders, "Level");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "TY The Tasmanian Tiger", placeholders);
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
            int health = Hypervisor.Read<byte>(0x2737CC);
            int level = Hypervisor.Read<byte>(0x28903C);
            int opals = Hypervisor.Read<int>(0x2888B0);

            levelvalue = await Levels.GetLevel(level);

            return new Dictionary<string, object>
            {
                { "health", health },
                { "opals", opals },
                { "level", levelvalue }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}