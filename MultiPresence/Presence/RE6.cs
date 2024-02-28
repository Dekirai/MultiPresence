using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.RE6;

namespace MultiPresence.Presence
{
    public class RE6
    {
        static Mem mem = new Mem();
        static string process = "BH6";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212349543463518268");
            InitializeDiscord();
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
                int stage_get = mem.Read2Byte("BH6.exe+1466884,0xA422C");
                int state_get = mem.Read2Byte("BH6.exe+1466884,0xA4228");
                var stagevalue = await Stages.GetStage(stage_get);

                string[] stage = stagevalue.Split(':');

                if (state_get == 0)
                {
                    discord.UpdateDetails($"Starting the game...");
                    discord.UpdateState($"");
                }
                else if (state_get > 1 && state_get < 9)
                {
                    discord.UpdateDetails($"{stage[0]}");
                    discord.UpdateState($"{stage[1]}");
                }
                else if (state_get == 20)
                {
                    discord.UpdateDetails($"In Main Menu");
                    discord.UpdateState($"");
                }
                else if (state_get == 10)
                {
                    discord.UpdateDetails($"Saving...");
                    discord.UpdateState($"");
                }
                else if (state_get > 20)
                {
                    discord.UpdateDetails($"{stage[0]}");
                    discord.UpdateState($"In a cutscene");
                }

                discord.UpdateLargeAsset($"logo", $"Resident Evil 6");


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