using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.KH3;

namespace MultiPresence.Presence
{
    public class KH3
    {
        Mem mem = new Mem();
        string process = "KINGDOM HEARTS III";
        private static DiscordRpcClient discord;
        public void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("827190870724837406");
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
                int world_get = mem.ReadByte($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x8664");
                //int room_get = mem.ReadByte($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x8668");
                int difficulty_get = mem.ReadByte($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x8660");
                int playtime_get = mem.ReadInt($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x866C");
                TimeSpan playtime_convert = TimeSpan.FromSeconds(playtime_get);
                string playtime = playtime_convert.ToString(@"hh\:mm\:ss");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);


                discord.UpdateLargeAsset($"{world[1]}", $"Playtime: {playtime}");

                try
                {
                    if (world_get == 27 || world_get == 28)
                    {
                        int level = mem.ReadByte($"{process}.exe+0A70B4A0,0x48,0x458,0x188,0x1B8,0x4D0,0x40");
                        if (level > 0 && level < 100)
                            discord.UpdateDetails($"Lv. {level} ({difficulty})");
                        else
                        {
                            try
                            {
                                int level_gummi = mem.ReadInt($"{process}.exe+0A70B4A0,0x48,0x470,0x550,0x250,0xD0,0x228,0x16C");
                                discord.UpdateDetails($"Gummi Lv. {level_gummi} ({difficulty})");
                            }
                            catch
                            {
                                discord.UpdateDetails($"Preparing for Gummi Mission");
                            }
                        }
                        discord.UpdateState($"{world[0]}");
                        discord.UpdateSmallAsset("", "");
                    }
                    else
                    {
                        if (world[0] == "Undefined location")
                        {
                            discord.UpdateDetails($"In Main Menu");
                            discord.UpdateState(null);
                        }
                        else
                        {
                            int level = mem.ReadByte($"{process}.exe+0A70B4A0,0x48,0x458,0x188,0x1B8,0x4D0,0x40");
                            //if (room_get > 5)
                            //    discord.UpdateState($"In a cutscene");
                            //else
                            //{
                            //    var room = await Rooms.GetRoom(world[0]);
                            //    discord.UpdateState($"{room[room_get]}");
                            //}
                            discord.UpdateState($"{world[0]}");
                            //discord.UpdateSmallAsset("logo",  $"Playtime: {playtime}");
                            discord.UpdateDetails($"Lv. {level} ({difficulty})");
                        }

                    }
                }
                catch
                {
                    discord.UpdateState(null);
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
