using DiscordRPC;
using MultiPresence.Models.MMBN6;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMBN6
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1257021467699449866");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man Battle Network 6.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("MMBN_LC2")[0];
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
            Process[] game = Process.GetProcessesByName("MMBN_LC2");
            if (game.Length > 0)
            {
                int _game = Hypervisor.Read<byte>(0xABEF0A0);
                if (_game != 9 && _game != 10)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    int gamestate = Hypervisor.Read<byte>(0x80205940, true);

                    if (gamestate == 12)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 6", placeholders, "In_Battle");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 6", placeholders);
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
            int area_get = Hypervisor.Read<byte>(0x80205944, true);
            int room_get = Hypervisor.Read<byte>(0x80205945, true);
            int hp = Hypervisor.Read<sbyte>(0x8020858C, true);
            int maxhp = Hypervisor.Read<sbyte>(0x8020858E, true);
            int hp_battle = Hypervisor.Read<sbyte>(0x8020A8F4, true);
            int maxhp_battle = Hypervisor.Read<sbyte>(0x8020A8F6, true);
            int gamestate = Hypervisor.Read<byte>(0x80205940, true);
            var location = await Areas.GetArea(area_get);
            int _game = Hypervisor.Read<byte>(0xABEF0A0);

            string gameicon = _game switch
            {
                9 => "gregar",
                10 => "falzar",
                _ => "logo"
            };

            string gamename = _game switch
            {
                9 => "Mega Man Battle Network 6: Cybeast Gregar",
                10 => "Mega Man Battle Network 6: Cybeast Falzar",
            };

            return new Dictionary<string, object>
            {
                { "hp", hp },
                { "hp_battle", hp_battle },
                { "maxhp", maxhp },
                { "maxhp_battle", maxhp_battle },
                { "location", location[room_get] },
                { "gameicon", gameicon },
                { "gamename", gamename }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
