using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.RE4;

namespace MultiPresence.Presence
{
    public class RE4
    {
        static Mem mem = new Mem();
        static string process = "bio4";
        public static string _main_address = "";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            await Task.Delay(7500);
            GetPID();
            long main_get = (await mem.AoBScan("00 00 00 00 60 BB ?? ?? ?? ?? 96 40 00 00 00 00 E4 CB 96 40 ?? ?? ?? ?? E4 CB 96 40 00 00 00 00 E4 CB 96 40", true)).FirstOrDefault();
            _main_address = main_get.ToString("X8");
            discord = new DiscordRpcClient("982193093388427314");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private static async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                string area = "";
                int stage = mem.ReadByte($"{_main_address}+5B85");
                int chapter = mem.ReadByte($"{_main_address}+5B72");
                int room = mem.ReadByte($"{_main_address}+5B84");
                int weapon = mem.ReadByte($"{_main_address}+1745C");
                int character = mem.ReadByte($"{_main_address}+5BA0");
                int score = mem.ReadInt($"{_main_address}+17544");
                int difficulty = mem.ReadByte($"{_main_address}+9054");

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

                var placeholders = new Dictionary<string, object>
                    {
                        { "room", room_name[room] },
                        { "chapter", chapter_name },
                        { "area", area },
                        { "difficulty", difficulty_name },
                        { "weapon", weapon_name },
                        { "character", character_name },
                        { "score", score},
                    };

                if (stage == 1 && room == 32)
                {
                    discord.UpdateLargeAsset("logo", $"Resident Evil 4");
                    discord.UpdateSmallAsset("", "");
                    string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Title_Screen");
                    string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Title_Screen");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else
                {
                    if (stage == 4)
                    {
                        if (room >= 0 && room <= 4)
                        {
                            discord.UpdateLargeAsset($"logo_alt", $"The Mercenaries");
                            discord.UpdateSmallAsset("", "");
                            string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Mercenaries");
                            string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Mercenaries");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                        }
                        else
                        {
                            discord.UpdateLargeAsset($"ada", $"Assignment Ada");
                            discord.UpdateSmallAsset("", "");
                            string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
                            string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Assignment_Ada");
                            discord.UpdateDetails(details);
                            discord.UpdateState(state);
                        }
                    }
                    else if (stage == 5)
                    {
                        discord.UpdateLargeAsset($"ada", $"Separate Ways - {room_name[room]}");
                        discord.UpdateSmallAsset("", "");
                        string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
                        string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Separate_Ways");
                        discord.UpdateDetails(details);
                        discord.UpdateState(state);
                    }
                    else
                    {
                        if (stage == 0)
                            discord.UpdateLargeAsset($"logo_alt", $"{area} - {room_name[room]}");
                        else
                            discord.UpdateLargeAsset($"{area.ToLower()}", $"{area} - {room_name[room]}");
                        discord.UpdateSmallAsset("logo", $"Playing on {difficulty_name}");
                        string details = updater.UpdateDetails("Resident Evil 4 (2005)", placeholders, "Main");
                        string state = updater.UpdateState("Resident Evil 4 (2005)", placeholders, "Main");
                        discord.UpdateDetails(details);
                        discord.UpdateState(state);
                    }
                }

                await Task.Delay(3000);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
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