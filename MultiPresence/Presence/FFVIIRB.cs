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
            updater = new DiscordStatusUpdater("config.json");
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
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x878]), true);
                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Rebirth", placeholders);
                }
                else
                {
                    discord.UpdateDetails("In Main Menu");
                    discord.UpdateState("");
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
            int level = Hypervisor.Read<byte>(0x70C1F60);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x878]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x87C]), true);
            int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x880]), true);
            int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x884]), true);
            int chapter = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x092A0100, [0x20]), true);

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
            discord.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}