using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIR
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1270065791957471242");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ff7remake_")[0];
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
            Process[] game = Process.GetProcessesByName("ff7remake_");
            if (game.Length > 0)
            {
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B0]), true);

                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Remake", placeholders);
                }
                else
                {
                    discord.UpdateDetails("In Main Menu");
                    discord.UpdateState("");
                }

                await Task.Delay(300);
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
            int level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x057CA5E8, [0x8A0]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B0]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B4]), true);
            int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B8]), true);
            int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8BC]), true);
            int chapter = Hypervisor.Read<byte>(0x59ADBF0);

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