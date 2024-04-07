using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.PYRE;

namespace MultiPresence.Presence
{
    public class PYRE
    {
        static Mem mem = new Mem();
        static string process = "ProjectG";
        private static DiscordRpcClient discord;
        public static async void DoAction()
        {
            await Task.Delay(10000);
            GetPID();
            discord = new DiscordRpcClient("1226462236181004338");
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
                string nickname = mem.ReadString($"{process}.exe+00A47E2C,0x3C0,0x0,0x8,0x1A0,0x228,0x1BC,0x5AC");
                int stage_get = mem.ReadByte($"{process}.exe+A47E2C,0x1058C");
                int mode_get = mem.ReadByte($"{process}.exe+A47E2C,0x1094C");
                int room = mem.ReadByte($"{process}.exe+A47E2C,0x1094D");
                int players = mem.ReadByte($"{process}.exe+A47E2C,0x10938");
                int playersmax = mem.ReadByte($"{process}.exe+A47E2C,0x10937");

                var stage = await Stages.GetStage(stage_get);
                var mode = await Modes.GetMode(mode_get);

                if (mode_get == 23 || mode_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"PangYa Reborn");
                    discord.UpdateDetails($"{nickname}");
                    discord.UpdateState($"In Lobby");
                }
                else
                {
                    discord.UpdateDetails($"{nickname} - In Room #{room}");
                    discord.UpdateState($"{mode[0]} ({players}/{playersmax} Players)");
                    discord.UpdateLargeAsset($"https://dekirai.crygod.de/rpc/multipresence/pyre/{stage_get}.png", $"{stage[0]}");
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