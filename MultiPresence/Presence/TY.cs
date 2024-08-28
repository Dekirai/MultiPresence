using DiscordRPC;
using Memory;
using MultiPresence.Models.TY;
using MultiPresence.Properties;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TY
    {
        static Mem mem = new Mem();
        static string process = "TY";
        private static DiscordRpcClient discord;
        public static string[] levelvalue = null;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("983292674863943720");
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
                int health = mem.ReadByte("TY.exe+2737CC");
                int level = mem.ReadByte("TY.exe+28903C");
                int opals = mem.ReadByte("TY.exe+2888B0");

                if (Settings.Default.langDE == true)
                    levelvalue = await Levels.GetLevelDE(level);
                else
                    levelvalue = await Levels.GetLevel(level);

                discord.UpdateLargeAsset($"logo", $"TY the Tasmanian Tiger");
                if (level == 4 || level == 5 || level == 6 || level == 8 || level == 9 || level == 10 || level == 12 || level == 13 || level == 14)
                    discord.UpdateDetails($"HP: {health} | Opals: {opals}/300");
                else
                    discord.UpdateDetails($"HP: {health}");
                discord.UpdateState($"{levelvalue[0]}");

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