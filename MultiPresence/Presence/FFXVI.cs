#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFXVI
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            discord = new DiscordRpcClient("1285884197084336161");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy XVI.json");
            PresenceRuntime.Start(nameof(FFXVI), "ffxvi", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("ffxvi");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("ffxvi"))
            {
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x018165E8, [0x50]), true);

                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy XVI", placeholders);
                }
                else
                {
                    discord.SetPresence(new RichPresence()
                    {
                        Details = "In Main Menu",
                        State = "",
                        Assets = new Assets()
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Final Fantasy XVI"
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
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x018165E8, [0x50]), true);
            int limitbreak = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x018165E8, [0x58]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x018165E8, [0x40]), true);
            int gil = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x01816668, [0x2C]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x018165E8, [0xCB50]), true);
            string difficulty = "";
            string limitbreak_bars = "";

            if (difficulty_get == 0)
                difficulty = "Easy";
            else if (difficulty_get == 1)
                difficulty = "Normal";
            else if (difficulty_get == 2)
                difficulty = "Final Fantasy";
            else if (difficulty_get == 3)
                difficulty = "Ultimaniac";

            if (limitbreak >= 0 && limitbreak <= 8399)
                limitbreak_bars = "0";
            else if (limitbreak >= 8400 && limitbreak <= 16799)
                limitbreak_bars = "1";
            else if (limitbreak >= 16800 && limitbreak <= 25199)
                limitbreak_bars = "2";
            else if (limitbreak >= 25200 && limitbreak <= 33599)
                limitbreak_bars = "3";
            else if (limitbreak == 33600)
                limitbreak_bars = "4";

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "gil", gil },
                { "difficulty", difficulty },
                { "limitbreak", limitbreak_bars }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}