using DiscordRPC;
using System.Diagnostics;
using MultiPresence.Models.MMBN;

namespace MultiPresence.Presence
{
    public class MMBN1
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1434620367053525092");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man Battle Network.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("MMBN_LC1")[0];
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
            Process[] game = Process.GetProcessesByName("MMBN_LC1");
            if (game.Length > 0)
            {
                int _game = Hypervisor.Read<byte>(0x987499C);
                if (_game != 0)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    int gamestate = Hypervisor.Read<byte>(0x802040E0, true);

                    if (gamestate == 12)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network", placeholders, "In_Battle");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network", placeholders);
                    }
                    await Task.Delay(3000);
                    Thread thread = new Thread(RPC);
                    thread.Start();
                }
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int area_get = Hypervisor.Read<byte>(0x802040E4, true);
            int room_get = Hypervisor.Read<byte>(0x802040E5, true);
            int hp = Hypervisor.Read<sbyte>(0x802040FC, true);
            int maxhp = Hypervisor.Read<sbyte>(0x802040FE, true);
            int hp_battle = Hypervisor.Read<sbyte>(0x80206990, true);
            int maxhp_battle = Hypervisor.Read<sbyte>(0x80206992, true);
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
