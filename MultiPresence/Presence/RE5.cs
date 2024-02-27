using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.RE5;

namespace MultiPresence.Presence
{
    public class RE5
    {
        static Mem mem = new Mem();
        static string process = "re5dx9";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212083539567186002");
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
                int stage_get = mem.ReadInt("re5dx9.exe+00DB2158,0x273D8");
                var stagevalue = await Stages.GetStage(stage_get);

                string[] stage = stagevalue.Split(':');

                discord.UpdateLargeAsset($"logo", $"Resident Evil 5");
                discord.UpdateDetails($"{stage[0]}");
                discord.UpdateState($"{stage[1]}");

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