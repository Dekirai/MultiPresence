using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.FFVIIR;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class FFVIIR
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1270065791957471242");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Final Fantasy VII Remake.json");
            PresenceRuntime.Start(nameof(FFVIIR), "ff7remake_", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("ff7remake_");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("FFVIIR: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("ff7remake_"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("ff7remake_"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy VII Remake", placeholders,
                        snapshot.Campaign == "INTERmission" ? "INTERmission" : "Default");
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Final Fantasy VII Remake memory is not ready.", exception);
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
                { "campaign", "" },
                { "character", "" },
                { "characterid", "" },
                { "characterkey", "" },
                { "level", "" },
                { "hp", "" },
                { "maxhp", "" },
                { "mp", "" },
                { "maxmp", "" },
                { "sp", "" },
                { "maxsp", "" },
                { "wp", "" },
                { "maxwp", "" },
                { "weaponlevel", "" },
                { "exp", "" },
                { "maxexp", "" },
                { "ep", "" },
                { "maxep", "" },
                { "totalexp", "" },
                { "nextexp", "" },
                { "chapter", "" },
                { "chaptername", "" },
                { "chapterid", "" },
                { "location", "" },
                { "region", "" },
                { "map", "" },
                { "area", "" },
                { "locationid", "" },
                { "mapid", "" },
                { "layerid", "" },
                { "groupid", "" },
                { "difficulty", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["campaign"] = snapshot.Campaign;
            placeholders["character"] = snapshot.Character;
            placeholders["characterid"] = snapshot.CharacterId;
            placeholders["characterkey"] = snapshot.CharacterKey;
            placeholders["level"] = snapshot.Level;
            placeholders["hp"] = snapshot.Hp;
            placeholders["maxhp"] = snapshot.MaxHp;
            placeholders["mp"] = snapshot.Mp;
            placeholders["maxmp"] = snapshot.MaxMp;
            placeholders["sp"] = snapshot.Sp;
            placeholders["maxsp"] = snapshot.MaxSp.HasValue ? snapshot.MaxSp.Value : "Unknown";
            placeholders["wp"] = snapshot.Sp;
            placeholders["maxwp"] = placeholders["maxsp"];
            placeholders["weaponlevel"] = snapshot.WeaponLevel;
            placeholders["exp"] = snapshot.Exp.HasValue ? snapshot.Exp.Value : "Unknown";
            placeholders["maxexp"] = snapshot.MaxExp.HasValue ? snapshot.MaxExp.Value : "Unknown";
            placeholders["ep"] = placeholders["exp"];
            placeholders["maxep"] = placeholders["maxexp"];
            placeholders["totalexp"] = snapshot.TotalExp;
            placeholders["nextexp"] = snapshot.NextExp.HasValue ? snapshot.NextExp.Value : "Unknown";
            placeholders["chapter"] = snapshot.Chapter > 0 ? snapshot.Chapter : "Unknown";
            placeholders["chaptername"] = snapshot.ChapterName;
            placeholders["chapterid"] = snapshot.ChapterId;
            placeholders["location"] = snapshot.Location;
            placeholders["region"] = snapshot.Region;
            placeholders["map"] = snapshot.Map;
            placeholders["area"] = snapshot.Area;
            placeholders["locationid"] = snapshot.LocationId;
            placeholders["mapid"] = snapshot.MapId;
            placeholders["layerid"] = snapshot.LayerId;
            placeholders["groupid"] = snapshot.GroupId;
            placeholders["difficulty"] = snapshot.Difficulty;
            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFVIIR: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Final Fantasy VII Remake"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFVIIR: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
