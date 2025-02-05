using DiscordRPC;
using MultiPresence.Models.KH1;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KH1
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("827214883190734889");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX")[0];
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
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS FINAL MIX");
            if (game.Length > 0)
            {
                int world_get = Hypervisor.Read<byte>(0x233FE94);
                int room_get = Hypervisor.Read<byte>(0x233FE8C);
                int battleflag = Hypervisor.Read<byte>(0x2867364);
                try
                {
                    if (room_get == 255 || world_get == 255)
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
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Final Mix", placeholders);
                        }
                        else if (battleflag == 1)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Final Mix", placeholders, "Battle");
                        }
                    }
                }
                catch
                {
                    discord.UpdateLargeAsset("logo", "Kingdom Hearts Final Mix");
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
            int world_get = Hypervisor.Read<byte>(0x233FE94);
            int room_get = Hypervisor.Read<byte>(0x233FE8C);
            int difficulty_get = Hypervisor.Read<byte>(0x2DFF78C);
            int level = Hypervisor.Read<byte>(0x2DE9364);
            int hp = Hypervisor.Read<int>(0x2D5CC4C);
            int hpmax = Hypervisor.Read<int>(0x2D5CC50);
            int mp = Hypervisor.Read<int>(0x2D5CC54);
            int mpmax = Hypervisor.Read<int>(0x2D5CC58);

            var world = await Worlds.GetWorld(world_get);
            var room = await Rooms.GetRoom(world[0]);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "mp", mp },
                { "mpmax", mp },
                { "room", room[room_get] },
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
