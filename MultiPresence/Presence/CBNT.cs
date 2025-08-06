using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class CBNT
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1396527703255027785");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Crash Bandicoot N. Sane Trilogy.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("CrashBandicootNSaneTrilogy")[0];
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
            Process[] game = Process.GetProcessesByName("CrashBandicootNSaneTrilogy");
            if (game.Length > 0)
            {
                string game_get = Hypervisor.ReadString(0x1A5C6E0, 100);

                try
                {
                    if (!game_get.ToLower().Contains("startscreen"))
                    {
                        if (game_get.ToLower().Contains("_hub"))
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Crash Bandicoot N. Sane Trilogy", placeholders, "Hub");
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersLevel);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Crash Bandicoot N. Sane Trilogy", placeholders, "Level");
                        }
                    }
                    else
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In the title screen",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Crash Bandicoot N. Sane Trilogy"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In the title screen",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Crash Bandicoot N. Sane Trilogy"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
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
            string game_get = Hypervisor.ReadString(0x1A5C6E0, 6);
            int lives = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01AA27C8, [0x10]), true);

            string game_short = game_get switch
            {
                "crash1" => "Crash 1",
                "crash2" => "Crash 2",
                "crash3" => "Crash 3",
                _ => "Unknown Game"
            };

            string game_long = game_get switch
            {
                "crash1" => "Crash Bandicoot",
                "crash2" => "Crash Bandicoot 2: Cortex Strikes Back",
                "crash3" => "Crash Bandicoot 3: Warped",
                _ => "Unknown Game"
            };

            return new Dictionary<string, object>
            {
                { "game_short", game_short },
                { "game_long", game_long },
                { "lives", lives },
                { "gamelogo", game_get }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersLevel()
        {
            int destroyablecrates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01A69A98, [0x18, 0x60, 0x0, 0x6B0]), true);
            int crates = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01A665E8, [0x60, 0x10, 0x40, 0x48, 0x58]), true);
            int lives = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01AA27C8, [0x10]), true);

            string game_get = Hypervisor.ReadString(0x1A5C6E0, 6);
            string[] level_path = Hypervisor.ReadString(0x1A5C6E0, 255).Split("/");
            string level_get = level_path[1].Split("_")[1];

            string game_short = game_get switch
            {
                "crash1" => "Crash 1",
                "crash2" => "Crash 2",
                "crash3" => "Crash 3",
                _ => "Unknown Game"
            };

            string game_long = game_get switch
            {
                "crash1" => "Crash Bandicoot",
                "crash2" => "Crash Bandicoot 2: Cortex Strikes Back",
                "crash3" => "Crash Bandicoot 3: Warped",
                _ => "Unknown Game"
            };

            return new Dictionary<string, object>
            {
                { "game_short", game_short },
                { "game_long", game_long },
                { "level", level_get },
                { "maxcrates", destroyablecrates },
                { "currentcrates", crates },
                { "lives", lives },
                { "gamelogo", game_get }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
