using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DMC1
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1358367799285649418");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("dmc1")[0];
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
            Process[] game = Process.GetProcessesByName("dmc1");
            if (game.Length > 0)
            {
                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Devil May Cry", placeholders);

                await Task.Delay(1000);
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
            int health = Hypervisor.Read<int>(0x001378BC, true);
            int redorbs = Hypervisor.Read<int>(0x001378FC, true);
            int difficulty_get = Hypervisor.Read<byte>(0x00102066, true);
            int mission = Hypervisor.Read<byte>(0x00102064, true);

            string difficulty = difficulty_get switch
            {
                3 => "Normal",
                5 => "Hard",
                6 => "Dante Must Die"
            };

            return new Dictionary<string, object>
            {
                { "redorbs", redorbs },
                { "mission", mission },
                { "difficulty", difficulty },
                { "health", health }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}