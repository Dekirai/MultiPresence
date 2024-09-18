using DiscordRPC;
using Memory;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFXVI
    {
        static Mem mem = new Mem();
        static string process = "ffxvi";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            discord = new DiscordRpcClient("1285884197084336161");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                int hp = mem.ReadInt($"{process}.exe+02752328,0x100,0x278,0xC8,0x1E8,0x0,0x7A8,0x248");
                int level = mem.ReadInt($"{process}.exe+02752328,0x100,0x278,0xC8,0x1E8,0x0,0x7A8,0x254");
                int gil = mem.ReadInt($"10A4072E6C");
                int difficulty_get = mem.ReadByte($"{process}.exe+1817CE8,0xCB50");
                string difficulty = "";

                if (difficulty_get == 0)
                    difficulty = "Normal";
                else if (difficulty_get == 1)
                    difficulty = "Hard";
                else if (difficulty_get == 2)
                    difficulty = "Final Fantasy";
                else if (difficulty_get == 3)
                    difficulty = "Ultimaniac";

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "hp", hp },
                    { "gil", gil },
                    { "difficulty", difficulty }
                };

                discord.UpdateLargeAsset($"logo", $"FINAL FANTASY XVI");
                if (hp > 0)
                {
                    string details = updater.UpdateDetails("Final Fantasy XVI", placeholders);
                    string state = updater.UpdateState("Final Fantasy XVI", placeholders);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
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