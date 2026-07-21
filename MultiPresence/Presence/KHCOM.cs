#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KHCOM;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHCOM
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1342148362471866460");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts ReChain of Memories.json");
            PresenceRuntime.Start(nameof(KHCOM), "KINGDOM HEARTS Re_Chain of Memories", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("KINGDOM HEARTS Re_Chain of Memories");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("KINGDOM HEARTS Re_Chain of Memories"))
            {
                int world_get = Hypervisor.Read<byte>(0x87B862);
                int battleflag = Hypervisor.Read<byte>(0x87B858);

                try
                {
                    if (world_get == 255)
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Kingdom Hearts Re:Chain of Memories"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
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
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Kingdom Hearts Re:Chain of Memories"
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
            int world_get = Hypervisor.Read<byte>(0x87B862);
            int character_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x87B0E8, [0x8, 0x60]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x87B0E8, [0x8, 0x61]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x87B380, [0x444]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x87B380, [0x42C]), true);
            int hpmax = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x87B380, [0x430]), true);

            var world = await Worlds.GetWorld(world_get);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);

            string character = character_get switch
            {
                0 => "Sora",
                1 => "Riku",
                _ => "Unknown"
            };

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "character", character },
                { "difficulty", difficulty },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "character_icon_name", character.ToLower() }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
