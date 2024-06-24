using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KHDDD;

namespace MultiPresence.Presence
{
    public class KHDDD
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS Dream Drop Distance";
        private static DiscordRpcClient discord;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("906904369151213629");
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
                int world_get = mem.ReadByte($"{process}.exe+A40724");
                int room_get = mem.ReadByte($"{process}.exe+A40725");
                int difficulty_get = mem.ReadByte($"{process}.exe+A40722");
                int character_get = mem.ReadByte($"{process}.exe+A40720");
                int level = mem.ReadByte($"{process}.exe+A97FF4");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var room = await Rooms.GetRoom(world[0]);
                string character = "";

                if (character_get == 0)
                    character = "Sora";
                else
                    character = "Riku";

                discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                
                try
                {
                    discord.UpdateState($"{room[room_get]}");
                    discord.UpdateDetails($"Lv. {level} ({difficulty})");
                    discord.UpdateSmallAsset($"{character_get}", $"Playing as {character}");
                }
                catch
                {
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails($"Lv. {level} ({difficulty})");
                    discord.UpdateSmallAsset($"", $"");
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
