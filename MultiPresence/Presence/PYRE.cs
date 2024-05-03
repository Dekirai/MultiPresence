using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.PYRE;
using Button = DiscordRPC.Button;

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
                string nickname = mem.ReadString($"{process}.exe+00A7D6A4,0x5AC");
                int stage_get = mem.ReadByte($"{process}.exe+A47E2C,0x10950");
                int mode_get = mem.ReadByte($"{process}.exe+A47E2C,0x1094C");
                int level_get = mem.ReadByte($"{process}.exe+00A7D6A4,0x711");
                int room = mem.ReadByte($"{process}.exe+A47E2C,0x1094D");
                int players = mem.ReadByte($"{process}.exe+A47E2C,0x10938");
                int playersmax = mem.ReadByte($"{process}.exe+A47E2C,0x10937");
                int isIngame = mem.ReadByte($"{process}.exe+A47E2C,0x10B27");
                int currenthole = mem.ReadByte($"{process}.exe+A47E2C,0xFFEC");
                int maxholes = mem.ReadByte($"{process}.exe+A47E2C,0x1094B");

                var stage = await Stages.GetStage(stage_get);
                var mode = await Modes.GetMode(mode_get);             
                var level = await Levels.GetLevel(level_get);

                if (mode_get == 23 || mode_get == 255 || mode_get == 40)
                {
                    discord.UpdateLargeAsset($"logo", $"Pangya Reborn");
                    discord.UpdateDetails($"{nickname} - {level[0]}");
                    discord.UpdateState($"In Lobby");
                }
                else
                {
                    if (isIngame == 1)
                    {
                        discord.UpdateLargeAsset($"{stage_get}", $"{stage[0]}");
                        discord.UpdateDetails($"{nickname} - {level[0]}");
                        if (mode_get == 0 || mode_get == 1 || mode_get == 4 || mode_get == 7 || mode_get == 10)
                            discord.UpdateState($"{mode[0]} » #{room} - H{currenthole}/{maxholes}");
                        else
                            discord.UpdateState($"{mode[0]} » #{room} - {players}/{playersmax} Players");
                    }
                    else
                    {
                        discord.UpdateDetails($"{nickname} - {level[0]}");
                        discord.UpdateState($"{mode[0]} » #{room} ({players}/{playersmax} Players)");
                        discord.UpdateLargeAsset($"{stage_get}", $"{stage[0]}");
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
                    new Button() { Label = $"Website", Url = "https://www.pangyareborn.com/" },
                    new Button() { Label = $"Discord", Url = "https://www.discord.com/invite/D9gC9Hhpss" }
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}