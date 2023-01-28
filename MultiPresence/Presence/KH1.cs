using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KH1;

namespace MultiPresence.Presence
{
    public class KH1
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS FINAL MIX";
        private static DiscordRpcClient discord;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("827214883190734889");
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
                int isGummi = mem.ReadByte($"{process}.exe+50421D");
                int world_get = mem.ReadByte($"{process}.exe+233CB4C");
                int room_get = mem.ReadByte($"{process}.exe+233CB44");
                int difficulty_get = mem.ReadByte($"{process}.exe+2DFBDFC");
                int level = mem.ReadByte($"{process}.exe+2DE59D4");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var room = await Rooms.GetRoom(world[0]);


                if (room_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"Main Menu");
                    discord.UpdateState($"Main Menu");
                    discord.UpdateDetails(null);
                }
                if (world_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"Main Menu");
                    discord.UpdateState($"Main Menu");
                    discord.UpdateDetails(null);
                }
                else
                {
                    if (isGummi == 1)
                    {
                        discord.UpdateLargeAsset($"worldmap", $"Gummi Ship");
                        discord.UpdateState($"Gummi Ship");
                        discord.UpdateDetails($"Lv. {level} ({difficulty[0]})");
                    }
                    else
                    {
                        discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                        discord.UpdateState($"{room[room_get]}");
                        discord.UpdateDetails($"Lv. {level} ({difficulty[0]})");
                    }
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
