using DiscordRPC;
using MultiPresence.Models.TY;
using MultiPresence.Properties;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class TY
    {
        private static DiscordRpcClient? discord;
        public static string[]? levelvalue = null;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("983292674863943720");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("TY")[0];
                if (_myProcess.Id > 0)
                    Hypervisor.AttachProcess(_myProcess);
            }
            catch
            {
                //nothing?
            }
        }

        private static async void RPC()
        {
            while (true)
            {
                Process[] game = Process.GetProcessesByName("TY");
                if (game.Length > 0)
                {
                    int health = Hypervisor.Read<byte>(0x2737CC);
                    int level = Hypervisor.Read<byte>(0x28903C);
                    int opals = Hypervisor.Read<int>(0x2888B0);

                    if (Settings.Default.langDE == true)
                        levelvalue = await Levels.GetLevelDE(level);
                    else
                        levelvalue = await Levels.GetLevel(level);

                    discord.UpdateLargeAsset($"logo", $"TY the Tasmanian Tiger");
                    if (level == 4 || level == 5 || level == 6 || level == 8 || level == 9 || level == 10 || level == 12 || level == 13 || level == 14)
                        discord.UpdateDetails($"HP: {health} | Opals: {opals}/300");
                    else
                        discord.UpdateDetails($"HP: {health}");
                    discord.UpdateState($"{levelvalue[0]}");

                    await Task.Delay(3000);
                }
                else
                {
                    discord.Deinitialize();
                    MainForm.gameUpdater.Start();
                    break;
                }
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