using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.KHBBS;

namespace MultiPresence.Presence
{
    public class KHBBS
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS Birth by Sleep FINAL MIX";
        private static DiscordRpcClient discord;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("839545395368820806");
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
                int world_get = mem.ReadByte($"{process}.exe+8150A0");
                int room_get = mem.ReadByte($"{process}.exe+8150A1");
                int difficulty_get = mem.ReadByte($"{process}.exe+10F890F1");
                int character_get = mem.ReadByte($"{process}.exe+10F9B2CC");
                int level = mem.ReadByte($"{process}.exe+10F9B361");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var character = await Characters.GetCharacter(character_get);
                var room = await Rooms.GetRoom(world[0]);
                
                try
                {
                    discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                    discord.UpdateSmallAsset($"{character.ToLower()}", $"Playing as {character}");
                    discord.UpdateState($"{room[room_get]}");
                    discord.UpdateDetails($"Lv. {level} ({difficulty})");
                }
                catch
                {
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails(null);
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
