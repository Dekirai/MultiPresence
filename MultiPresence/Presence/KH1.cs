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
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("827214883190734889");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
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
                        var world = await Worlds.GetWorld(world_get);
                        var room = await Rooms.GetRoom(world[0]);
                        discord.UpdateLargeAsset(world[1], world[0]);

                        var placeholders = new Dictionary<string, object>
                        {
                            { "level", level },
                            { "room", room[room_get] },
                            { "world", world },
                            { "difficulty", difficulty }
                        };

                        string details = updater.UpdateDetails("Kingdom Hearts Final Mix", placeholders);
                        string state = updater.UpdateState("Kingdom Hearts Final Mix", placeholders);
                        discord.UpdateDetails(details);
                        discord.UpdateState(state);
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
