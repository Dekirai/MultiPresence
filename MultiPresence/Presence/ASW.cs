using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DiscordRPC;
using Memory;
using MultiPresence.Models.ASW;

namespace MultiPresence.Presence
{
    public class ASW
    {
        static Mem mem = new Mem();
        static string process = "SoulWorker";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1264599106521927690");
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
                int level = mem.ReadByte($"{process}.exe+1792A28");
                byte[] nickname_base = mem.ReadBytes($"{process}.exe+16E2008", 24);
                string nickname = Encoding.Unicode.GetString(nickname_base);
                int character = mem.ReadByte($"{process}.exe+1792A2C");
                var character_name = await Characters.GetCharacter(character);

                var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "nickname", nickname },
                        { "character", character_name[0] }
                    };

                if (character == 255 || level == 0)
                {
                    discord.UpdateLargeAsset($"logo", $"asobiSW");
                    discord.UpdateDetails($"In Main Menu");
                    discord.UpdateState($"");
                }
                else
                {
                    string details = updater.UpdateDetails("asobiSW", placeholders);
                    string state = updater.UpdateState("asobiSW", placeholders);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                    discord.UpdateLargeAsset($"{character_name[1]}", $"asobiSW");
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