using DiscordRPC;
using MultiPresence.Models.KHCOM;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHCOM
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1342148362471866460");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS Re_Chain of Memories")[0];
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
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS Re_Chain of Memories");
            if (game.Length > 0)
            {
                int world_get = Hypervisor.Read<byte>(0x87B862);
                int battleflag = Hypervisor.Read<byte>(0x87B858);

                try
                {
                    if (world_get == 255)
                    {
                        discord.UpdateLargeAsset($"logo", $"Main Menu");
                        discord.UpdateState($"Main Menu");
                        discord.UpdateDetails("");
                    }
                    else
                    {
                        if (battleflag == 0)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Re:Chain of Memories", placeholders);
                        }
                        else if (battleflag == 1)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Re:Chain of Memories", placeholders, "Battle");
                        }
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Kingdom Hearts Re:Chain of Memories");
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int world_get = Hypervisor.Read<byte>(0x87B862);
            int difficulty_get = Hypervisor.Read<byte>(0xC87660);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x87B380, [0x444]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x87B380, [0x42C]), true);
            int hpmax = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x87B380, [0x430]), true);

            var world = await Worlds.GetWorld(world_get);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty }
            };
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
