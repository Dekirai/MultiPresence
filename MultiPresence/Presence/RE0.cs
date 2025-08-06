using DiscordRPC;
using MultiPresence.Models.RE0;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE0
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1401966795811852379");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 0.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re0hd")[0];
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
            Process[] game = Process.GetProcessesByName("re0hd");
            if (game.Length > 0)
            {
                try
                {
                    int floor_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x97C9C0, [0xE472C]), true);

                    if (floor_get > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 0", placeholders);
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
                                LargeImageText = "Resident Evil 0"
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
                            LargeImageText = "Resident Evil 0"
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
            int stage_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x9CBEB4, [0x114]), true);

            int health = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x9E41BC, [0x23C, 0x154, 0x2BC, 0x3BC]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x9E41BC, [0x23C, 0x154, 0x2BC, 0x3C0]), true);

            string stage = await Stages.GetStage(stage_get);

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

            return new Dictionary<string, object>
            {
                { "room", stage },
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}