using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KH1;
using Newtonsoft.Json;

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
                int world_get = mem.ReadByte($"{process}.exe+233FE94");
                int room_get = mem.ReadByte($"{process}.exe+233FE8C");
                int difficulty_get = mem.ReadByte($"{process}.exe+2DFF78C");
                int level = mem.ReadByte($"{process}.exe+2DE9364");
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);


                if (room_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"Main Menu");
                    discord.UpdateState($"Main Menu");
                    discord.UpdateDetails("");
                }
                if (world_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"Main Menu");
                    discord.UpdateState($"Main Menu");
                    discord.UpdateDetails("");
                }
                else
                {

                    try
                    {
                        string json = JSONs.KHI_Locations_RAW;
                        dynamic jsonData = JsonConvert.DeserializeObject(json);

                        string world = jsonData[world_get.ToString()]["Name"];
                        string room = jsonData[world_get.ToString()]["Areas"][room_get];
                        string imagekey = jsonData[world_get.ToString()]["ImageKey"];
                        discord.UpdateLargeAsset(imagekey, world);
                        discord.UpdateDetails($"Lv. {level} ({difficulty})");
                        discord.UpdateState(room);
                    }
                    catch
                    {
                        discord.UpdateLargeAsset("logo", "Kingdom Hearts");
                        discord.UpdateDetails($"In Main Menu");
                        discord.UpdateState("");
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
