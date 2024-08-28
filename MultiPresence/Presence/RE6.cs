using DiscordRPC;
using Memory;
using MultiPresence.Models.RE6;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE6
    {
        static Mem mem = new Mem();
        static string process = "BH6";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212349543463518268");
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
                int stage_get = mem.Read2Byte("BH6.exe+1466884,0xA422C");
                int state_get = mem.Read2Byte("BH6.exe+1466884,0xA4228");
                var stagevalue = await Stages.GetStage(stage_get);

                string[] stage = stagevalue.Split(':');

                var placeholders = new Dictionary<string, object>
                    {
                        { "chapter", stage[0] },
                        { "room", stage[1] }
                    };

                if (state_get == 0)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Booting");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Booting");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else if (state_get > 1 && state_get < 9)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Ingame");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Ingame");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else if (state_get == 20)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Main_Menu");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Main_Menu");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else if (state_get == 10)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Saving");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Saving");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else if (state_get > 20)
                {
                    string details = updater.UpdateDetails("Resident Evil 6", placeholders, "Cutscene");
                    string state = updater.UpdateState("Resident Evil 6", placeholders, "Cutscene");
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }

                discord.UpdateLargeAsset($"logo", $"Resident Evil 6");


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