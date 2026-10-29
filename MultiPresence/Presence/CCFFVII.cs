using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.CCFFVII;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class CCFFVII
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1271522314248523837");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/CRISIS CORE –FINAL FANTASY VII– REUNION.json");
            PresenceRuntime.Start(nameof(CCFFVII), "CCFF7R-Win64-Shipping", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("CCFF7R-Win64-Shipping");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("CCFFVII: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("CCFF7R-Win64-Shipping"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("CCFF7R-Win64-Shipping"))
                    {
                        discord.ClearPresence();
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State != "Ingame")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "In Main Menu");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "CRISIS CORE –FINAL FANTASY VII– REUNION", placeholders,
                        snapshot.Mode == "Mission" ? "Mission" : "Default");
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("CRISIS CORE –FINAL FANTASY VII– REUNION memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    discord.ClearPresence();
                }
            }
            else
            {
                MemoryReader.Reset();
                discord.ClearPresence();
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static GameSnapshot ReadSnapshot()
        {
            return MemoryReader.ReadSnapshot();
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(GameSnapshot snapshot)
        {
            var placeholders = new Dictionary<string, object>
            {
                { "state", snapshot.State == "Ingame" ? "In Game" : snapshot.State == "Loading" ? "Loading" : "Main Menu" },
                { "mode", "" },
                { "character", "" },
                { "level", "" },
                { "gil", "" },
                { "sp", "" },
                { "difficulty", "" },
                { "hp", "" },
                { "maxhp", "" },
                { "mp", "" },
                { "maxmp", "" },
                { "ap", "" },
                { "maxap", "" },
                { "hp_mission", "" },
                { "maxhp_mission", "" },
                { "mp_mission", "" },
                { "maxmp_mission", "" },
                { "ap_mission", "" },
                { "maxap_mission", "" },
                { "chapter", "" },
                { "chaptername", "" },
                { "location", "" },
                { "region", "" },
                { "area", "" },
                { "mapid", "" },
                { "missionid", "" },
                { "missionname", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["mode"] = snapshot.Mode;
            placeholders["character"] = snapshot.Character;
            placeholders["level"] = snapshot.Level;
            placeholders["gil"] = snapshot.Gil;
            placeholders["sp"] = snapshot.Sp;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["hp"] = snapshot.Hp;
            placeholders["maxhp"] = snapshot.MaxHp;
            placeholders["mp"] = snapshot.Mp;
            placeholders["maxmp"] = snapshot.MaxMp;
            placeholders["ap"] = snapshot.Ap;
            placeholders["maxap"] = snapshot.MaxAp;
            placeholders["chapter"] = snapshot.Chapter is 0 or 11 && snapshot.ChapterName.Length > 0
                ? snapshot.ChapterName : snapshot.Chapter > 0 ? snapshot.Chapter : "Unknown";
            placeholders["chaptername"] = snapshot.ChapterName;
            placeholders["location"] = snapshot.Location;
            placeholders["region"] = snapshot.Region;
            placeholders["area"] = snapshot.Area;
            placeholders["mapid"] = snapshot.MapId;

            if (snapshot.Mode == "Mission")
            {
                placeholders["hp_mission"] = snapshot.Hp;
                placeholders["maxhp_mission"] = snapshot.MaxHp;
                placeholders["mp_mission"] = snapshot.Mp;
                placeholders["maxmp_mission"] = snapshot.MaxMp;
                placeholders["ap_mission"] = snapshot.Ap;
                placeholders["maxap_mission"] = snapshot.MaxAp;
                placeholders["missionid"] = snapshot.MissionId;
                placeholders["missionname"] = snapshot.MissionName;
            }

            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("CCFFVII: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "CRISIS CORE –FINAL FANTASY VII– REUNION"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("CCFFVII: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
