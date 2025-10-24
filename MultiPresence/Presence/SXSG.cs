using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class SXSG
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1430902391665987605");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Sonic Generations.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("SONIC_GENERATIONS")[0];
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
            Process[] game = Process.GetProcessesByName("SONIC_GENERATIONS");
            if (game.Length > 0)
            {
                float _time = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01F52428, [0x10, 0x258]), true);

                try
                {
                    if (_time > 0)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Sonic Generations", placeholders);
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
                                SmallImageKey = "sonic",
                                SmallImageText = "Playing Sonic Generations",
                                LargeImageText = "SONIC X SHADOW GENERATIONS"
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
                            SmallImageKey = "sonic",
                            SmallImageText = "Playing Sonic Generations",
                            LargeImageText = "SONIC X SHADOW GENERATIONS"
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
            string _stageget = Hypervisor.ReadString(Hypervisor.GetPointer64(0x01B2D610, [0x0, 0x0, 0x0, 0x21B]), 4, true);
            int _rings = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x1E87518, [0x648]), true);
            float _time = Hypervisor.Read<float>(Hypervisor.GetPointer64(0x01F52428, [0x10, 0x258]), true);

            TimeSpan time = TimeSpan.FromSeconds(_time);

            // Format as mm:ss.ms
            string formatted = string.Format("{0:D2}:{1:D2}.{2:D3}",
                time.Minutes,
                time.Seconds,
                time.Milliseconds);

            string _stage = _stageget switch
            {
                "pam0" => "Hub",
                "fig0" => "Statue Room",
                "ghz1" => "Green Hill (Classic)",
                "cpz1" => "Chemical Plant (Classic)",
                "ssz1" => "Sky Sanctuary (Classic)",
                "sph1" => "Speed Highway (Classic)",
                "cte1" => "City Escape (Classic)",
                "ssh1" => "Seaside Hill (Classic)",
                "csc1" => "Crisis City (Classic)",
                "euc1" => "Rooftop Run (Classic)",
                "pla1" => "Planet Wisp (Classic)",
                "ghz2" => "Green Hill (Modern)",
                "cpz2" => "Chemical Plant (Modern)",
                "ssz2" => "Sky Sanctuary (Modern)",
                "sph2" => "Speed Highway (Modern)",
                "cte2" => "City Escape (Modern)",
                "ssh2" => "Seaside Hill (Modern)",
                "csc2" => "Crisis City (Modern)",
                "euc2" => "Rooftop Run (Modern)",
                "pla2" => "Planet Wisp (Modern)",
                "bms\\" => "Boss: Metal Sonic",
                "bms0" => "Boss: Metal Sonic (Hard)",
                "bde\\" => "Boss: Death Egg Robot",
                "bde0" => "Boss: Death Egg Robot (Hard)",
                "bsd\\" => "Boss: Shadow",
                "bsd0" => "Boss: Shadow (Hard)",
                "bpc\\" => "Boss: Perfect Chaos",
                "bpc0" => "Boss: Perfect Chaos (Hard)",
                "bsl\\" => "Boss: Silver",
                "bsl0" => "Boss: Silver (Hard)",
                "bne\\" => "Boss: Egg Dragoon",
                "bne0" => "Boss: Egg Dragoon (Hard)",
                "blb\\" => "Boss: Time Eater",
                "blb0" => "Boss: Time Eater (Hard)",
                _ => "Unknown"
            };


            return new Dictionary<string, object>
            {
                { "rings", _rings },
                { "time", formatted },
                { "stage", _stage }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}