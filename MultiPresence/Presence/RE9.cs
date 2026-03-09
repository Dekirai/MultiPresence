using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE9
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1479128647133560892");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 9.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re9")[0];
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
            Process[] game = Process.GetProcessesByName("re9");
            if (game.Length > 0)
            {
                try
                {
                    int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0E81ACC8, [0x40, 0x70, 0x10, 0x30]), true);

                    if (maxhealth >= 1 && maxhealth <= 10000)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 9", placeholders);
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
                                LargeImageText = "Resident Evil Requiem"
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
                            LargeImageText = "Resident Evil Requiem"
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
            int health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0E81ACC8, [0x40, 0x70, 0x10, 0x28]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0E81ACC8, [0x40, 0x70, 0x10, 0x30]), true);

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
                { "health", health },
                { "maxhealth", maxhealth },
                { "healthstatus", healthstatus },
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}