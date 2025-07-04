using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class P5S
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1389246400126255136");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Persona 5 Strikers.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("game")[0];
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
            Process[] game = Process.GetProcessesByName("game");
            if (game.Length > 0)
            {
                int inbattle = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x02498458, [0x960, 0x0, 0x0, 0x20, 0x3B8, 0x410, 0xBC]), true);

                float hp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x02380C60, [0x4E0, 0xAA4]), true);

                if (hp > 0)
                {
                    if (inbattle == 1)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersBattle);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 5 Strikers", placeholders, "Battle");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Persona 5 Strikers", placeholders);
                    }
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
                            LargeImageText = "Persona 5 Strikers"
                        },
                        Timestamps = PlaceholderHelper._startTimestamp
                    });
                }

                    await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {

            ulong _CSaveDataManager = Hypervisor.GetPointer64(0x142357FE0, [0x0], true);
            ulong _Inventory = _CSaveDataManager + 0x5E4980;
            ulong _Characters = _CSaveDataManager + 0x5DD82C;

            int money = Hypervisor.Read<int>(_Inventory + 0x15BC, true);
            int level = Hypervisor.Read<byte>(_Characters, true);

            float hp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x02380C60, [0x4E0, 0xAA4]), true);
            float sp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x02380C60, [0x4E0, 0xC58]), true);

            int hp_rounded = (int)Math.Round(hp, 0, MidpointRounding.AwayFromZero);
            int sp_rounded = (int)Math.Round(sp, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "money", money },
                { "level", level },
                { "hp", hp_rounded },
                { "sp", sp_rounded }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersBattle()
        {
            ulong _CSaveDataManager = Hypervisor.GetPointer64(0x142357FE0, [0x0], true);
            ulong _Inventory = _CSaveDataManager + 0x5E4980;
            ulong _Characters = _CSaveDataManager + 0x5DD82C;

            int money = Hypervisor.Read<int>(_Inventory + 0x15BC, true);
            int level = Hypervisor.Read<byte>(_Characters, true);

            float hp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x02380C60, [0x4E0, 0xAA4]), true);
            float sp = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x02380C60, [0x4E0, 0xC58]), true);

            int hp_rounded = (int)Math.Round(hp, 0, MidpointRounding.AwayFromZero);
            int sp_rounded = (int)Math.Round(sp, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "money", money },
                { "level", level },
                { "hp", hp_rounded },
                { "sp", sp_rounded }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}