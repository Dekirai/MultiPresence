using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DiscordRPC;
using Memory;
using MultiPresence.Models.MSMR;

namespace MultiPresence.Presence
{
    public class MSMR
    {
        static Mem mem = new Mem();
        static string process = "Spider-Man";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1266485584822796331");
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
                int level = mem.ReadByte($"{process}.exe+5DB60A0");
                int location_get = mem.ReadByte($"{process}.exe+5DC06D0");

                float health_get = mem.ReadFloat($"{process}.exe+6D302D8");
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
                    discord.UpdateLargeAsset($"logo", $"Marvel's Spider-Man Remastered");
                    string details = updater.UpdateDetails("Marvel's Spider-Man Remastered", placeholders, "Default");
                    string state = updater.UpdateState("Marvel's Spider-Man Remastered", placeholders, "Default");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else
                {
                    discord.UpdateLargeAsset($"logo", $"Marvel's Spider-Man Remastered");
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