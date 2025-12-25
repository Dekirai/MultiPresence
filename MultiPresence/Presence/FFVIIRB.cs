using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIRB
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1332349500572045312");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy VII Rebirth.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ff7rebirth_")[0];
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
            Process[] game = Process.GetProcessesByName("ff7rebirth_");
            if (game.Length > 0)
            {
                int hp = Hypervisor.Read<int>(0x71C0F30);
                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Rebirth", placeholders);
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
                            LargeImageText = "Final Fantasy VII Rebirth"
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
            int level = Hypervisor.Read<byte>(0x71C0F20);
            int hp = Hypervisor.Read<int>(0x71C0F30);
            int maxhp = Hypervisor.Read<int>(0x71C0F34);
            int mp = Hypervisor.Read<int>(0x71C0F38);
            int maxmp = Hypervisor.Read<int>(0x71C0F3C);
            int chapter = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x939B9E0, [0x20]), true);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "maxhp", maxhp },
                { "mp", mp },
                { "maxmp", maxmp },
                { "chapter", chapter }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}