using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIR
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
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
            try
            {
                var _myProcess = Process.GetProcessesByName("ff7remake_")[0];
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
            Process[] game = Process.GetProcessesByName("ff7remake_");
            if (game.Length > 0)
            {
                int level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x057CA5E8, [0x8A0]), true);
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B0]), true);
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B4]), true);
                int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B8]), true);
                int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8BC]), true);

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