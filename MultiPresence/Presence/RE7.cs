using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.RE7;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class RE7
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1401528728567418993");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Resident Evil 7.json");
            PresenceRuntime.Start(nameof(RE7), "re7", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re7");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE7: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re7"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re7"))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State is not "Main Story" and not "DLC Story" and not "DLC Minigame")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : snapshot.State == "Main Menu" ? "In Main Menu" : "Waiting for game data");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 7", placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Resident Evil 7 memory is not ready.", exception);
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
                { "room", "" },
                { "roomid", "" },
                { "mapid", "" },
                { "objective", "" },
                { "primaryobjective", "" },
                { "secondaryobjective", "" },
                { "objectivepath", "" },
                { "objectives", "" },
                { "objectiveid", "" },
                { "primaryobjectiveid", "" },
                { "secondaryobjectiveid", "" },
                { "weapon", "" },
                { "currentweapon", "" },
                { "weaponid", "" },
                { "ammo", "" },
                { "loadedammo", "" },
                { "reserveammo", "" },
                { "infiniteammo", "" }
            };

            if (snapshot.State is not "Main Story" and not "DLC Story" and not "DLC Minigame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["mode"] = snapshot.Mode;
            placeholders["gamemode"] = snapshot.Mode;
            placeholders["character"] = snapshot.Character;
            placeholders["characterid"] = snapshot.CharacterId;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["difficultyid"] = snapshot.DifficultyId < 0 ? "" : snapshot.DifficultyId;
            placeholders["health"] = snapshot.Health;
            placeholders["maxhealth"] = snapshot.MaxHealth;
            placeholders["healthstatus"] = snapshot.HealthStatus;
            placeholders["healthpercentage"] = snapshot.MaxHealth > 0 ? (double)snapshot.Health / snapshot.MaxHealth * 100 : "";
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
            placeholders["room"] = snapshot.Room;
            placeholders["roomid"] = snapshot.RoomId;
            placeholders["mapid"] = snapshot.MapId;
            placeholders["objective"] = snapshot.Objective;
            placeholders["primaryobjective"] = snapshot.PrimaryObjective;
            placeholders["secondaryobjective"] = snapshot.SecondaryObjective;
            placeholders["objectivepath"] = snapshot.ObjectivePath;
            placeholders["objectives"] = snapshot.Objectives;
            placeholders["objectiveid"] = snapshot.ObjectiveId;
            placeholders["primaryobjectiveid"] = snapshot.PrimaryObjectiveId;
            placeholders["secondaryobjectiveid"] = snapshot.SecondaryObjectiveId;
            placeholders["weapon"] = snapshot.Weapon;
            placeholders["currentweapon"] = snapshot.Weapon;
            placeholders["weaponid"] = snapshot.WeaponId;
            placeholders["ammo"] = snapshot.Ammo < 0 ? "" : snapshot.Ammo;
            placeholders["loadedammo"] = placeholders["ammo"];
            placeholders["reserveammo"] = snapshot.InfiniteAmmo == true ? "∞" : snapshot.ReserveAmmo < 0 ? "" : snapshot.ReserveAmmo;
            placeholders["infiniteammo"] = snapshot.InfiniteAmmo.HasValue ? snapshot.InfiniteAmmo.Value : "";
            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE7: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil 7"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE7: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
