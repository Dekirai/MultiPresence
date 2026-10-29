using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.FFXVI;
using MultiPresence.Runtime;

namespace MultiPresence.Presence
{
    public class FFXVI
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1285884197084336161");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Final Fantasy XVI.json");
            PresenceRuntime.Start(nameof(FFXVI), "ffxvi", RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach("ffxvi");
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("FFXVI: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning("ffxvi"))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach("ffxvi"))
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
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Final Fantasy XVI", placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Final Fantasy XVI memory is not ready.", exception);
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
                { "character", "" },
                { "level", "" },
                { "hp", "" },
                { "maxhp", "" },
                { "gil", "" },
                { "abilitypoints", "" },
                { "difficulty", "" },
                { "limitbreak", "" },
                { "maxlimitbreak", "" },
                { "limitbreakraw", "" },
                { "location", "" },
                { "region", "" },
                { "area", "" },
                { "mapid", "" },
                { "quest", "" },
                { "questid", "" },
                { "objective", "" },
                { "objectiveid", "" }
            };

            if (snapshot.State != "Ingame")
            {
                return Task.FromResult(placeholders);
            }

            placeholders["character"] = snapshot.Character;
            placeholders["level"] = snapshot.Level;
            placeholders["hp"] = snapshot.Hp;
            placeholders["maxhp"] = snapshot.MaxHp;
            placeholders["gil"] = snapshot.Gil;
            placeholders["abilitypoints"] = snapshot.AbilityPoints;
            placeholders["difficulty"] = snapshot.Difficulty;
            placeholders["limitbreak"] = snapshot.LimitBreakBars;
            placeholders["maxlimitbreak"] = snapshot.MaxLimitBreakBars;
            placeholders["limitbreakraw"] = snapshot.LimitBreak;
            placeholders["location"] = snapshot.Location;
            placeholders["region"] = snapshot.Region;
            placeholders["area"] = snapshot.Area;
            placeholders["mapid"] = snapshot.MapId;
            placeholders["quest"] = snapshot.Quest;
            placeholders["questid"] = snapshot.QuestId;
            placeholders["objective"] = snapshot.QuestObjective;
            placeholders["objectiveid"] = snapshot.QuestObjectiveId;
            return Task.FromResult(placeholders);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFXVI: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Final Fantasy XVI"
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("FFXVI: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
