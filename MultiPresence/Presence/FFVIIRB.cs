#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIRB
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1332349500572045312");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy VII Rebirth.json");
            PresenceRuntime.Start(nameof(FFVIIRB), "ff7rebirth_", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("ff7rebirth_");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("ff7rebirth_"))
            {
                int hp = Hypervisor.Read<int>(0x71C0F30);
                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Rebirth", placeholders);
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
                            LargeImageText = "Final Fantasy VII Rebirth"
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
            int level = Hypervisor.Read<byte>(0x71C0F20);
            int hp = Hypervisor.Read<int>(0x71C0F30);
            int maxhp = Hypervisor.Read<int>(0x71C0F34);
            int mp = Hypervisor.Read<int>(0x71C0F38);
            int maxmp = Hypervisor.Read<int>(0x71C0F3C);
            int chapter = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x939B9E0, [0x20]), true);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "maxhp", maxhp },
                { "mp", mp },
                { "maxmp", maxmp },
                { "chapter", chapter }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}