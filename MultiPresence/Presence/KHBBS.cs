using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.KHBBS;

namespace MultiPresence.Presence
{
    public class KHBBS
    {
        static Mem mem = new Mem();
        static string process = "KINGDOM HEARTS Birth by Sleep FINAL MIX";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("839545395368820806");
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
                int world_get = mem.ReadByte($"{process}.exe+817120");
                int room_get = mem.ReadByte($"{process}.exe+817121");
                int difficulty_get = mem.ReadByte($"{process}.exe+10F9F801");
                int character_get = mem.ReadByte($"{process}.exe+10F9DDCC");
                int level = mem.ReadByte($"{process}.exe+10F9DE61");
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var character = await Characters.GetCharacter(character_get);
                var room = await Rooms.GetRoom(world[0]);
                
                try
                {
                    discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                    discord.UpdateSmallAsset($"{character.ToLower()}", $"Playing as {character}");
                    var placeholders = new Dictionary<string, object>
                    {
                        { "level", level },
                        { "room", room[room_get] },
                        { "world", world },
                        { "difficulty", difficulty }
                    };
                    string details = updater.UpdateDetails("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    string state = updater.UpdateState("Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                catch
                {
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails("");
                    discord.UpdateSmallAsset($"");
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
