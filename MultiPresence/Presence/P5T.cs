#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class P5T
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1482730815220617226");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Persona 5 Tactica.json");
            PresenceRuntime.Start(nameof(P5T), "Persona 5 Tactica", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("Persona 5 Tactica");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("Persona 5 Tactica"))
            {
                int currentturn = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0495C0F0, [0xB8, 0x28, 0xA38, 0x10, 0x20, 0x50], false, "GameAssembly.dll"), true);

                if (currentturn >= 1 && currentturn <= 255)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBattle);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 5 Tactica", placeholders, "Battle");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 5 Tactica", placeholders);
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
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x049708A8, [0xB8, 0x0, 0x18, 0x40], false, "GameAssembly.dll"), true);
            int phantomthief_lv = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x049708A8, [0xB8, 0x0, 0x18, 0x34], false, "GameAssembly.dll"), true);
            int phantomthief_xp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x049708A8, [0xB8, 0x0, 0x18, 0x38], false, "GameAssembly.dll"), true);

            return new Dictionary<string, object>
            {
                { "money", money },
                { "level", phantomthief_lv },
                { "xp", phantomthief_xp }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBattle()
        {
            int money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x049708A8, [0xB8, 0x0, 0x18, 0x40], false, "GameAssembly.dll"), true);
            int phantomthief_lv = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x049708A8, [0xB8, 0x0, 0x18, 0x34], false, "GameAssembly.dll"), true);
            int phantomthief_xp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x049708A8, [0xB8, 0x0, 0x18, 0x38], false, "GameAssembly.dll"), true);
            int currentturn = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0495C0F0, [0xB8, 0x28, 0xA38, 0x10, 0x20, 0x50], false, "GameAssembly.dll"), true);

            return new Dictionary<string, object>
            {
                { "currentturn", currentturn },
                { "money", money },
                { "level", phantomthief_lv },
                { "xp", phantomthief_xp }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}