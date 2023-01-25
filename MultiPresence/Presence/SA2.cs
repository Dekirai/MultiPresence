using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.SA2;

namespace MultiPresence.Presence
{
    public class SA2
    {
        static Mem mem = new Mem();
        static string process = "sonic2app";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("982562008228560986");
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
                int stage = mem.ReadByte($"{process}.exe+1534B70");
                int character = mem.ReadByte($"{process}.exe+1534B80");
                int lives = mem.ReadByte($"{process}.exe+134B024");
                int rings = mem.ReadInt($"{process}.exe+134B028");
                int p1_costume = mem.ReadByte($"{process}.exe+134B015");
                int islevel = mem.ReadByte($"{process}.exe+15420FF");

                var stage_name = await Stages.GetStage(stage);
                var character_name = await Characters.GetCharacter(character);

                if (islevel == 0)
                {
                    discord.UpdateLargeAsset($"logo", $"Sonic Adventure 2");
                    discord.UpdateDetails($"Idle");
                    discord.UpdateState(null);
                }
                else
                {
                    if (p1_costume > 0)
                        discord.UpdateLargeAsset($"{character_name[3]}", $"{character_name[2]}");
                    else
                        discord.UpdateLargeAsset($"{character_name[1]}", $"{character_name[0]}");
                    discord.UpdateDetails($"Lives: {lives}, Rings: {rings}");
                    discord.UpdateState($"{stage_name}");
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