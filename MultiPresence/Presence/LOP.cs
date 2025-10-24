using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class LOP
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1344746533446483998");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Lies of P.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("LOP-Win64-Shipping")[0];
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
            Process[] game = Process.GetProcessesByName("LOP-Win64-Shipping");
            if (game.Length > 0)
            {
                int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xD2C]), true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Lies of P", placeholders);
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
                                LargeImageText = "Lies of P"
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
                            LargeImageText = "Lies of P"
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
                updater.Dispose();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int clearcount = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06DD87E8, [0xA0, 0x7C]), true);
            int deaths = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06DD87E8, [0xA0, 0x98]), true);
            int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06DD87E8, [0xA0, 0xA0]), true);
            int ergo = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06DD87E8, [0xA0, 0xA4]), true);
            int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xC]), true);
            int maxhp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xD2C]), true);
            int fable = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0x6C]), true);
            int maxfable = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xD8C]), true);
            int legion = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xCC]), true);
            int maxlegion = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xDEC]), true);
            int stamina = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0x3C]), true);
            int maxstamina = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x06F367B8, [0x0, 0x20, 0x160, 0x40, 0xE0, 0x28, 0xD5C]), true);

            int GetFableBars(int value, int barSize = 4000)
            {
                return value / barSize;
            }

            int fableBars = GetFableBars(fable);
            int maxFableBars = GetFableBars(maxfable);

            return new Dictionary<string, object>
            {
                { "clearcount", clearcount },
                { "deaths", deaths },
                { "level", level },
                { "ergo", ergo },
                { "hp", hp },
                { "maxhp", maxhp },
                { "fable", fableBars },
                { "maxfable", maxFableBars },
                { "legion", legion },
                { "maxlegion", maxlegion },
                { "stamina", stamina },
                { "maxstamina", maxstamina }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
