using DiscordRPC;
using MultiPresence.Models.RE;
using System.Diagnostics;
using System.Security.AccessControl;

namespace MultiPresence.Presence
{
    public class RE8
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1402217219546677248");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 8.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re8")[0];
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
            Process[] game = Process.GetProcessesByName("re8");
            if (game.Length > 0)
            {
                try
                {
                    string pawn_name = Hypervisor.ReadString(Hypervisor.GetPointer64(0x0C4A27B8, [0x58, 0x28, 0x14]), 255, true, null, true);
                    float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x0C4A2870, [0x58, 0x108, 0xC8, 0x68, 0x48, 0x10]), true);
                    ulong _mercenaries = Hypervisor.GetPointer64(0x0C4EA5C0, [0x60, 0x18]);

                    string gamemode = pawn_name switch
                    {
                        "pl1000" => "Main Story",
                        "pl2000" => "Main Story",
                        "pl3000" => "Shadow of Rose",
                        "pl9000" => "Main Menu", //??? 
                        _ => "Main Story"
                    };

                    if (_mercenaries > 10000000)
                        gamemode = "Mercenaries";

                    if (gamemode == "Main Story")
                    {
                        if (maxhealth >= 1 && maxhealth <= 10000)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 8", placeholders, "Main Story");
                        }
                    }
                    else if (gamemode == "Shadow of Rose")
                    {
                        if (maxhealth >= 1 && maxhealth <= 10000)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 8", placeholders, "Shadow of Rose");
                        }
                    }
                    else if (gamemode == "Mercenaries" && pawn_name.StartsWith("pl"))
                    {
                        if (maxhealth >= 1 && maxhealth <= 10000)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 8", placeholders, "Mercenaries");
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
                                LargeImageText = "Resident Evil Village"
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
                            LargeImageText = "Resident Evil Village"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x0C4A2870, [0x58, 0x108, 0xC8, 0x68, 0x48, 0x14]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x0C4A2870, [0x58, 0x108, 0xC8, 0x68, 0x48, 0x10]), true);
            string pawn_name = Hypervisor.ReadString(Hypervisor.GetPointer64(0x0C4A27B8, [0x58, 0x28, 0x14]), 255, true, null, true);
            int mercenariesscore = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0C4EA5C0, [0x60, 0x18]), true);

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
                "pl1000" => "Main Story",
                "pl2000" => "Main Story",
                "pl3000" => "Shadow of Rose",
                "pl9000" => "Main Menu", //???
                _ => "Main Story"
            };

            string character = pawn_name switch
            {
                "pl1000" => "Ethan Winters",
                "pl2000" => "Chris Redfield",
                "pl2001" => "Chris Redfield",
                "pl3000" => "Rosemary Winters",
                "pl4000" => "Karl Heisenberg",
                "pl6000" => "Alcina Dimitrescu",
                _ => "Ethan Winters"
            };

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
                { "character", character },
                { "score", mercenariesscore }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}