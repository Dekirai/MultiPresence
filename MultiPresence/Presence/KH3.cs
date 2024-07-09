using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KH3;
using MultiPresence.Models.KHIII;
using Newtonsoft.Json;
using MultiPresence.Properties;

namespace MultiPresence.Presence
{
    public class KH3
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS III";
        public static string _room_address = "";
        static private DiscordRpcClient discord;
        public static string difficulty = "";
        public static string room = null;
        public static string[] world = null;
        public static async void DoAction()
        {
            await Task.Delay(7500); // Wait 7.5 seconds to make sure that the AoB is actually findable
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
                if (Settings.Default.langDE == true)
                    difficulty = await Difficulties.GetDifficultyDE(difficulty_get);
                else
                    difficulty = await Difficulties.GetDifficulty(difficulty_get);

                try
                {
                    if (Settings.Default.langDE == true)
                    {
                        room = await Rooms.GetRoomDE(room_get);
                        world = await Worlds.GetWorldDE(room_get.Split('_')[0]);
                    }
                    else
                    {
                        room = await Rooms.GetRoom(room_get);
                        world = await Worlds.GetWorld(room_get.Split('_')[0]);
                    }

                    if (world[0] == "Main Menu")
                    {
                        discord.UpdateLargeAsset("logo", "Kingdom Hearts III");
                        discord.UpdateDetails($"In Main Menu");
                        discord.UpdateState("");
                    }
                    else
                    {
                        discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");

                        if (room_get.Contains("wm"))
                            discord.UpdateDetails($"Playing on {difficulty}");
                        else if (room_get.Contains("gm"))
                        {
                            int gummilevel = mem.ReadByte($"{process}.exe+09D8E920,0x48,0x470,0x550,0x250,0xD0,0x228,0x16C");
                            discord.UpdateDetails($"Gummi Lv. {gummilevel} ({difficulty})");
                        }
                        else
                        {
                            int level = mem.ReadByte($"{process}.exe+09D8E920,0x48,0x458,0x188,0x1B8,0x4D0,0x40");
                            discord.UpdateDetails($"Lv. {level} ({difficulty})");
                        }
                        discord.UpdateState($"{room}");
                        discord.UpdateSmallAsset("", "");
                    }
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
