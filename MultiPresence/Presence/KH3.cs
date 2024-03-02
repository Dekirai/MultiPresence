using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KH3;
using Newtonsoft.Json;

namespace MultiPresence.Presence
{
    public class KH3
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS III";
        static private DiscordRpcClient discord;
        public static void DoAction()
        {
            GetPID();
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
                int world_get = mem.ReadByte($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x8664");
                int room_get = mem.ReadByte($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x8668");
                int difficulty_get = mem.ReadByte($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x8660");
                int playtime_get = mem.ReadInt($"{process}.exe+09D2E310,0x2B0,0x228,0x70,0x140,0x866C");
                TimeSpan playtime_convert = TimeSpan.FromSeconds(playtime_get);
                string playtime = playtime_convert.ToString(@"hh\:mm\:ss");
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);

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
                                if (level_gummi == 0)
                                    discord.UpdateDetails($"Preparing for Gummi Mission");
                                else
                                    discord.UpdateDetails($"Gummi Lv. {level_gummi} ({difficulty})");
                            }
                            catch
                            {
                                discord.UpdateDetails($"Preparing for Gummi Mission");
                            }
                        }
                        discord.UpdateLargeAsset("worldmap", "World Map");
                        discord.UpdateState($"World Map");
                        discord.UpdateSmallAsset("", "");
                    }
                    else
                    {
                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                string json = await client.GetStringAsync(JSONs.KHIII_Locations_URL);
                                dynamic jsonData = JsonConvert.DeserializeObject(json);

                                int level = mem.ReadByte($"{process}.exe+0A70B4A0,0x48,0x458,0x188,0x1B8,0x4D0,0x40");
                                string world = jsonData[world_get.ToString()]["Name"];
                                string room = jsonData[world_get.ToString()]["Areas"][room_get];
                                string imagekey = jsonData[world_get.ToString()]["ImageKey"];

                                discord.UpdateLargeAsset(imagekey, world);
                                discord.UpdateDetails($"Lv. {level} ({difficulty})");
                                discord.UpdateState(room);
                            }
                        }
                        catch
                        {
                            int level = mem.ReadByte($"{process}.exe+0A70B4A0,0x48,0x458,0x188,0x1B8,0x4D0,0x40");
                            discord.UpdateLargeAsset("logo", "Kingdom Hearts III");
                            discord.UpdateDetails($"Lv. {level} ({difficulty})");
                            discord.UpdateState($"Playtime: {playtime}");
                        }
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
