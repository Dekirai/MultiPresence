using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.RE;

namespace MultiPresence.Presence
{
    public class RE
    {
        static Mem mem = new Mem();
        static string process = "bhd";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212466561068171294");
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
                int floor_get = mem.ReadInt("bhd.exe+0098A0B0,0x74,0x1C,0x48,0x4,0x314");
                int stage_get = mem.ReadInt("bhd.exe+0098A0B0,0x74,0x1C,0x48,0x4,0x318");
                int character_get = mem.ReadByte("bhd.exe+97C9C0,0x5118");
                var stagevalue = await Stages.GetStage(floor_get);

                string[] stage = stagevalue[stage_get].Split(':');

                discord.UpdateLargeAsset($"logo", $"Resident Evil");
                discord.UpdateDetails($"{stage[0]}");
                discord.UpdateState($"{stage[1]}");

                if (floor_get > 0)
                {
                    if (character_get == 0)
                        discord.UpdateSmallAsset("chris", "Playing as Chris");
                    else if (character_get == 1)
                        discord.UpdateSmallAsset("jill", "Playing as Jill");
                    else if (character_get == 2)
                        discord.UpdateSmallAsset("rebecca", "Playing as Rebecca");
                }
                else
                    discord.UpdateSmallAsset("", "");

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