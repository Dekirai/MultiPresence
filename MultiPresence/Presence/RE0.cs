using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.RE0;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class RE0
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1566462142948057200");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Resident Evil 0.json");
            PresenceRuntime.Start(nameof(RE0), "re0hd", RPC);
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("re0hd");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE0: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("re0hd"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("re0hd"))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State is not "Default" and not "Ingame")
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "Waiting for game data");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 0", placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Resident Evil 0 memory is not ready.", exception);
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
                { "character", "" },
                { "characterid", "" },
                { "difficulty", "" },
                { "difficultyid", "" },
                { "location", "" },
                { "area", "" },
                { "region", "" },
                { "floor", "" },
                { "room", "" },
                { "roomid", "" },
                { "weapon", "" },
                { "currentweapon", "" },
                { "weaponid", "" },
                { "weaponammo", "" },
                { "ammo", "" },
                { "loadedammo", "" },
                { "reserveammo", "" },
                { "infiniteammo", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["character"] = snapshot.Character;
            placeholders["characterid"] = snapshot.CharacterId < 0 ? "" : snapshot.CharacterId;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["difficultyid"] = snapshot.DifficultyId < 0 ? "" : snapshot.DifficultyId;
            placeholders["location"] = snapshot.Location;
            placeholders["area"] = snapshot.Region;
            placeholders["region"] = snapshot.Region;
            placeholders["floor"] = snapshot.Floor;
            placeholders["room"] = snapshot.Room;
            placeholders["roomid"] = snapshot.RoomId < 0 ? "" : snapshot.RoomId;
            placeholders["weapon"] = snapshot.Weapon;
            placeholders["currentweapon"] = snapshot.Weapon;
            placeholders["weaponid"] = snapshot.WeaponId < 0 ? "" : snapshot.WeaponId;
            placeholders["ammo"] = snapshot.InfiniteAmmo ? "∞" : snapshot.Ammo < 0 ? "" : snapshot.Ammo;
            placeholders["loadedammo"] = snapshot.Ammo < 0 ? "" : snapshot.Ammo;
            placeholders["reserveammo"] = snapshot.InfiniteAmmo ? "∞" : snapshot.ReserveAmmo < 0 ? "" : snapshot.ReserveAmmo;
            placeholders["infiniteammo"] = snapshot.InfiniteAmmo;
            var ammunition = snapshot.InfiniteAmmo ? "∞" : snapshot.Ammo >= 0 ? snapshot.Ammo + (snapshot.ReserveAmmo >= 0 ? $" / {snapshot.ReserveAmmo}" : "") : "";
            placeholders["weaponammo"] = ammunition.Length == 0 ? snapshot.Weapon : $"{snapshot.Weapon} | Ammo: {ammunition}";
            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE0: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil 0"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE0: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
