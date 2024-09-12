using DiscordRPC;
using Memory;
using MultiPresence.Models.VS;
using MultiPresence.Properties;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VS
    {
        static Mem mem = new Mem();
        static string process = "VampireSurvivors";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1283783524423438409");
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
                int characterid = mem.ReadInt("GameAssembly.dll+048F7858,0x78,0xB8,0x0,0x40,0x10,0x50,0x44");
                int stageid = mem.ReadInt("GameAssembly.dll+048F7858,0x78,0xB8,0x0,0x40,0x10,0x50,0x48");
                float coins = mem.ReadFloat("GameAssembly.dll+048F7858,0x78,0xB8,0x0,0x40,0x10,0x50,0x70");
                float coinsingame = mem.ReadFloat("GameAssembly.dll+048F7858,0x78,0xB8,0x0,0x40,0x10,0x50,0x74");
                int kills = mem.ReadInt("GameAssembly.dll+048F7858,0x78,0xB8,0x0,0x40,0x10,0x50,0x78");
                float health = mem.ReadFloat("GameAssembly.dll+049541B8,0xB8,0x0,0xF0,0x98,0x18,0x28,0x188");
                int level = mem.ReadInt("GameAssembly.dll+049541B8,0xB8,0x0,0xF0,0x98,0x18,0x28,0x18C");
                float time = mem.ReadFloat("GameAssembly.dll+04962070,0x80,0x78,0x48,0x40,0xB8,0x0,0x360");
                int isIngame = mem.ReadByte("UnityPlayer.dll+1B32778"); //???? Idk if it's actually ingame check

                TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                string formattedTime = string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
                
                var character = await Characters.GetCharacter(characterid);
                string char_first = character.Split(' ')[0];
                string char_last = character.Split(' ')[1];
                var stage = await Stages.GetStages(stageid);

                try
                {
                    var placeholders = new Dictionary<string, object>
                    {
                        { "character", character },
                        { "firstname", char_first },
                        { "lastname", char_last },
                        { "stage", stage },
                        { "coins", (int)Math.Floor(coins) },
                        { "coinsingame", (int)Math.Floor(coinsingame) },
                        { "kills", kills },
                        { "health", (int)Math.Floor(health) },
                        { "level", level },
                        { "time", formattedTime }
                    };

                    if (isIngame == 0)
                    {
                        string details = updater.UpdateDetails("Vampire Survivors", placeholders);
                        string state = updater.UpdateState("Vampire Survivors", placeholders);
                        string largeasset = updater.UpdateLargeAsset("Vampire Survivors", placeholders);
                        string largeassettext = updater.UpdateLargeAssetText("Vampire Survivors", placeholders);
                        string smallasset = updater.UpdateSmallAsset("Vampire Survivors", placeholders);
                        string smallassettext = updater.UpdateSmallAssetText("Vampire Survivors", placeholders);
                        discord.UpdateLargeAsset(largeasset, largeassettext);
                        discord.UpdateSmallAsset(smallasset, smallassettext);
                        discord.UpdateDetails(details);
                        discord.UpdateState(state);
                    }
                    else
                    {
                        string details = updater.UpdateDetails("Vampire Survivors", placeholders, "Ingame");
                        string state = updater.UpdateState("Vampire Survivors", placeholders, "Ingame");
                        string largeasset = updater.UpdateLargeAsset("Vampire Survivors", placeholders, "Ingame");
                        string largeassettext = updater.UpdateLargeAssetText("Vampire Survivors", placeholders, "Ingame");
                        string smallasset = updater.UpdateSmallAsset("Vampire Survivors", placeholders, "Ingame");
                        string smallassettext = updater.UpdateSmallAssetText("Vampire Survivors", placeholders, "Ingame");
                        discord.UpdateLargeAsset(largeasset, largeassettext);
                        discord.UpdateSmallAsset(smallasset, smallassettext);
                        discord.UpdateDetails(details);
                        discord.UpdateState(state);
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Vampire Survivors");
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
