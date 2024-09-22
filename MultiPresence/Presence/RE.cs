using DiscordRPC;
using Memory;
using MultiPresence.Models.RE;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE
    {
        static Mem mem = new Mem();
        static string process = "bhd";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212466561068171294");
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
                int floor_get = mem.ReadInt("bhd.exe+0098A0B0,0x74,0x1C,0x48,0x4,0x314");
                int stage_get = mem.ReadInt("bhd.exe+0098A0B0,0x74,0x1C,0x48,0x4,0x318");
                int character_get = mem.ReadByte("bhd.exe+97C9C0,0x5118");
                var stagevalue = await Stages.GetStage(floor_get);

                string[] stage = stagevalue[stage_get].Split(':');

                var placeholders = new Dictionary<string, object>
                    {
                        { "floor", stage[0] },
                        { "room", stage[1] }
                    };

                string details = updater.UpdateDetails("Resident Evil", placeholders);
                string state = updater.UpdateState("Resident Evil", placeholders);
                string largeasset = updater.UpdateLargeAsset("Resident Evil", placeholders);
                string largeassettext = updater.UpdateLargeAssetText("Resident Evil", placeholders);
                string smallasset = updater.UpdateSmallAsset("Resident Evil", placeholders);
                string smallassettext = updater.UpdateSmallAssetText("Resident Evil", placeholders);
                string button1text = updater.UpdateButton1Text("Resident Evil", placeholders);
                string button2text = updater.UpdateButton2Text("Resident Evil", placeholders);
                string button1url = updater.UpdateButton1URL("Resident Evil", placeholders);
                string button2url = updater.UpdateButton2URL("Resident Evil", placeholders);
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

                if (floor_get > 0)
                {
                    if (character_get == 0)
                        discord.UpdateSmallAsset("chris", "Playing as Chris");
                    else if (character_get == 1)
                        discord.UpdateSmallAsset("jill", "Playing as Jill");
                    else if (character_get == 2)
                        discord.UpdateSmallAsset("rebecca", "Playing as Rebecca");
                }
                else
                    discord.UpdateSmallAsset("", "");

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