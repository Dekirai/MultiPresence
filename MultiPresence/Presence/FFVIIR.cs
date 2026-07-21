#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFVIIR
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1270065791957471242");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Final Fantasy VII Remake.json");
            PresenceRuntime.Start(nameof(FFVIIR), "ff7remake_", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("ff7remake_");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("ff7remake_"))
            {
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B0]), true);

                if (hp > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Remake", placeholders);
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
                            LargeImageText = "Final Fantasy VII Remake"
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
            int level = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x057CA5E8, [0x8A0]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B0]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B4]), true);
            int mp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8B8]), true);
            int maxmp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x057CA5E8, [0x8BC]), true);
            int chapter = Hypervisor.Read<byte>(0x59ADBF0);

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