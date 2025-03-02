using DiscordRPC;
using MultiPresence.Models.RE4R;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE4R
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1343156987021754399");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("re4")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName("re4");
            if (game.Length > 0)
            {
                int chapter = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0DBB8808, [0x30]), true);
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

                await Task.Delay(1000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int difficulty = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x0DBB8808, [0x28]), true);
            int health = Hypervisor.Read<int>(Hypervisor.GetPointer64(0xDC028A8, [0x78, 0x18, 0x30, 0x148, 0x14]), true);
            int maxhealth = Hypervisor.Read<int>(Hypervisor.GetPointer64(0xDC028A8, [0x78, 0x18, 0x30, 0x148, 0x10]), true);
            float healthpercentage = (int)Math.Floor(((float)health / maxhealth) * 100);
            int chapter = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x0DBB8808, [0x30]), true);
            string difficultystring = await Difficulties.GetDifficulty(difficulty);
            string state = "";
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
                    { "state", state },
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
                    { "state", state },
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

                int score = Hypervisor.Read<int>(Hypervisor.GetPointer64(0xDBB8808, [0x50, 0x1A8, 0xA8, 0xB0, 0xB8, 0x350, 0x28]), true);

                return new Dictionary<string, object>
                {
                    { "map", map },
                    { "health", health },
                    { "character", charactername },
                    { "maxhealth", maxhealth },
                    { "healthpercentage", healthpercentage },
                    { "score", score },
                    { "state", state },
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