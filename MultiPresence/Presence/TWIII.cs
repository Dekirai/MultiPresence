using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TWIII
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1385197081102123048");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/The Witcher 3.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("witcher3")[0];
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
            Process[] game = Process.GetProcessesByName("witcher3");
            if (game.Length > 0)
            {
                float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x05BD3B30, [0x30, 0x1C0, 0x110, 0x30, 0x4]), true);

                if (maxhealth > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "The Witcher 3", placeholders);
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
                            LargeImageText = "The Witcher 3"
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
            float health = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x05BD3B30, [0x30, 0x1C0, 0x110, 0x30, 0x0]), true);
            float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x05BD3B30, [0x30, 0x1C0, 0x110, 0x30, 0x4]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057F5960, [0x1A8, 0x40, 0x1C0, 0x40, 0x40, 0x20, 0x14]), true);
            int experience = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05A93B30, [0x110, 0x38]), true);

            return new Dictionary<string, object>
            {
                { "health", health },
                { "maxhealth", maxhealth },
                { "level", level },
                { "xp", experience }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}