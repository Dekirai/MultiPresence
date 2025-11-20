using DiscordRPC;
using MultiPresence.Models.MMBN5;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MMBN5
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1435202230553673728");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Mega Man Battle Network 5.json");
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
                if (_game != 7 && _game != 8)
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                }
                else
                {
                    int gamestate = Hypervisor.Read<byte>(0x80218E12, true);

                    if (gamestate == 12)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 5", placeholders, "In_Battle");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Mega Man Battle Network 5", placeholders);
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
            int area_get = Hypervisor.Read<byte>(0x80205FF4, true);
            int room_get = Hypervisor.Read<byte>(0x80205FF5, true);
            int hp = Hypervisor.Read<sbyte>(0x80208998, true);
            int maxhp = Hypervisor.Read<sbyte>(0x8020899A, true);
            int hp_battle = Hypervisor.Read<sbyte>(0x8020BE14, true);
            int maxhp_battle = Hypervisor.Read<sbyte>(0x8020BE16, true);
            int gamestate = Hypervisor.Read<byte>(0x80218E12, true);
            var location = await Areas.GetArea(area_get);

            int _game = Hypervisor.Read<byte>(0xABEF0A0);

            string gameicon = _game switch
            {
                7 => "protoman",
                8 => "colonel",
                _ => "logo"
            };

            string gamename = _game switch
            {
                7 => "Mega Man Battle Network 5: Team ProtoMan",
                8 => "Mega Man Battle Network 5: Team Colonel",
                _ => "Mega Man Battle Network 5"
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
