using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFXVI
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1285884197084336161");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy XVI.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ffxvi")[0];
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
            Process[] game = Process.GetProcessesByName("ffxvi");
            if (game.Length > 0)
            {
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x1813CE8, [0x50]), true);

                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy XVI", placeholders);
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
                            LargeImageText = "Final Fantasy XVI"
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
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x1813CE8, [0x50]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x1813CE8, [0x40]), true);
            int gil = Hypervisor.Read<int>(0x10A4072E6C);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x1813CE8, [0xCB50]), true);
            string difficulty = "";

            if (difficulty_get == 0)
                difficulty = "Easy";
            else if (difficulty_get == 1)
                difficulty = "Normal";
            else if (difficulty_get == 2)
                difficulty = "Final Fantasy";
            else if (difficulty_get == 3)
                difficulty = "Ultimaniac";

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "gil", gil },
                { "difficulty", difficulty }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}