using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.KHDDD;

namespace MultiPresence.Presence
{
    public class KHDDD
    {
        Mem mem = new Mem();
        string process = "KINGDOM HEARTS Dream Drop Distance";
        private static DiscordRpcClient discord;
        public void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("906904369151213629");
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
                int world_get = mem.ReadByte($"{process}.exe+A3CE04");
                int room_get = mem.ReadByte($"{process}.exe+A3CE05");
                int difficulty_get = mem.ReadByte($"{process}.exe+A3CE02");
                int level = mem.ReadByte($"{process}.exe+A946D4");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var room = await Rooms.GetRoom(world[0]);

                discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                
                try
                {
                    discord.UpdateState($"{room[room_get]}");
                    discord.UpdateDetails($"Lv. {level} ({difficulty})");
                }
                catch
                {
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails(null);
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
