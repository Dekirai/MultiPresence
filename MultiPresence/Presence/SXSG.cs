using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.SXSG;

namespace MultiPresence.Presence
{
    public class SXSG
    {
        private const string SonicProcessName = "SONIC_GENERATIONS";
        private const string ShadowProcessName = "SONIC_X_SHADOW_GENERATIONS";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static bool shadow;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            StartPresence(false);
        }

        public static void DoShadowAction()
        {
            StartPresence(true);
        }

        private static void StartPresence(bool isShadow)
        {
            shadow = isShadow;
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient(shadow ? "1564332536660889841" : "1430902391665987605");
            InitializeDiscord();
            updater = new DiscordStatusUpdater(shadow
                ? "Assets/Config/Shadow Generations.json"
                : "Assets/Config/Sonic Generations.json");
            PresenceRuntime.Start(nameof(SXSG), shadow ? ShadowProcessName : SonicProcessName, RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(shadow ? ShadowProcessName : SonicProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("SXSG: presence is not initialized.");
            }

            if (shadow)
            {
                await ShadowRPC();
                return;
            }

            if (ProcessMonitor.IsRunning(SonicProcessName))
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
            }
            else
            {
                StopPresence();
            }
        }

        private static async Task ShadowRPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("SXSG: presence is not initialized.");
            }

            if (!ProcessMonitor.IsRunning(ShadowProcessName) || ProcessMonitor.IsRunning(SonicProcessName))
            {
                StopPresence();
                return;
            }

            try
            {
                if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(ShadowProcessName))
                {
                    discord.ClearPresence();
                    return;
                }

                var snapshot = MemoryReader.ReadSnapshot();
                if (snapshot.State == "OtherGame")
                {
                    StopPresence();
                    return;
                }

                if (snapshot.State == "Loading")
                {
                    ShowShadowStatus("Loading");
                    return;
                }

                var placeholders = await PlaceholderHelper.GetPlaceholders(() => GenerateShadowPlaceholders(snapshot));
                string state = snapshot.State == "Ingame" && snapshot.Mode == "White Space" ? "WhiteSpace" : snapshot.State;
                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Shadow Generations", placeholders, state);
            }
            catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
            {
                if (DateTime.UtcNow >= nextWarning)
                {
                    AppLog.Warning("Shadow Generations memory is not ready.", exception);
                    nextWarning = DateTime.UtcNow.AddSeconds(30);
                }

                discord.ClearPresence();
            }
        }

        private static Task<Dictionary<string, object>> GenerateShadowPlaceholders(GameSnapshot snapshot)
        {
            var placeholders = new Dictionary<string, object>
            {
                { "character", "" },
                { "stage", "" },
                { "stageid", "" },
                { "mode", "" },
                { "rings", "" },
                { "time", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            int time = (int)Math.Round(snapshot.Time * 100, MidpointRounding.AwayFromZero);
            string formatted = string.Format("{0:D2}:{1:D2}.{2:D2}",
                time / 6000,
                time / 100 % 60,
                time % 100);

            placeholders["character"] = snapshot.Character;
            placeholders["stage"] = snapshot.Stage;
            placeholders["stageid"] = snapshot.StageId;
            placeholders["mode"] = snapshot.Mode;
            placeholders["rings"] = snapshot.Rings;
            placeholders["time"] = formatted;

            return Task.FromResult(placeholders);
        }

        private static void ShowShadowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("SXSG: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Shadow Generations"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void StopPresence()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("SXSG: presence is not initialized.");
            }

            MemoryReader.Reset();
            discord.ClearPresence();
            discord.Deinitialize();
            updater.Dispose();
            PresenceRuntime.RequestDetection();
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
            if (discord == null)
            {
                throw new InvalidOperationException("SXSG: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
