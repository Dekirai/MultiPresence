#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class YSI
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static string[] levelvalue = null;

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1435551695315927111");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Ys I Chronicles.json");
            PresenceRuntime.Start(nameof(YSI), "ys1plus", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("ys1plus");
            }
            catch
            {
                // nothing
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("ys1plus"))
            {
                try
                {
                    int menuflag = Hypervisor.Read<byte>(0x131664);
                    if (menuflag == 0x01)
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Ys I Chronicles"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Ys I Chronicles", placeholders);
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
                            LargeImageText = "Ys I Chronicles"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }
            }
            else
            {
                discord?.Deinitialize();
                updater?.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int hp = Hypervisor.Read<int>(0x1317FC);
            int max_hp = Hypervisor.Read<int>(0x131800);
            int gold = Hypervisor.Read<int>(0x13180C);
            int str = Hypervisor.Read<int>(0x131694);
            int def = Hypervisor.Read<int>(0x131698);
            int exp = Hypervisor.Read<int>(0x131810);
            int level = Hypervisor.Read<byte>(0x131814);

            string expDisplay;
            if (level >= 10)
            {
                expDisplay = $"Level {level} (MAX)";
            }
            else if (level > 0 && level < 10)
            {
                int expStart = YS1_CUMULATIVE_TABLE.ContainsKey(level) ? YS1_CUMULATIVE_TABLE[level] : 0;
                int expChunk = YS1_LEVEL_CHUNK_TABLE.ContainsKey(level) ? YS1_LEVEL_CHUNK_TABLE[level] : 0;
                int expEarned = exp - expStart;
                expDisplay = $"Level {level} (EXP: {expEarned}/{expChunk})";
            }
            else
            {
                expDisplay = $"Level {level} (EXP: ---)";
            }

            return new Dictionary<string, object>
            {
                { "hp", hp },
                { "max_hp", max_hp },
                { "gold", gold },
                { "str", str },
                { "def", def },
                { "exp", expDisplay }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }

        private static readonly Dictionary<int, int> YS1_LEVEL_CHUNK_TABLE = new Dictionary<int, int>
        {
            { 1, 200 },
            { 2, 200 },
            { 3, 400 },
            { 4, 800 },
            { 5, 1600 },
            { 6, 3200 },
            { 7, 6400 },
            { 8, 12800 },
            { 9, 25600 },
            { 10, 0 }
        };

        private static readonly Dictionary<int, int> YS1_CUMULATIVE_TABLE = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 200 },
            { 3, 400 },
            { 4, 800 },
            { 5, 1600 },
            { 6, 3200 },
            { 7, 6400 },
            { 8, 12800 },
            { 9, 25600 },
            { 10, 51200 }
        };
    }
}
