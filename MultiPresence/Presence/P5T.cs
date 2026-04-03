using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class P5T
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1482730815220617226");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Persona 5 Tactica.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("Persona 5 Tactica")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("Persona 5 Tactica");
            if (game.Length > 0)
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

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                MainForm.gameUpdater.Start();
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