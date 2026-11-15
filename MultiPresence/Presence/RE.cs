using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.RE;

namespace MultiPresence.Presence
{
    public class RE
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;

        public static void DoAction()
        {
            MemoryReader.Reset();
            ProcessMonitor.TryAttach("bhd");
            discord = new DiscordRpcClient("1212466561068171294");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 1.json");
            PresenceRuntime.Start(nameof(RE), "bhd", RPC);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("RE: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("bhd"))
            {
                try
                {
                    if (Hypervisor.IsAttached || ProcessMonitor.TryAttach("bhd"))
                    {
                        var snapshot = MemoryReader.ReadSnapshot();
                        if (snapshot.State == "Ingame")
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil", placeholders);
                            return;
                        }
                    }
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    // Game objects can disappear while returning to the title screen.
                }

                discord.SetPresence(new RichPresence()
                {
                    Details = "In Main Menu",
                    State = "",
                    Assets = new Assets()
                    {
                        LargeImageKey = "logo",
                        LargeImageText = "Resident Evil"
                    },
                    Timestamps = PlaceholderHelper._startTimestamp
                });
            }
            else
            {
                MemoryReader.Reset();
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders(GameSnapshot snapshot)
        {
            var location = await Stages.GetLocation(snapshot.Floor, snapshot.Room, snapshot.Camera);
            string character = "";
            string character_icon = "";

            string weapon = await Weapons.GetWeapon(snapshot.Weapon);

            string healthstatus = "";

            double percentage = (double)snapshot.Health / snapshot.MaxHealth * 100;

            if (percentage > 75)
                healthstatus = "Fine";
            else if (percentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            if (snapshot.Character == 0)
            {
                character = "Chris Redfield";
                character_icon = "chris";
            }
            else if (snapshot.Character == 1)
            {
                character = "Jill Valentine";
                character_icon = "jill";
            }
            else if (snapshot.Character == 2)
            {
                character = "Rebecca Chambers";
                character_icon = "rebecca";
            }

            return new Dictionary<string, object>
            {
                { "floor", location.Floor },
                { "room", location.Room },
                { "character", character },
                { "character_icon", character_icon },
                { "health", snapshot.Health },
                { "maxhealth", snapshot.MaxHealth },
                { "healthstatus", healthstatus },
                { "weapon", weapon },
                { "ammoclip", (int)snapshot.AmmoClip },
                { "maxammoclip", (int)snapshot.MaxAmmoClip }
            };
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("RE: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
