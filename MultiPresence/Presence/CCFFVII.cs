#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.CCFFVII;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class CCFFVII
    {

        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1271522314248523837");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/CRISIS CORE –FINAL FANTASY VII– REUNION.json");
            PresenceRuntime.Start(nameof(CCFFVII), "CCFF7R-Win64-Shipping", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("CCFF7R-Win64-Shipping");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("CCFF7R-Win64-Shipping"))
            {
                int hp_mission = Hypervisor.Read<int>(0x718DC28);

                if (hp_mission > 0)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders, "Mission");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders);
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
            int level = Hypervisor.Read<byte>(0x71B3F36);
            int gil = Hypervisor.Read<int>(0x71B3FB0);
            int difficulty_get = Hypervisor.Read<byte>(0x71B3FD7);

            var difficulty = await Difficulties.GetDifficulty(difficulty_get);

            int hp = Hypervisor.Read<int>(0x71B3F10);
            int maxhp = Hypervisor.Read<int>(0x71B3F14);
            int mp = Hypervisor.Read<int>(0x71B3F1C);
            int maxmp = Hypervisor.Read<int>(0x71B3F20);
            int ap = Hypervisor.Read<int>(0x71B3F28);
            int maxap = Hypervisor.Read<int>(0x71B3F2C);

            int hp_mission = Hypervisor.Read<int>(0x718DC28);
            int maxhp_mission = Hypervisor.Read<int>(0x718DC2C);
            int mp_mission = Hypervisor.Read<int>(0x718DC30);
            int maxmp_mission = Hypervisor.Read<int>(0x718DC34);
            int ap_mission = Hypervisor.Read<int>(0x718DC38);
            int maxap_mission = Hypervisor.Read<int>(0x718DC3C);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "gil", gil },
                { "difficulty", difficulty },
                { "hp", hp },
                { "maxhp", maxhp },
                { "mp", mp },
                { "maxmp", maxmp },
                { "ap", ap },
                { "maxap", maxap },
                { "hp_mission", hp_mission },
                { "maxhp_mission", maxhp_mission },
                { "mp_mission", mp_mission },
                { "maxmp_mission", maxmp_mission },
                { "ap_mission", ap_mission },
                { "maxap_mission", maxap_mission }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}