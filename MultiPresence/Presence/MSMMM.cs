using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DiscordRPC;
using Memory;
using MultiPresence.Models.MSMMM;

namespace MultiPresence.Presence
{
    public class MSMMMM
    {
        static Mem mem = new Mem();
        static string process = "MilesMorales";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1266464310360670241");
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
                int level = mem.ReadByte($"{process}.exe+671CA70");
                int location_get = mem.ReadByte($"{process}.exe+6724900");

                float health_get = mem.ReadFloat($"{process}.exe+7796D68");
                var location = await Locations.GetLocations(location_get);
                int health = (int)Math.Floor(health_get);

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "health", health },
                    { "location", location }
                };

                if (health > 0)
                {
                    discord.UpdateLargeAsset($"logo", $"Marvel's Spider-Man: Miles Morales");
                    string details = updater.UpdateDetails("Marvel's Spider-Man: Miles Morales", placeholders, "Default");
                    string state = updater.UpdateState("Marvel's Spider-Man: Miles Morales", placeholders, "Default");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else
                {
                    discord.UpdateLargeAsset($"logo", $"Marvel's Spider-Man: Miles Morales");
                    discord.UpdateDetails("Loading...");
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