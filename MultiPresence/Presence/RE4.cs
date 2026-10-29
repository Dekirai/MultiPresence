using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.RE4;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class RE4
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static async Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            ProcessMonitor.TryAttach("bio4");
            discord = new DiscordRpcClient("982193093388427314");
            discord.Initialize();
            updater = new DiscordStatusUpdater("Assets/Config/Resident Evil 4 (2005).json");
            PresenceRuntime.Start(nameof(RE4), "bio4", RPC);
            await Task.CompletedTask;
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE4: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("bio4"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("bio4"))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State is not "Title_Screen" and not "Main" and not "Mercenaries" and not "Assignment_Ada" and not "Separate_Ways")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "Waiting for game data");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4 (2005)", placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Resident Evil 4 (2005) memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    ShowStatus("Waiting for game data");
                }
            }
            else
            {
                MemoryReader.Reset();
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
                { "state", snapshot.State },
                { "mode", "" },
                { "gamemode", "" },
                { "character", "" },
                { "characterid", "" },
                { "difficulty", "" },
                { "difficultyid", "" },
                { "health", "" },
                { "maxhealth", "" },
                { "healthstatus", "" },
                { "healthpercentage", "" },
                { "healthstatusemoji", "" },
                { "location", "" },
                { "area", "" },
                { "region", "" },
                { "area_icon_name", "" },
                { "room", "" },
                { "roomid", "" },
                { "stageid", "" },
                { "chapter", "" },
                { "chapterid", "" },
                { "weapon", "" },
                { "currentweapon", "" },
                { "weaponid", "" },
                { "ammo", "" },
                { "loadedammo", "" },
                { "reserveammo", "" },
                { "infiniteammo", "" },
                { "money", "" },
                { "score", "" }
            };

            if (snapshot.State == "Title_Screen")
            {
                placeholders["area"] = "Main Menu";
                return Task.FromResult(placeholders);
            }

            if (snapshot.State is not "Main" and not "Mercenaries" and not "Assignment_Ada" and not "Separate_Ways")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["mode"] = snapshot.Mode;
            placeholders["gamemode"] = snapshot.Mode;
            placeholders["character"] = snapshot.Character;
            placeholders["characterid"] = snapshot.CharacterId < 0 ? "" : snapshot.CharacterId;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["difficultyid"] = snapshot.DifficultyId < 0 ? "" : snapshot.DifficultyId;
            placeholders["health"] = snapshot.Health < 0 ? "" : snapshot.Health;
            placeholders["maxhealth"] = snapshot.MaxHealth <= 0 ? "" : snapshot.MaxHealth;
            placeholders["healthstatus"] = snapshot.HealthStatus;
            placeholders["healthpercentage"] = snapshot.MaxHealth > 0 && snapshot.Health >= 0 ? (double)snapshot.Health / snapshot.MaxHealth * 100 : "";
            placeholders["healthstatusemoji"] = snapshot.HealthStatus switch
            {
                "Fine" => "🟢",
                "Caution" => "🟡",
                "Danger" or "Dead" => "🔴",
                _ => ""
            };
            placeholders["location"] = snapshot.Location;
            placeholders["area"] = snapshot.Area;
            placeholders["region"] = snapshot.Area;
            placeholders["area_icon_name"] = snapshot.AreaIconName;
            placeholders["room"] = snapshot.Room;
            placeholders["roomid"] = snapshot.RoomId < 0 ? "" : snapshot.RoomId;
            placeholders["stageid"] = snapshot.StageId < 0 ? "" : snapshot.StageId;
            placeholders["chapter"] = snapshot.Chapter;
            placeholders["chapterid"] = snapshot.ChapterId < 0 ? "" : snapshot.ChapterId;
            placeholders["weapon"] = snapshot.Weapon;
            placeholders["currentweapon"] = snapshot.Weapon;
            placeholders["weaponid"] = snapshot.WeaponId < 0 ? "" : snapshot.WeaponId;
            placeholders["ammo"] = snapshot.InfiniteAmmo == true ? "∞" : snapshot.Ammo < 0 ? "" : snapshot.Ammo;
            placeholders["loadedammo"] = snapshot.Ammo < 0 ? "" : snapshot.Ammo;
            placeholders["reserveammo"] = snapshot.InfiniteAmmo == true ? "∞" : snapshot.ReserveAmmo < 0 ? "" : snapshot.ReserveAmmo;
            placeholders["infiniteammo"] = snapshot.InfiniteAmmo.HasValue ? snapshot.InfiniteAmmo.Value : "";
            placeholders["money"] = snapshot.Money < 0 ? "" : snapshot.Money;
            placeholders["score"] = snapshot.Score < 0 ? "" : snapshot.Score;
            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE4: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil 4 (2005)"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }
    }
}
