#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class P5X
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        private static string uid;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1390679033074942154");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Persona 5 The Phantom X.json");
            PresenceRuntime.Start(nameof(P5X), "p5x", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("p5x");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("p5x"))
            {
                try
                {
                    ulong _finduid;
                    _finduid = (ulong)Hypervisor.FindSignature("72 6F 6C 65 49 64 00 11 00 00 00 0B");

                    uid = Hypervisor.ReadString(_finduid + 0xC, 12, true);

                    if (uid.Length > 10)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 5: The Phantom X", placeholders);
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
                                LargeImageText = "Persona 5: The Phantom X"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
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
                            LargeImageText = "Persona 5: The Phantom X"
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
            return new Dictionary<string, object>
            {
                { "uid", uid }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}