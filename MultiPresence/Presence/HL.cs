using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.HL;
using MultiPresence.Runtime;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class HL
    {
        private const string GameName = "Hogwarts Legacy";
        private const string ProcessName = "HogwartsLegacy";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static DateTime nextWarning;

        public static Task DoAction()
        {
            MemoryReader.Reset();
            nextWarning = DateTime.MinValue;
            GetPID();
            discord = new DiscordRpcClient("1324797968682979481");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Hogwarts Legacy.json");
            PresenceRuntime.Start(nameof(HL), ProcessName, RPC);
            return Task.CompletedTask;
        }

        private static bool GetPID()
        {
            if (Hypervisor.IsAttached && IsGameProcess(Hypervisor.Process))
            {
                return true;
            }

            var processes = Process.GetProcessesByName(ProcessName);
            try
            {
                foreach (var process in processes)
                {
                    if (IsGameProcess(process))
                    {
                        Hypervisor.AttachProcess(process);
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                foreach (var process in processes)
                {
                    process.Dispose();
                }
            }
        }

        private static bool IsGameProcess(Process? process)
        {
            try
            {
                return process != null && !process.HasExited &&
                    process.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase) &&
                    process.MainModule?.FileName.EndsWith(@"\Phoenix\Binaries\Win64\HogwartsLegacy.exe", StringComparison.OrdinalIgnoreCase) == true;
            }
            catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
            {
                return false;
            }
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("HL: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(ProcessName))
            {
                try
                {
                    if (!GetPID())
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
                        AppLog.Warning("Hogwarts Legacy memory is not ready.", exception);
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
                { "character", snapshot.Character },
                { "level", snapshot.Level },
                { "location", snapshot.Location },
                { "difficulty", Difficulties.GetDifficulty(snapshot.Difficulty) },
                { "quest", Missions.GetQuest(snapshot.QuestId) },
                { "mods", snapshot.Modded switch
                    {
                        true => "Mods Enabled",
                        false => "No Mods",
                        _ => "Mod Status Unknown"
                    }
                }
            });
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("HL: presence is not initialized.");
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
                throw new InvalidOperationException("HL: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
