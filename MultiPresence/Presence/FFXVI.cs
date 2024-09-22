using DiscordRPC;
using Memory;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFXVI
    {
        static Mem mem = new Mem();
        static string process = "ffxvi";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        private static bool button1 = false;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1285884197084336161");
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
                int hp = mem.ReadInt($"{process}.exe+02752328,0x100,0x278,0xC8,0x1E8,0x0,0x7A8,0x248");
                int level = mem.ReadInt($"{process}.exe+02752328,0x100,0x278,0xC8,0x1E8,0x0,0x7A8,0x254");
                int gil = mem.ReadInt($"10A4072E6C");
                int difficulty_get = mem.ReadByte($"{process}.exe+1817CE8,0xCB50");
                string difficulty = "";

                if (difficulty_get == 0)
                    difficulty = "Easy";
                else if (difficulty_get == 1)
                    difficulty = "Normal";
                else if (difficulty_get == 2)
                    difficulty = "Final Fantasy";
                else if (difficulty_get == 3)
                    difficulty = "Ultimaniac";

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "hp", hp },
                    { "gil", gil },
                    { "difficulty", difficulty }
                };

                if (hp > 0)
                {
                    string details = updater.UpdateDetails("Final Fantasy XVI", placeholders);
                    string state = updater.UpdateState("Final Fantasy XVI", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Final Fantasy XVI", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Final Fantasy XVI", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Final Fantasy XVI", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Final Fantasy XVI", placeholders);
                    string button1text = updater.UpdateButton1Text("Final Fantasy XVI", placeholders);
                    string button2text = updater.UpdateButton2Text("Final Fantasy XVI", placeholders);
                    string button1url = updater.UpdateButton1URL("Final Fantasy XVI", placeholders);
                    string button2url = updater.UpdateButton2URL("Final Fantasy XVI", placeholders);
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