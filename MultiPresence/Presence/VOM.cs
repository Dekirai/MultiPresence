using DiscordRPC;
using Memory;
using MultiPresence.Models.VOM;
using MultiPresence.Properties;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VOM
    {
        static Mem mem = new Mem();
        static string process = "VisionsofMana-Win64-Shipping";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static string difficulty = "";
        public static string[] room = null;
        public static string[] world = null;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1279828464613982333");
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
                float hp = mem.ReadFloat($"{process}.exe+067B2800,0x30,0x228,0x338,0x9A8,0x140,0x0,0x48");
                float hpmax = mem.ReadFloat($"{process}.exe+067B2800,0x30,0x228,0x338,0x9A8,0x140,0x0,0x68");
                float mp = mem.ReadFloat($"{process}.exe+067B2800,0x30,0x228,0x338,0x9A8,0x140,0x0,0x88");
                float mpmax = mem.ReadFloat($"{process}.exe+067B2800,0x30,0x228,0x338,0x9A8,0x140,0x0,0x98");
                int difficulty_get = mem.ReadByte($"{process}.exe+0622DB50,0x490,0x688,0x10,0x140,0x88,0xC30");
                float level = mem.ReadFloat($"{process}.exe+067B2800,0x30,0x228,0x338,0x9A8,0x140,0x0,0x38");

                try
                {
                    if (Settings.Default.langDE == true)
                        difficulty = await Difficulties.GetDifficultyDE(difficulty_get);
                    else
                        difficulty = await Difficulties.GetDifficulty(difficulty_get);

                    var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "hp", hp },
                        { "hpmax", hpmax },
                        { "mp", mp },
                        { "mpmax", mp },
                        { "difficulty", difficulty }
                    };

                    if (level == 0)
                    {
                        discord.UpdateLargeAsset("logo", "Visions of Mana");
                        discord.UpdateSmallAsset("", "");
                        discord.UpdateDetails($"In Main Menu");
                        discord.UpdateState("");
                    }
                    else
                    {
                        string details = updater.UpdateDetails("Visions of Mana", placeholders);
                        string state = updater.UpdateState("Visions of Mana", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Visions of Mana", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Visions of Mana", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Visions of Mana", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Visions of Mana", placeholders);
                        discord.UpdateLargeAsset(largeasset, largeassettext);
                        discord.UpdateSmallAsset(smallasset, smallassettext);
                        discord.UpdateDetails(details);
                        discord.UpdateState(state);
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Visions of Mana");
                    discord.UpdateDetails($"In Main Menu");
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
