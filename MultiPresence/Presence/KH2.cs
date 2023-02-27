using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KH2;
using Newtonsoft.Json;

namespace MultiPresence.Presence
{
    public class KH2
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS II FINAL MIX";
        private static DiscordRpcClient discord;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("826145131152408625");
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
                int world_get = mem.ReadByte($"{process}.exe+714DB8");
                int room_get = mem.ReadByte($"{process}.exe+714DB9");
                int difficulty_get = mem.ReadByte($"{process}.exe+9A9548");
                int level = mem.ReadByte($"{process}.exe+9A95AF");
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string json = await client.GetStringAsync(JSONs.KHII_Locations_URL);
                        dynamic jsonData = JsonConvert.DeserializeObject(json);

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
                    discord.UpdateLargeAsset("logo", "");
                    discord.UpdateDetails($"");
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
