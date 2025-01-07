using DiscordRPC;
using MultiPresence.Models.RE5;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE5
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212083539567186002");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re5dx9")[0];
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
            Process[] game = Process.GetProcessesByName("re5dx9");
            if (game.Length > 0)
            {
                int stage_get = Hypervisor.Read<int>(Hypervisor.GetPointer32(0x00DB2158, [0x273D8]), true);
                int chris_health = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x00DB27DC, [0x24, 0x1364]), true);
                int sheva_health = Hypervisor.Read<short>(Hypervisor.GetPointer32(0x00DB27DC, [0x28, 0x1364]), true);
                var stagevalue = await Stages.GetStage(stage_get);

                string[] stage = stagevalue.Split(':');

                var placeholders = new Dictionary<string, object>
                    {
                        { "chapter", stage[0] },
                        { "room", stage[1] },
                        { "chris_health", chris_health },
                        { "sheva_health", sheva_health }
                    };

                discord.UpdateLargeAsset($"logo", $"Resident Evil 5");
                if (chris_health > 0 || sheva_health > 0)
                {
                    string details = updater.UpdateDetails("Resident Evil 5", placeholders);
                    string state = updater.UpdateState("Resident Evil 5", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Resident Evil 5", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Resident Evil 5", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Resident Evil 5", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Resident Evil 5", placeholders);
                    string button1text = updater.UpdateButton1Text("Resident Evil 5", placeholders);
                    string button2text = updater.UpdateButton2Text("Resident Evil 5", placeholders);
                    string button1url = updater.UpdateButton1URL("Resident Evil 5", placeholders);
                    string button2url = updater.UpdateButton2URL("Resident Evil 5", placeholders);
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
                    discord.UpdateDetails("In Menus");

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