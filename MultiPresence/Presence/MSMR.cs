using DiscordRPC;
using MultiPresence.Models.MSMR;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MSMR
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
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
            try
            {
                var _myProcess = Process.GetProcessesByName("Spider-Man")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("Spider-Man");
            if (game.Length > 0)
            {
                float health_get = Hypervisor.Read<float>(0x6D302D8);
                int health = (int)Math.Floor(health_get);

                if (health > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Marvel's Spider-Man Remastered", placeholders);
                }
                else
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Marvel's Spider-Man Remastered"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int level = Hypervisor.Read<byte>(0x671CA70);
            int location_get = Hypervisor.Read<byte>(0x6724900);
            float health_get = Hypervisor.Read<float>(0x7796D68);
            var location = await Locations.GetLocations(location_get);
            int health = (int)Math.Floor(health_get);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "health", health },
                { "location", location }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}