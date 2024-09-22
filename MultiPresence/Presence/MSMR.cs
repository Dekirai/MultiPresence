using DiscordRPC;
using Memory;
using MultiPresence.Models.MSMR;
using System.Diagnostics;

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
                    string details = updater.UpdateDetails("Marvel's Spider-Man Remastered", placeholders);
                    string state = updater.UpdateState("Marvel's Spider-Man Remastered", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Marvel's Spider-Man Remastered", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Marvel's Spider-Man Remastered", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Marvel's Spider-Man Remastered", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Marvel's Spider-Man Remastered", placeholders);
                    string button1text = updater.UpdateButton1Text("Marvel's Spider-Man Remastered", placeholders);
                    string button2text = updater.UpdateButton2Text("Marvel's Spider-Man Remastered", placeholders);
                    string button1url = updater.UpdateButton1URL("Marvel's Spider-Man Remastered", placeholders);
                    string button2url = updater.UpdateButton2URL("Marvel's Spider-Man Remastered", placeholders);
                    discord.UpdateLargeAsset(largeasset, largeassettext);
                    discord.UpdateSmallAsset(smallasset, smallassettext);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);

                    if (button1url.Length > 0 && button2url.Length == 0)
                    {
                        discord.UpdateButtons(new DiscordRPC.Button[]
                        {
                                new DiscordRPC.Button() { Label = button1text, Url = button1url }
                        });
                    }
                    else if (button1url.Length > 0 && button2url.Length > 0)
                    {
                        discord.UpdateButtons(new DiscordRPC.Button[]
                        {
                                new DiscordRPC.Button() { Label = button1text, Url = button1url },
                                new DiscordRPC.Button() { Label = button2text, Url = button2url }
                        });
                    }
                    else
                    {
                        discord.UpdateButtons(null);
                    }
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