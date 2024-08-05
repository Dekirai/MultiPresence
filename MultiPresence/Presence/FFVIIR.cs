using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using DiscordRPC;
using Memory;

namespace MultiPresence.Presence
{
    public class FFVIIR
    {
        static Mem mem = new Mem();
        static string process = "ff7remake_";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1270065791957471242");
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
                int level = mem.ReadByte($"{process}.exe+059907A8,0x288,0x430,0x138,0x3B8,0x388,0x28,0x20");
                int hp = mem.ReadInt($"{process}.exe+059907A8,0x288,0x430,0x138,0x3B8,0x388,0x28,0x30");
                int maxhp = mem.ReadInt($"{process}.exe+059907A8,0x288,0x430,0x138,0x3B8,0x388,0x28,0x34");
                int mp = mem.ReadInt($"{process}.exe+059907A8,0x288,0x430,0x138,0x3B8,0x388,0x28,0x38");
                int maxmp = mem.ReadInt($"{process}.exe+059907A8,0x288,0x430,0x138,0x3B8,0x388,0x28,0x3C");

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "hp", hp },
                    { "maxhp", maxhp },
                    { "mp", mp },
                    { "maxmp", maxmp }
                };

                discord.UpdateLargeAsset($"logo", $"Final Fantasy VII Remake");
                string details = updater.UpdateDetails("Final Fantasy VII Remake", placeholders);
                string state = updater.UpdateState("Final Fantasy VII Remake", placeholders);
                discord.UpdateDetails(details);
                discord.UpdateState(state);

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