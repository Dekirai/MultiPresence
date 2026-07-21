#nullable disable
using MultiPresence.Runtime;
using DiscordRPC;
using MultiPresence.Models.RE4R;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE4R
    {
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1343156987021754399");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/config/Resident Evil 4 Remake.json");
            PresenceRuntime.Start(nameof(RE4R), "re4", RPC);
        }

        private static void GetPID()
        {
            try
            {
                ProcessMonitor.TryAttach("re4");
            }
            catch
            {
                //nothing?
            }
        }

        private static async Task RPC()
        {
            if (ProcessMonitor.IsRunning("re4"))
            {
                int chapter = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0D67C908, [0x30]), true);
                if (chapter >= 21100 && chapter <= 35100)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4", placeholders, "Main Story");
                }
                else if (chapter >= 41000 && chapter <= 44600)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4", placeholders, "Mercenaries");
                }
                else if (chapter >= 30100 && chapter < 41000)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4", placeholders, "Separate Ways");
                }
                else
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4", placeholders, "Main Menu");
                }
            }
            else
            {
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int difficulty = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0D67C908, [0x28]), true);
            int health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0D686200, [0x78, 0x18, 0x30, 0x148, 0x14]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0D686200, [0x78, 0x18, 0x30, 0x148, 0x10]), true);
            float healthpercentage = (int)Math.Floor(((float)health / maxhealth) * 100);
            int chapter = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0D67C908, [0x30]), true);
            string difficultystring = await Difficulties.GetDifficulty(difficulty);
            string state = "";
            string healthstatus = "";

            if (healthpercentage > 75)
                healthstatus = "Fine";
            else if (healthpercentage > 50)
                healthstatus = "Caution";
            else if (healthpercentage > 25)
                healthstatus = "Caution";
            else
                healthstatus = "Danger";

            if (chapter == 30100 || chapter == 31100 || chapter == 31200 || chapter == 32100 || chapter == 32200 || chapter == 33100 || chapter == 33200 || chapter == 34100 || chapter == 35100)
            {
                string realchapter = await Chapters.GetChapter(chapter);
                state = "Separate Ways";
                return new Dictionary<string, object>
                {
                    { "chapter", realchapter },
                    { "difficulty", difficultystring },
                    { "health", health },
                    { "maxhealth", maxhealth },
                    { "healthpercentage", healthpercentage },
                    { "healthstatus", healthstatus },
                    { "state", state }
                };
            }
            else if (chapter >= 21100 && chapter <= 35100)
            {
                string realchapter = await Chapters.GetChapter(chapter);
                state = "Main Story";
                return new Dictionary<string, object>
                {
                    { "chapter", realchapter },
                    { "difficulty", difficultystring },
                    { "health", health },
                    { "maxhealth", maxhealth },
                    { "healthpercentage", healthpercentage },
                    { "healthstatus", healthstatus },
                    { "state", state }
                };
            }
            else if (chapter >= 41000 && chapter <= 44600)
            {
                state = "Mercenaries";
                string map = "";
                string charactername = "";
                int reduction = 0;
                if (chapter >= 41000 && chapter < 42000)
                {
                    map = "Village";
                    reduction = 41000;
                }
                else if (chapter >= 42000 && chapter < 43000)
                {
                    map = "Castle";
                    reduction = 42000;
                }
                else if (chapter >= 43000 && chapter < 44000)
                {
                    map = "Island";
                    reduction = 43000;
                }
                else if (chapter >= 44000 && chapter < 45000)
                {
                    map = "The Docs";
                    reduction = 44000;
                }

                int character = chapter - reduction;
                if (character == 100)
                    charactername = "Leon";
                else if (character == 200)
                    charactername = "Luis";
                else if (character == 300)
                    charactername = "Krauser";
                else if (character == 400)
                    charactername = "Hunk";
                else if (character == 500)
                    charactername = "Ada";
                else if (character == 600)
                    charactername = "Wesker";

                int score = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0D66E940, [0xA8, 0xB0, 0xB8, 0x370]), true);

                return new Dictionary<string, object>
                {
                    { "map", map },
                    { "health", health },
                    { "character", charactername },
                    { "maxhealth", maxhealth },
                    { "healthpercentage", healthpercentage },
                    { "healthstatus", healthstatus },
                    { "score", score },
                    { "state", state }
                };
            }

            state = "Main Menu";
            return new Dictionary<string, object>
            {
                { "state", state },
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
        }
    }
}