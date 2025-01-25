using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIRB
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1332349500572045312");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ff7rebirth_")[0];
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
            Process[] game = Process.GetProcessesByName("ff7rebirth_");
            if (game.Length > 0)
            {
                int level = Hypervisor.Read<byte>(0x70C1F60);
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x878]), true);
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x87C]), true);
                int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x880]), true);
                int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x871DD30, [0x6A0, 0x40, 0x48, 0x3A8, 0x884]), true);
                int chapter = Hypervisor.Read<byte>(0x7833FB2);

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "hp", hp },
                    { "maxhp", maxhp },
                    { "mp", mp },
                    { "maxmp", maxmp },
                    { "chapter", chapter }
                };

                discord.UpdateLargeAsset($"logo", $"Final Fantasy VII Rebirth");
                if (hp > 0)
                {
                    string details = updater.UpdateDetails("Final Fantasy VII Rebirth", placeholders);
                    string state = updater.UpdateState("Final Fantasy VII Rebirth", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Final Fantasy VII Rebirth", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Final Fantasy VII Rebirth", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Final Fantasy VII Rebirth", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Final Fantasy VII Rebirth", placeholders);
                    string button1text = updater.UpdateButton1Text("Final Fantasy VII Rebirth", placeholders);
                    string button2text = updater.UpdateButton2Text("Final Fantasy VII Rebirth", placeholders);
                    string button1url = updater.UpdateButton1URL("Final Fantasy VII Rebirth", placeholders);
                    string button2url = updater.UpdateButton2URL("Final Fantasy VII Rebirth", placeholders);
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