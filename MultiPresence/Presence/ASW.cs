using DiscordRPC;
using Memory;
using MultiPresence.Models.ASW;
using System.Diagnostics;
using System.Text;

namespace MultiPresence.Presence
{
    public class ASW
    {
        static Mem mem = new Mem();
        static string process = "SoulWorker";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1264599106521927690");
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
                int level = mem.ReadByte($"{process}.exe+1792A28");
                byte[] nickname_base = mem.ReadBytes($"{process}.exe+16E2008", 24);
                string nickname = Encoding.Unicode.GetString(nickname_base);
                int character = mem.ReadByte($"{process}.exe+1792A2C");
                var character_name = await Characters.GetCharacter(character);

                var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "nickname", nickname },
                        { "character", character_name[0] },
                        { "character_icon_name", character_name[1] }
                    };

                if (character == 255 || level == 0)
                {
                    discord.UpdateLargeAsset($"logo", $"asobiSW");
                    discord.UpdateDetails($"In Main Menu");
                    discord.UpdateState($"");
                }
                else
                {
                    string details = updater.UpdateDetails("asobiSW", placeholders);
                    string state = updater.UpdateState("asobiSW", placeholders);
                    string largeasset = updater.UpdateLargeAsset("asobiSW", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("asobiSW", placeholders);
                    string smallasset = updater.UpdateSmallAsset("asobiSW", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("asobiSW", placeholders);
                    string button1text = updater.UpdateButton1Text("asobiSW", placeholders);
                    string button2text = updater.UpdateButton2Text("asobiSW", placeholders);
                    string button1url = updater.UpdateButton1URL("asobiSW", placeholders);
                    string button2url = updater.UpdateButton2URL("asobiSW", placeholders);
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