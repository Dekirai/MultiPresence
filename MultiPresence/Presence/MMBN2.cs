#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.MMBN2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMBN2
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434620841689092188");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man Battle Network 2.json");
            PresenceRuntime.Start(nameof(MMBN2), "MMBN_LC1", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("MMBN_LC1");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("MMBN_LC1"))
            {
                int _game = Hypervisor.Read<byte>(0x987499C);
                if (_game != 1)
                {
                    discord.Deinitialize();
                    updater.Dispose();
                    PresenceRuntime.RequestDetection();
                }
                else
                {
                    int gamestate = Hypervisor.Read<byte>(0x802040E0, true);

                    if (gamestate == 12)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 2", placeholders, "In_Battle");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 2", placeholders);
                    }
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
            int area_get = Hypervisor.Read<byte>(0x802040E4, true);
            int room_get = Hypervisor.Read<byte>(0x802040E5, true);
            int hp = Hypervisor.Read<sbyte>(0x80204100, true);
            int maxhp = Hypervisor.Read<sbyte>(0x80204102, true);
            int hp_battle = Hypervisor.Read<sbyte>(0x802084A4, true);
            int maxhp_battle = Hypervisor.Read<sbyte>(0x802084A6, true);
            int zenny = Hypervisor.Read<int>(0x80204154, true);
            var location = await Areas.GetArea(area_get);

            return new Dictionary<string, object>
            {
                { "hp", hp },
                { "hp_battle", hp_battle },
                { "maxhp", maxhp },
                { "maxhp_battle", maxhp_battle },
                { "zenny", zenny },
                { "location", location[room_get] }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
