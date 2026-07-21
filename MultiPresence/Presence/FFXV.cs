#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFXV
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async Task DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1286960651738288160");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy XV.json");
            PresenceRuntime.Start(nameof(FFXV), "ffxv_s", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("ffxv_s");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("ffxv_s"))
            {
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x154]), true);

                if (maxhp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy XV", placeholders);
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
                            LargeImageText = "Final Fantasy XV"
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
            string character = Hypervisor.ReadString(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x80]), 20, true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x14C]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x154]), true);
            int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x1A0]), true);
            int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x1A4]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x45DAB28, [0x10, 0x8, 0x8, 0x630, 0x10, 0x50, 0x1B8]), true);

            string story = ""; //???

            if (character == "Noctis")
                story = "Main Story";
            else if (character == "Gladiolus")
                story = "Episode Gladiolus";
            else if (character == "Prompto")
                story = "Episode Prompto";
            else if (character == "Ignis")
                story = "Episode Ignis";
            else if (character == "Ardyn")
                story = "Episode Ardyn";
            else if (character.Length > 0)
                story = "Multiplayer: Comrades";
            else story = "Main Story";


            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "maxhp", maxhp },
                { "mp", mp },
                { "maxmp", maxmp },
                { "character", character },
                { "story", story }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}