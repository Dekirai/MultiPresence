using DiscordRPC;
using MultiPresence.Models.KHBBS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHBBS
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("839545395368820806");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts Birth by Sleep.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX")[0];
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
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            if (game.Length > 0)
            {
                int world_get = Hypervisor.Read<byte>(0x818120);
                var world = await Worlds.GetWorld(world_get);
                var room = await Rooms.GetRoom(world[0]);
                int battleflag = Hypervisor.Read<byte>(0x10F9EE48);

                try
                {
                    if (battleflag == 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    }
                    else if (battleflag == 1)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Birth by Sleep Final Mix", placeholders, "Battle");
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
                            LargeImageText = "Kingdom Hearts Birth by Sleep Final Mix"
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
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int world_get = Hypervisor.Read<byte>(0x818120);
            int room_get = Hypervisor.Read<byte>(0x818121);
            int difficulty_get = Hypervisor.Read<byte>(0x10FA0881);
            int character_get = Hypervisor.Read<byte>(0x10F9EE4C);
            int level = Hypervisor.Read<byte>(0x10F9EEE1);
            int hp = Hypervisor.Read<ushort>(Hypervisor.GetPointer64(0x10F9EC98, [0x10, 0x4A0]), true);
            int hpmax = Hypervisor.Read<ushort>(Hypervisor.GetPointer64(0x10F9EC98, [0x10, 0x4A2]), true);

            var world = await Worlds.GetWorld(world_get);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);
            var character = await Characters.GetCharacter(character_get);
            var room = await Rooms.GetRoom(world[0]);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "room", room[room_get] },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty },
                { "character", character },
                { "character_icon_name", character.ToLower() }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
