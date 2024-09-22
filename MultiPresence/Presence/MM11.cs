using DiscordRPC;
using Memory;
using MultiPresence.Models.MM11;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class MM11
    {
        static Mem mem = new Mem();
        static string process = "game";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("981534050781122570");
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
                string _save = "140C3F6C0";
                string _game = "140B87A20";
                int lives = mem.ReadByte($"{_save},0x3A40");
                int difficulty = mem.ReadByte($"{_save},0x388C");
                int stage = mem.ReadByte($"{_game},0xDF0,0xA8,0x18,0xA0");

                var stagename = await Stages.GetStage(stage);
                var difficultyname = await Difficulties.GetDifficulty(difficulty);

                var placeholders = new Dictionary<string, object>
                    {
                        { "lives", lives },
                        { "difficulty", difficultyname[0] },
                        { "stage", stagename[0] },
                        { "stage_icon_name", stagename[1] }
                    };

                string details = updater.UpdateDetails("Mega Man 11", placeholders);
                string state = updater.UpdateState("Mega Man 11", placeholders);
                string largeasset = updater.UpdateLargeAsset("Mega Man 11", placeholders);
                string largeassettext = updater.UpdateLargeAssetText("Mega Man 11", placeholders);
                string smallasset = updater.UpdateSmallAsset("Mega Man 11", placeholders);
                string smallassettext = updater.UpdateSmallAssetText("Mega Man 11", placeholders);
                string button1text = updater.UpdateButton1Text("Mega Man 11", placeholders);
                string button2text = updater.UpdateButton2Text("Mega Man 11", placeholders);
                string button1url = updater.UpdateButton1URL("Mega Man 11", placeholders);
                string button2url = updater.UpdateButton2URL("Mega Man 11", placeholders);
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