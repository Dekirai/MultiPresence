using DiscordRPC;
using Steamworks;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SB
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static string[]? levelvalue = null;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1384106213213737002");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Stellar Blade.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("SB-Win64-Shipping")[0];
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
            Process[] game = Process.GetProcessesByName("SB-Win64-Shipping");
            if (game.Length > 0)
            {
                ulong _base = Hypervisor.GetPointer64(0x07030520, [0xC8, 0x28, 0x0, 0x11C]);
                float _maxhealth = Hypervisor.Read<float>(_base + 0x4, true);

                try
                {
                    if (_maxhealth > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Stellar Blade", placeholders);
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
                                LargeImageText = "Stellar Blade"
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
                            LargeImageText = "Stellar Blade"
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
            ulong _base = Hypervisor.GetPointer64(0x07030520, [0xC8, 0x28, 0x0, 0x11C]);

            float _health = Hypervisor.Read<float>(_base, true);
            float _maxhealth = Hypervisor.Read<float>(_base + 0x4, true);
            float _shield = Hypervisor.Read<float>(_base + 0x18, true);
            float _maxshield = Hypervisor.Read<float>(_base + 0x1C, true);
            float _level = Hypervisor.Read<float>(_base + 0x1A0, true);

            return new Dictionary<string, object>
            {
                { "health", _health },
                { "maxhealth", _maxhealth },
                { "shield", _shield },
                { "maxshield", _maxshield },
                { "level", _level }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}