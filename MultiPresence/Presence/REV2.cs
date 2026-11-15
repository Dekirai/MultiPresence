using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.REV2;
using MultiPresence.Infrastructure;

namespace MultiPresence.Presence
{
    public class REV2
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static void DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            ProcessMonitor.TryAttach("rerev2");
            discord = new DiscordRpcClient("1213180163446149121");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil Revelations 2.json", "Resident Evil Revelations 2", "Campaign");
            PresenceRuntime.Start(nameof(REV2), "rerev2", RPC);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("REV2: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("rerev2"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("rerev2"))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = MemoryReader.ReadSnapshot();
                    if (snapshot.State is "Lobby" or "Ingame" or "Campaign")
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil Revelations 2", placeholders, snapshot.State);
                    }
                    else
                    {
                        ShowStatus(snapshot.State == "Loading" ? "Loading" : "");
                    }
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Resident Evil Revelations 2 memory is not ready.", exception);
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders(GameSnapshot snapshot)
        {
            string campaign = snapshot.State != "Campaign" ? "" : snapshot.CampaignMode switch
            {
                3 => "The Struggle",
                4 => "Little Miss",
                _ => "Main Campaign"
            };
            string campaignDisplay = "";
            if (snapshot.State == "Campaign")
            {
                if (snapshot.CampaignMode is 3 or 4) campaignDisplay = $"Extra Episode: {campaign}";
                else campaignDisplay = snapshot.Episode.HasValue ? $"Campaign: Episode {snapshot.Episode}" : campaign;
            }
            string chapter = snapshot.ChapterId is int id ? await Chapters.GetChapter(id) : "";
            string difficulty = snapshot.DifficultyId switch
            {
                0 => "Normal",
                1 => "Hard",
                2 => "Very Hard",
                3 => "Code Red",
                _ => ""
            };
            string missionDisplay = "";
            if (snapshot.State == "Ingame")
            {
                if (snapshot.Mission.HasValue)
                {
                    string prefix = chapter.Length > 0 ? $"{chapter}-" : "";
                    missionDisplay = $"Mission {prefix}{snapshot.Mission:00}";
                }
                else missionDisplay = "In Mission";
                if (difficulty.Length > 0) missionDisplay += $" | {difficulty}";
                if (snapshot.RecommendedLevel.HasValue) missionDisplay += $" | Recommended Lv. {snapshot.RecommendedLevel}";
            }

            return new Dictionary<string, object>
            {
                { "character", snapshot.CharacterId is int character ? await Characters.GetCharacter(character) : snapshot.CampaignCharacter },
                { "episode", snapshot.Episode is int episode ? episode : "" },
                { "campaign", campaign },
                { "campaigndisplay", campaignDisplay },
                { "location", snapshot.Location },
                { "locationdisplay", snapshot.Location.Length > 0 ? $" | {snapshot.Location}" : "" },
                { "partner", snapshot.Partner },
                { "partnerdisplay", snapshot.Partner.Length > 0 ? $" | Partner: {snapshot.Partner}" : "" },
                { "objective", snapshot.Objective },
                { "level", snapshot.Level is int level ? level : "" },
                { "money", snapshot.Money is long money ? money : "" },
                { "chapter", chapter },
                { "mission", snapshot.Mission is int mission ? mission : "" },
                { "missiondisplay", missionDisplay },
                { "difficulty", difficulty },
                { "recommendedlevel", snapshot.RecommendedLevel is int recommended ? recommended : "" },
                { "kills", snapshot.Kills is int kills ? kills : "" },
                { "enemytarget", snapshot.EnemyTarget is int target ? target : "" },
                { "enemyprogress", snapshot.Kills.HasValue && snapshot.EnemyTarget.HasValue ? $" | Enemies: {snapshot.Kills}/{snapshot.EnemyTarget}" : "" },
                { "medallions", snapshot.Badges is int badges ? badges : "" },
                { "playername", snapshot.PlayerName },
                { "redhearts", snapshot.RedHearts is int red ? red : "" },
                { "bluehearts", snapshot.BlueHearts is int blue ? blue : "" }
            };
        }

        private static void ShowStatus(string details)
        {
            discord?.SetPresence(new RichPresence()
            {
                Details = details,
                State = "",
                Assets = new Assets
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Resident Evil Revelations 2"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("REV2: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
