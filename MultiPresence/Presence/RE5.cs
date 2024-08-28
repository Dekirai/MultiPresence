using DiscordRPC;
using Memory;
using MultiPresence.Models.RE5;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class RE5
    {
        static Mem mem = new Mem();
        static string process = "re5dx9";
        private static DiscordRpcClient discord;
        private static DiscordStatusUpdater updater;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1212083539567186002");
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
                int stage_get = mem.ReadInt("re5dx9.exe+00DB2158,0x273D8");
                int chris_health = mem.Read2Byte("re5dx9.exe+00DB27DC,0x24,0x1364");
                int sheva_health = mem.Read2Byte("re5dx9.exe+00DB27DC,0x28,0x1364");
                var stagevalue = await Stages.GetStage(stage_get);

                string[] stage = stagevalue.Split(':');

                var placeholders = new Dictionary<string, object>
                    {
                        { "chapter", stage[0] },
                        { "room", stage[1] },
                        { "chris_health", chris_health },
                        { "sheva_health", sheva_health }
                    };

                discord.UpdateLargeAsset($"logo", $"Resident Evil 5");
                if (chris_health > 0 || sheva_health > 0)
                {
                    string details = updater.UpdateDetails("Resident Evil 5", placeholders);
                    string state = updater.UpdateState("Resident Evil 5", placeholders);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);
                }
                else
                    discord.UpdateDetails("In Menus");

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