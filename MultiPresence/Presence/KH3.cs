using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KH3;
using MultiPresence.Models.KHIII;
using Newtonsoft.Json;

namespace MultiPresence.Presence
{
    public class KH3
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS III";
        public static string _room_address = "";
        static private DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            long room_get = (await mem.AoBScan("?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 53 63 72 69 70 74 2F 54 72 65 73 47 61 6D 65 2E 54 72 65 73 50 6C 61 79 65 72 43 6F 6E 74 72 6F 6C 6C 65 72 53 6F 72 61", true)).FirstOrDefault();
            _room_address = room_get.ToString("X8");
            discord = new DiscordRpcClient("827190870724837406");
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
                int world_get = mem.ReadByte($"{process}.exe+086D80A0,0x10,0x130,0x30,0x80,0xD0,0x1C0");
                string room_get = mem.ReadString($"{_room_address}", "", 5);
                int difficulty_get = mem.ReadByte($"{process}.exe+87ECC8C");
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);

                try
                {
                    var world = await Worlds.GetWorld(world_get);
                    var room = await Rooms.GetRoom(room_get);

                    discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                    discord.UpdateDetails($"Playing on {difficulty}");
                    discord.UpdateState($"{room}");
                    discord.UpdateSmallAsset("", "");
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Kingdom Hearts III");
                    discord.UpdateDetails($"In Main Menu");
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
