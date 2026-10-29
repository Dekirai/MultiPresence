using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.GFR;
using MultiPresence.Runtime;
using System.Globalization;

namespace MultiPresence.Presence
{
    public class GFR
    {
        private const string GameName = "Gunfire Reborn";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1342886727953416313");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Gunfire Reborn.json");
            PresenceRuntime.Start(nameof(GFR), GameName, RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(GameName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("GFR: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(GameName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(GameName))
                    {
                        ShowStatus("Waiting for game data");
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (!snapshot.IsInGame)
                    {
                        ShowStatus("In Menus / Loading");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    if (DateTime.UtcNow >= nextWarning)
                    {
                        AppLog.Warning("Gunfire Reborn memory is not ready.", exception);
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
            if (!snapshot.IsInGame)
            {
                return Task.FromResult(new Dictionary<string, object>());
            }

            return Task.FromResult(new Dictionary<string, object>
            {
                { "character", Characters.GetCharacter(snapshot.CharacterId) },
                { "characterid", snapshot.CharacterId },
                { "level", snapshot.Level },
                { "stage", Stages.GetStage(snapshot.StageId) },
                { "stageid", snapshot.StageId },
                { "difficulty", Difficulties.GetDifficulty(snapshot.Round, snapshot.Cycle) },
                { "copper", snapshot.Copper },
                { "soulessence", snapshot.SoulEssence },
                { "hp", snapshot.Hp.ToString("0.##", CultureInfo.InvariantCulture) },
                { "maxhp", snapshot.MaxHp.ToString("0.##", CultureInfo.InvariantCulture) },
                { "shield", snapshot.Shield.ToString("0.##", CultureInfo.InvariantCulture) },
                { "maxshield", snapshot.MaxShield.ToString("0.##", CultureInfo.InvariantCulture) }
            });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("GFR: presence is not initialized.");
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

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("GFR: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
