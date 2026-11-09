using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.TK8;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class TK8
    {
        private const string ProcessName = "Polaris-Win64-Shipping";
        private const string GameName = "TEKKEN 8";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1569375240146915338");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/TEKKEN 8.json");
            PresenceRuntime.Start(nameof(TK8), ProcessName, RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(ProcessName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("TK8: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(ProcessName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(ProcessName))
                    {
                        discord.ClearPresence();
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State == "Loading")
                    {
                        ShowStatus("Loading");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("TEKKEN 8 memory is not ready.", exception);
                        nextWarning = DateTime.UtcNow.AddSeconds(30);
                    }

                    discord.ClearPresence();
                }
            }
            else
            {
                StopPresence();
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
                { "mode", "" },
                { "session", "" },
                { "activity", "" },
                { "stage", "" },
                { "stageid", "" },
                { "character", "" },
                { "opponent", "" },
                { "side", "" },
                { "rank", "" },
                { "rankid", "" },
                { "leftcharacter", "" },
                { "rightcharacter", "" },
                { "score", "" },
                { "roundswon", "" },
                { "opponentrounds", "" },
                { "leftrounds", "" },
                { "rightrounds", "" },
                { "round", "" },
                { "roundstowin", "" },
                { "bestof", "" },
                { "matchup", "" },
                { "matchdetails", "" }
            };

            if (snapshot.State is "Default" or "Loading") return Task.FromResult(placeholders);
            string activity = Join(snapshot.Mode, snapshot.Session, " | ");
            placeholders["mode"] = snapshot.Mode;
            placeholders["session"] = snapshot.Session;
            placeholders["activity"] = activity;
            if (snapshot.State != "Ingame") return Task.FromResult(placeholders);

            bool left = snapshot.LocalSide != 1;
            string first = left ? snapshot.LeftFighter : snapshot.RightFighter;
            string second = left ? snapshot.RightFighter : snapshot.LeftFighter;
            int? firstScore = left ? snapshot.LeftRounds : snapshot.RightRounds;
            int? secondScore = left ? snapshot.RightRounds : snapshot.LeftRounds;
            string score = firstScore.HasValue && secondScore.HasValue ? $"{firstScore}-{secondScore}" : "";
            if (snapshot.LocalSide is 0 or 1)
            {
                placeholders["character"] = first;
                placeholders["side"] = left ? "Left" : "Right";
                placeholders["rank"] = snapshot.Rank;
                placeholders["rankid"] = Number(snapshot.RankId);
            }
            placeholders["matchdetails"] = Join(activity, snapshot.Stage, " | ");
            placeholders["stage"] = snapshot.Stage;
            placeholders["stageid"] = Number(snapshot.StageId);
            if (snapshot.IsMatchmaking)
            {
                placeholders["matchup"] = snapshot.LocalSide.HasValue ? Join(first, snapshot.Rank, " | ") : "";
                return Task.FromResult(placeholders);
            }
            if (snapshot.LocalSide is 0 or 1)
            {
                placeholders["opponent"] = second;
                placeholders["score"] = score;
                placeholders["roundswon"] = Number(firstScore);
                placeholders["opponentrounds"] = Number(secondScore);
            }
            placeholders["leftcharacter"] = snapshot.LeftFighter;
            placeholders["rightcharacter"] = snapshot.RightFighter;
            placeholders["leftrounds"] = Number(snapshot.LeftRounds);
            placeholders["rightrounds"] = Number(snapshot.RightRounds);
            placeholders["round"] = Number(snapshot.CurrentRound);
            placeholders["roundstowin"] = Number(snapshot.RoundsToWin);
            placeholders["bestof"] = Number(snapshot.RoundsToWin * 2 - 1);
            var local = first.Length != 0 && snapshot.LocalSide.HasValue && snapshot.Rank.Length != 0
                ? $"{first} ({snapshot.Rank})" : first;
            placeholders["matchup"] = Join(Join(local, second, " vs "), score, " | ");
            return Task.FromResult(placeholders);
        }

        private static string Number(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "";

        private static string Join(string first, string second, string separator)
        {
            if (first.Length == 0) return second;
            if (second.Length == 0) return first;
            return first + separator + second;
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("TK8: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = GameName
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private static void StopPresence()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("TK8: presence is not initialized.");
            }

            MemoryReader.Reset();
            discord.ClearPresence();
            discord.Deinitialize();
            updater.Dispose();
            PresenceRuntime.RequestDetection();
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("TK8: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
