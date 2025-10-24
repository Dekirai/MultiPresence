using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class CV
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1394051549348630548");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Code Vein.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("CodeVein-Win64-Shipping")[0];
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
            Process[] game = Process.GetProcessesByName("CodeVein-Win64-Shipping");
            if (game.Length > 0)
            {
                ulong _base = Hypervisor.GetPointer64(0x043697C0, [0x30, 0x398, 0x358, 0xA8, 0x0]);
                float maxhp = Hypervisor.Read<float>(_base + 0x208, true);

                try
                {
                    if (maxhp > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "CODE VEIN", placeholders);
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
                                LargeImageText = "CODE VEIN"
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
                            LargeImageText = "CODE VEIN"
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
            ulong _base = Hypervisor.GetPointer64(0x043697C0, [0x30, 0x398, 0x358, 0xA8, 0x0]);

            int level = Hypervisor.Read<int>(_base + 0x1E8, true);
            int haze = Hypervisor.Read<int>(_base + 0x548, true);
            float hp = Hypervisor.Read<float>(_base + 0x1FC, true);
            float maxhp = Hypervisor.Read<float>(_base + 0x208, true);
            float stamina = Hypervisor.Read<float>(_base + 0x264, true);
            float maxstamina = Hypervisor.Read<float>(_base + 0x26C, true);
            float inchor = Hypervisor.Read<float>(_base + 0x500, true);
            float maxinchor = Hypervisor.Read<float>(_base + 0x508, true);

            int hp_rounded = (int)Math.Round(hp, 0, MidpointRounding.AwayFromZero);
            int maxhp_rounded = (int)Math.Round(maxhp, 0, MidpointRounding.AwayFromZero);
            int stamina_rounded = (int)Math.Round(stamina, 0, MidpointRounding.AwayFromZero);
            int maxstamina_rounded = (int)Math.Round(maxstamina, 0, MidpointRounding.AwayFromZero);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "haze", haze },
                { "hp", hp_rounded },
                { "maxhp", maxhp_rounded },
                { "stamina", stamina_rounded },
                { "maxstamina", maxstamina_rounded },
                { "inchor", inchor },
                { "maxinchor", maxinchor }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
