using DiscordRPC;
using Memory;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIR
    {
        static Mem mem = new Mem();
        static string process = "ff7remake_";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1270065791957471242");
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
                int level = mem.ReadByte($"{process}.exe+057B9268,0x8A0");
                int hp = mem.ReadInt($"{process}.exe+057B9268,0x8B0");
                int maxhp = mem.ReadInt($"{process}.exe+057B9268,0x8B4");
                int mp = mem.ReadInt($"{process}.exe+057B9268,0x8B8");
                int maxmp = mem.ReadInt($"{process}.exe+057B9268,0x8BC");

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "hp", hp },
                    { "maxhp", maxhp },
                    { "mp", mp },
                    { "maxmp", maxmp }
                };

                discord.UpdateLargeAsset($"logo", $"Final Fantasy VII Remake");
                if (hp > 0)
                {
                    string details = updater.UpdateDetails("Final Fantasy VII Remake", placeholders);
                    string state = updater.UpdateState("Final Fantasy VII Remake", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Final Fantasy VII Remake", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Final Fantasy VII Remake", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Final Fantasy VII Remake", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Final Fantasy VII Remake", placeholders);
                    string button1text = updater.UpdateButton1Text("Final Fantasy VII Remake", placeholders);
                    string button2text = updater.UpdateButton2Text("Final Fantasy VII Remake", placeholders);
                    string button1url = updater.UpdateButton1URL("Final Fantasy VII Remake", placeholders);
                    string button2url = updater.UpdateButton2URL("Final Fantasy VII Remake", placeholders);
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
                    discord.UpdateDetails("In Main Menu");
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