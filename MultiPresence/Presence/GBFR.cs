using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class GBFR
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1426303485208559616");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Granblue Fantasy Relink.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("granblue_fantasy_relink")[0];
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
            Process[] game = Process.GetProcessesByName("granblue_fantasy_relink");
            if (game.Length > 0)
            {
                int _maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E59900, [0x164]), true);

                try
                {
                    if (_maxhealth > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Granblue Fantasy: Relink", placeholders);
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
                                LargeImageText = "Granblue Fantasy: Relink"
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
                            LargeImageText = "Granblue Fantasy: Relink"
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
            int _health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E59900, [0x160]), true);
            int _maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E59900, [0x164]), true);
            int _level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05FCA4F0, [0x140, 0x38, 0x3C]), true);
            int _money = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x05E48228, [0x30]), true);
            string _name = Hypervisor.ReadString(Hypervisor.GetPointer64(0x05E48228, [0x2B0]), 16, true);

            return new Dictionary<string, object>
            {
                { "health", _health },
                { "maxhealth", _maxhealth },
                { "level", _level },
                { "money", _money },
                { "name", _name }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}