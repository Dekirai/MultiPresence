using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.MM11;

namespace MultiPresence.Presence
{
    public class MM11
    {
        static Mem mem = new Mem();
        static string process = "game";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("981534050781122570");
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
                string _save = "140C3F6C0";
                string _game = "140B87A20";
                int lives = mem.ReadByte($"{_save},0x3A40");
                int difficulty = mem.ReadByte($"{_save},0x388C");
                int stage = mem.ReadByte($"{_game},0xDF0,0xA8,0x18,0xA0");

                var stagename = await Stages.GetStage(stage);
                var difficultyname = await Difficulties.GetDifficulty(difficulty);

                discord.UpdateLargeAsset($"{stagename[1]}", $"{stagename[0]}");
                discord.UpdateDetails($"Lives: {lives} ({difficultyname[0]})");
                discord.UpdateState($"{stagename[0]}");

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