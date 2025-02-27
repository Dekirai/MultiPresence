using DiscordRPC;
using MultiPresence.Models.RE4;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE4
    {
        public static ulong _main_address = 0;
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(7500);
            GetPID();
            _main_address = (ulong)Hypervisor.FindSignature("00 00 00 00 60 BB ?? ?? ?? ?? 96 40 00 00 00 00 E4 CB 96 40 ?? ?? ?? ?? E4 CB 96 40 00 00 00 00 E4 CB 96 40");
            discord = new DiscordRpcClient("982193093388427314");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("bio4")[0];
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
            Process[] game = Process.GetProcessesByName("bio4");
            if (game.Length > 0)
            {
                int stage = Hypervisor.Read<byte>(_main_address + 0x5B85, true);
                int chapter = Hypervisor.Read<byte>(_main_address + 0x5B72, true);
                int room = Hypervisor.Read<byte>(_main_address + 0x5B84, true);

                if (stage == 1 && room == 32)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersTitle);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4 (2005)", placeholders, "Title_Screen");
                }
                else
                {
                    if (stage == 4)
                    {
                        if (room >= 0 && room <= 4)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4 (2005)", placeholders, "Mercenaries");
                        }
                        else
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
                        }
                    }
                    else if (stage == 5)
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4 (2005)", placeholders, "Separate_Ways");
                    }
                    else
                    {
                        var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                        PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Resident Evil 4 (2005)", placeholders, "Main");
                    }
                }

                await Task.Delay(300);
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
            string area = "";
            int stage = Hypervisor.Read<byte>(_main_address + 0x5B85, true);
            int chapter = Hypervisor.Read<byte>(_main_address + 0x5B72, true);
            int room = Hypervisor.Read<byte>(_main_address + 0x5B84, true);
            int weapon = Hypervisor.Read<byte>(_main_address + 0x1745C, true);
            int character = Hypervisor.Read<byte>(_main_address + 0x5BA0, true);
            int score = Hypervisor.Read<int>(_main_address + 0x17544, true);
            int difficulty = Hypervisor.Read<byte>(_main_address + 0x9054, true);

            var room_name = await Stages.GetStage(stage);
            var weapon_name = await Weapons.GetWeapon(weapon);
            var difficulty_name = await Difficulties.GetDifficulty(difficulty);
            var character_name = await Characters.GetCharacter(character);
            var chapter_name = await Chapters.GetChapter(chapter);

            if (stage == 1)
                area = "Village";
            if (stage == 2)
                area = "Castle";
            if (stage == 3)
                area = "Island";
            return new Dictionary<string, object>
            {
                { "room", room_name[room] },
                { "chapter", chapter_name },
                { "area", area },
                { "area_icon_name", area.ToLower() },
                { "difficulty", difficulty_name },
                { "weapon", weapon_name },
                { "character", character_name },
                { "score", score},
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersTitle()
        {
            string area = "Main Menu";
            return new Dictionary<string, object>
            {
                { "area", area }
            };
        }

        private static void InitializeDiscord()
        {
            discord.Initialize();
            discord.SetPresence(new RichPresence()
            {
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}