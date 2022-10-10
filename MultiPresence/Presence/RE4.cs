using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.RE4;

namespace MultiPresence.Presence
{
    public class RE4
    {
        Mem mem = new Mem();
        string process = "bio4";
        public string _main_address = "";
        private static DiscordRpcClient discord;
        public async void DoAction()
        {
            await Task.Delay(7500);
            GetPID();
            long main_get = (await mem.AoBScan("00 00 00 00 60 BB ?? ?? ?? ?? 96 40 00 00 00 00 E4 CB 96 40 ?? ?? ?? ?? E4 CB 96 40 00 00 00 00 E4 CB 96 40", true)).FirstOrDefault();
            _main_address = main_get.ToString("X8");
            discord = new DiscordRpcClient("982193093388427314");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private async void RPC()
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

                if (stage == 1 && room == 32)
                {
                    discord.UpdateLargeAsset("logo", $"Resident Evil 4");
                    discord.UpdateSmallAsset(null, null);
                    discord.UpdateDetails($"At the Title Screen");
                    discord.UpdateState(null);
                }
                else
                {
                    if (stage == 4)
                    {
                        if (room >= 0 && room <= 4)
                        {
                            discord.UpdateLargeAsset($"logo_alt", $"The Mercenaries");
                            discord.UpdateSmallAsset("", "");
                            discord.UpdateDetails($"The Mercenaries - 🎯{score}");
                            discord.UpdateState($"🧑{character_name} on {room_name[room]}");
                        }
                        else
                        {
                            discord.UpdateLargeAsset($"ada", $"Assignment Ada");
                            discord.UpdateSmallAsset("", "");
                            discord.UpdateDetails($"Assignment Ada - 🔫{weapon_name}");
                            discord.UpdateState($"📍{room_name[room]}");
                        }
                    }
                    else if (stage == 5)
                    {
                        discord.UpdateLargeAsset($"ada", $"Separate Ways - {room_name[room]}");
                        discord.UpdateSmallAsset("", "");
                        discord.UpdateDetails($"Separate Ways - 🔫{weapon_name}");
                        discord.UpdateState($"📕Chapter {chapter_name} 📍{room_name[room]}");
                    }
                    else
                    {
                        if (stage == 0)
                            discord.UpdateLargeAsset($"logo_alt", $"{area} - {room_name[room]}");
                        else
                            discord.UpdateLargeAsset($"{area.ToLower()}", $"{area} - {room_name[room]}");
                        discord.UpdateSmallAsset("logo", $"Playing on {difficulty_name}");
                        discord.UpdateDetails($"🔫{weapon_name} ⚖️{difficulty_name}");
                        discord.UpdateState($"📕Chapter {chapter_name} 📍{room_name[room]}");
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
                Buttons = new Button[]
                {
#if DEBUG
                    new Button() { Label = $"Powered by MultiPresence", Url = "https://github.com/Dekirai/MultiPresence" }
#endif
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}