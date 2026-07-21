#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.KHDDD;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHDDD
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("906904369151213629");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Kingdom Hearts Dream Drop Distance.json");
            PresenceRuntime.Start(nameof(KHDDD), "KINGDOM HEARTS Dream Drop Distance", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("KINGDOM HEARTS Dream Drop Distance");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("KINGDOM HEARTS Dream Drop Distance"))
            {
                int world_get = Hypervisor.Read<byte>(0xA40764);
                int room_get = Hypervisor.Read<byte>(0xA40765);

                var world = await Worlds.GetWorld(world_get);
                var room = await Rooms.GetRoom(world[0]);

                try
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Dream Drop Distance", placeholders);
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
                            LargeImageText = "Kingdom Hearts Dream Drop Distance"
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
            int world_get = Hypervisor.Read<byte>(0xA40764);
            int room_get = Hypervisor.Read<byte>(0xA40765);
            int difficulty_get = Hypervisor.Read<byte>(0xA40762);
            int character_get = Hypervisor.Read<byte>(0xA40760);
            int level = Hypervisor.Read<byte>(0xA98034);

            var world = await Worlds.GetWorld(world_get);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);
            var room = await Rooms.GetRoom(world[0]);
            string character = "";

            if (character_get == 0)
                character = "Sora";
            else
                character = "Riku";

            return new Dictionary<string, object>
            {
                { "level", level },
                { "room", room[room_get] },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty },
                { "character", character },
                { "character_icon_name", character_get }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
