#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KHBBS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHBBS
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("839545395368820806");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts Birth by Sleep.json");
            PresenceRuntime.Start(nameof(KHBBS), "KINGDOM HEARTS Birth by Sleep FINAL MIX", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("KINGDOM HEARTS Birth by Sleep FINAL MIX"))
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
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
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
