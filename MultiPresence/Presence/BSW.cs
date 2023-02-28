using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DiscordRPC;
using Memory;
using MultiPresence.Models.BSW;

namespace MultiPresence.Presence
{
    public class BSW
    {
        static Mem mem = new Mem();
        static string process = "BurningSW";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1079887873198784552");
            InitializeDiscord();
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
                int level = mem.ReadByte($"{process}.exe+13C9304");
                byte[] nickname_base = mem.ReadBytes($"{process}.exe+131F3D8", 24);
                string nickname = Encoding.Unicode.GetString(nickname_base);
                int character = mem.ReadByte($"{process}.exe+13C9308");
                var character_name = await Characters.GetCharacter(character);
                if (character == 255 || level == 0)
                {
                    discord.UpdateLargeAsset($"logo", $"Burning SoulWorker");
                    discord.UpdateDetails($"In Main Menu");
                    discord.UpdateState($"");
                }
                else
                {
                    discord.UpdateLargeAsset($"{character_name[1]}", $"Burning SoulWorker");
                    discord.UpdateDetails($"{nickname} (Lv. {level})");
                    discord.UpdateState($"Playing as {character_name[0]}");
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