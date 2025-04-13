using DiscordRPC;
using MultiPresence.Models.MSM2;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MSM2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1337729691951431795");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("Spider-Man2")[0];
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
            Process[] game = Process.GetProcessesByName("Spider-Man2");
            if (game.Length > 0)
            {
                try
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Marvel's Spider-Man 2", placeholders);
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Marvel's Spider-Man 2"
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
            int location_get = Hypervisor.Read<byte>(0x9A42D28);
            int level = Hypervisor.Read<byte>(0xB010A68);
            int character_get = Hypervisor.Read<byte>(0x9A3460C);
            var location = await Locations.GetLocations(location_get);
            string character = "";
            string character_icon = "";

            if (character_get == 0)
            {
                character = "Peter Parker";
                character_icon = "peter";
            }
            else if (character_get == 1)
            {
                character = "Miles Morales";
                character_icon = "miles";
            }


            return new Dictionary<string, object>
            {
                { "level", level },
                { "character", character },
                { "character_icon", character_icon },
                { "location", location }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}