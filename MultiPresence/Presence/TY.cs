using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.TY;

namespace MultiPresence.Presence
{
    public class TY
    {
        Mem mem = new Mem();
        string process = "TY";
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("983292674863943720");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                int health = mem.ReadByte("TY.exe+2737CC");
                int level = mem.ReadByte("TY.exe+28903C");
                var levelvalue = await Levels.GetLevel(level);
                int opals = mem.ReadByte("TY.exe+2888B0");

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
                Buttons = new Button[]
                {
#if DEBUG
                    new Button() { Label = $"Powered by MultiPresence", Url = "https://github.com/Dekirai/MultiPresence" }
#endif
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}