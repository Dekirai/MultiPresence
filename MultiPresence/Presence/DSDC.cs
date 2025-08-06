using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class DSDC
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1402720720584314962");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Death Stranding.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ds")[0];
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
            Process[] game = Process.GetProcessesByName("ds");
            if (game.Length > 0)
            {

                float maxhealth = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x07BC7568, [0x60, 0x48, 0x20, 0x0, 0x78, 0x70, 0xC4]), true);

                try
                {
                    if (maxhealth > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Death Stranding", placeholders);
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
                                LargeImageText = "Death Stranding"
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
                            LargeImageText = "Death Stranding"
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
            ulong _base = Hypervisor.GetPointer64(0x07BC7568, [0x60, 0x48, 0x20, 0x0, 0x78, 0x70, 0xC0]);
            float health = Hypervisor.Read<float>(_base, true);
            float maxhealth = Hypervisor.Read<float>(_base + 0x04, true);
            float stamina = Hypervisor.Read<float>(_base + 0x80, true);
            float maxstamina = Hypervisor.Read<float>(_base + 0x50, true);

            return new Dictionary<string, object>
            {
                { "maxhealth", maxhealth },
                { "health", health },
                { "maxstamina", maxstamina },
                { "stamina", stamina }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}
