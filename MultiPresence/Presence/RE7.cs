using DiscordRPC;
using MultiPresence.Models.RE;
using System.Diagnostics;
using System.Security.AccessControl;

namespace MultiPresence.Presence
{
    public class RE7
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1401528728567418993");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 7.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re7")[0];
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
            Process[] game = Process.GetProcessesByName("re7");
            if (game.Length > 0)
            {
                try
                {
                    ulong _base = Hypervisor.GetPointer64(0x08FB2130, [0xD0, 0xF8, 0x0]);
                    ulong _player = Hypervisor.GetPointer64(_base + 0x70, [0x0], true);
                    ulong _properties = Hypervisor.GetPointer64(_base + 0x10, [0x0], true);
                    ulong _pawn = Hypervisor.GetPointer64(_properties + 0x28, [0x0], true);

                    string pawn_name = Hypervisor.ReadString(_pawn + 0x14, 255, true, null, true);
                    float maxhealth = Hypervisor.Read<float>(_player + 0x10, true);

                    string gamemode = pawn_name switch
                    {
                        "Pl0000" => "Main Story",
                        "Pl0000_Chapter1" => "Main Story",
                        "Pl1000" => "DLC Story",
                        "Pl2000" => "Main Story",
                        "Pl2100" => "Main Story",
                        "Pl3100_Chapter7_3" => "DLC Minigame",
                        "Pl3100_Chapter7_1" => "DLC Minigame",
                        "Pl3100_Chapter7_2" => "DLC Minigame",
                        "Pl3400" => "DLC Minigame",
                        "Pl9000" => "DLC Story",
                        "Pl2000_Birthday" => "DLC Minigame",
                        "Pl0000_IMD" => "DLC Story",
                        _ => "Main Story"
                    };

                    if (gamemode == "Main Story")
                    {
                        if (maxhealth >= 1 && maxhealth <= 5000)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 7", placeholders, "Main Story");
                        }
                    }
                    else if (gamemode == "DLC Story")
                    {
                        if (maxhealth >= 1 && maxhealth <= 5000)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersDLC);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 7", placeholders, "DLC Story");
                        }
                    }
                    else if (gamemode == "DLC Minigame")
                    {
                        if (maxhealth >= 1 && maxhealth <= 5000)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersMinigame);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 7", placeholders, "DLC Minigame");
                        }
                    }
                    else
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Resident Evil 7"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Resident Evil 7"
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
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            ulong _base = Hypervisor.GetPointer64(0x08FB2130, [0xD0, 0xF8, 0x0]);
            ulong _player = Hypervisor.GetPointer64(_base + 0x70, [0x0], true);
            ulong _properties = Hypervisor.GetPointer64(_base + 0x10, [0x0], true);
            ulong _pawn = Hypervisor.GetPointer64(_properties + 0x28, [0x0], true);

            float health = Hypervisor.Read<float>(_player + 0x14, true);
            float maxhealth = Hypervisor.Read<float>(_player + 0x10, true);
            string pawn_name = Hypervisor.ReadString(_pawn + 0x14, 255, true, null, true);

            string healthstatus = "";

            double percentage = (double)health / maxhealth * 100;

            if (percentage > 75)
                healthstatus = "Fine";
            else if (percentage > 50)
                healthstatus = "Caution";
            else if (percentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            string gamemode = pawn_name switch
            {
                "Pl0000" => "Main Story",
                "Pl0000_Chapter1" => "Main Story",
                "Pl2000" => "Main Story",
                "Pl2100" => "Main Story",
                _ => "Main Story"
            };

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "mode", gamemode }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersDLC()
        {
            ulong _base = Hypervisor.GetPointer64(0x08FB2130, [0xD0, 0xF8, 0x0]);
            ulong _player = Hypervisor.GetPointer64(_base + 0x70, [0x0], true);
            ulong _properties = Hypervisor.GetPointer64(_base + 0x10, [0x0], true);
            ulong _pawn = Hypervisor.GetPointer64(_properties + 0x28, [0x0], true);

            float health = Hypervisor.Read<float>(_player + 0x14, true);
            float maxhealth = Hypervisor.Read<float>(_player + 0x10, true);
            string pawn_name = Hypervisor.ReadString(_pawn + 0x14, 255, true, null, true);

            string healthstatus = "";

            double percentage = (double)health / maxhealth * 100;

            if (percentage > 75)
                healthstatus = "Fine";
            else if (percentage > 50)
                healthstatus = "Caution";
            else if (percentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            string gamemode = pawn_name switch
            {
                "Pl1000" => "Not a Hero",
                "Pl9000" => "End of Zoe",
                "Pl0000_IMD" => "Ethan Must Die",
                _ => "DLC Story"
            };

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "mode", gamemode }
            };
        }
        private static async Task<Dictionary<string, object>> GeneratePlaceholdersMinigame()
        {
            ulong _base = Hypervisor.GetPointer64(0x08FB2130, [0xD0, 0xF8, 0x0]);
            ulong _player = Hypervisor.GetPointer64(_base + 0x70, [0x0], true);
            ulong _properties = Hypervisor.GetPointer64(_base + 0x10, [0x0], true);
            ulong _pawn = Hypervisor.GetPointer64(_properties + 0x28, [0x0], true);

            float health = Hypervisor.Read<float>(_player + 0x14, true);
            float maxhealth = Hypervisor.Read<float>(_player + 0x10, true);
            string pawn_name = Hypervisor.ReadString(_pawn + 0x14, 255, true, null, true);

            string healthstatus = "";

            double percentage = (double)health / maxhealth * 100;

            if (percentage > 75)
                healthstatus = "Fine";
            else if (percentage > 50)
                healthstatus = "Caution";
            else if (percentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            string gamemode = pawn_name switch
            {
                "Pl3100_Chapter7_3" => "Banned Footage - Nightmare",
                "Pl3100_Chapter7_1" => "Banned Footage - Bedroom",
                "Pl3100_Chapter7_2" => "Banned Footage - 21",
                "Pl3400" => "Banned Footage - Daughters",
                "Pl2000_Birthday" => "Jack's 55th Birthday",
                _ => "DLC Minigame"
            };

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "mode", gamemode }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}