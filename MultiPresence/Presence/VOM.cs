#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.VOM;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class VOM
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static string difficulty = "";

        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1279828464613982333");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Visions of Mana.json");
            PresenceRuntime.Start(nameof(VOM), "VisionsofMana-Win64-Shipping", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("VisionsofMana-Win64-Shipping");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("VisionsofMana-Win64-Shipping"))
            {
                float hp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x48]), true);
                float hpmax = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x68]), true);
                float mp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x88]), true);
                float mpmax = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x98]), true);
                int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x06232B70, [0x3A0, 0x780, 0x80, 0xF0, 0x140, 0x88, 0xC30]), true);
                float level = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x38]), true);

                try
                {
                    difficulty = await Difficulties.GetDifficulty(difficulty_get);

                    if (level == 0)
                    {
                        discord.SetPresence(new RichPresence()
                        {
                            Details = "In Main Menu",
                            State = "",
                            Assets = new Assets()
                            {
                                LargeImageKey = "logo",
                                LargeImageText = "Visions of Mana"
                            },
                            Timestamps = PlaceholderHelper._startTimestamp
                        });
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Visions of Mana", placeholders);
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
                            LargeImageText = "Visions of Mana"
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
            float hp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x48]), true);
            float hpmax = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x68]), true);
            float mp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x88]), true);
            float mpmax = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x98]), true);
            int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x06232B70, [0x3A0, 0x780, 0x80, 0xF0, 0x140, 0x88, 0xC30]), true);
            float level = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x067F3510, [0x30, 0x228, 0x338, 0x9A8, 0x140, 0x0, 0x38]), true);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "hp", hp },
                { "hpmax", hpmax },
                { "mp", mp },
                { "mpmax", mp },
                { "difficulty", difficulty }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
