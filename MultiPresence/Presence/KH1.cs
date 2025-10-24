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
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts I.json");
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
                bool isEpicGames = false;
                int world_get = 0;
                int room_get = 0;
                int battleflag = 0;

                string version = Hypervisor.ReadString(0x46A822, 8);
                if (version.Contains("japanese"))
                    isEpicGames = true;

                if (isEpicGames)
                {
                    world_get = Hypervisor.Read<byte>(0x2340ECC);
                    room_get = Hypervisor.Read<byte>(0x2340EC4);
                    battleflag = Hypervisor.Read<byte>(0x2867CD8);
                }
                else
                {
                    world_get = Hypervisor.Read<byte>(0x233FE94);
                    room_get = Hypervisor.Read<byte>(0x233FE8C);
                    battleflag = Hypervisor.Read<byte>(0x2867364);
                }
                try
                {
                    if (room_get == 255 || world_get == 255)
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Kingdom Hearts Final Mix"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                    else
                    {
                        if (battleflag == 1)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Final Mix", placeholders, "Battle");
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Final Mix", placeholders);
                        }
                    }
                }
                catch
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Kingdom Hearts Final Mix"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            bool isEpicGames = false;

            int world_get = 0;
            int room_get = 0;
            int difficulty_get = 0;
            int level = 0;
            int hp = 0;
            int hpmax = 0;
            int mp = 0;
            int mpmax = 0;

            string version = Hypervisor.ReadString(0x46A822, 8);
            if (version.Contains("japanese"))
                isEpicGames = true;

            if (isEpicGames)
            {
                world_get = Hypervisor.Read<byte>(0x2340ECC);
                room_get = Hypervisor.Read<byte>(0x2340EC4);
                difficulty_get = Hypervisor.Read<byte>(0x2DFF78C + 0xA00);
                level = Hypervisor.Read<byte>(0x2DE9364 + 0xA00);
                hp = Hypervisor.Read<int>(0x2D5CC4C + 0xA00);
                hpmax = Hypervisor.Read<int>(0x2D5CC50 + 0xA00);
                mp = Hypervisor.Read<int>(0x2D5CC54 + 0xA00);
                mpmax = Hypervisor.Read<int>(0x2D5CC58 + 0xA00);
            }
            else
            {
                world_get = Hypervisor.Read<byte>(0x233FE94);
                room_get = Hypervisor.Read<byte>(0x233FE8C);
                difficulty_get = Hypervisor.Read<byte>(0x2DFF78C);
                level = Hypervisor.Read<byte>(0x2DE9364);
                hp = Hypervisor.Read<int>(0x2D5CC4C);
                hpmax = Hypervisor.Read<int>(0x2D5CC50);
                mp = Hypervisor.Read<int>(0x2D5CC54);
                mpmax = Hypervisor.Read<int>(0x2D5CC58);
            }

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
        }
    }
}
