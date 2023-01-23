using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.MMXCM;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using static System.Reflection.Metadata.BlobBuilder;

namespace MultiPresence.Presence
{
    public class MMXCM
    {
        Mem mem = new Mem();
        string process = "pcsx2-qtx64-avx2";
        private static DiscordRpcClient discord;

        [DllImport("GetProcAddressEx.dll")]
        private static extern IntPtr GetEEMem();

        public async void DoAction()
        {
            await Task.Delay(3000);
            GetPID();
            discord = new DiscordRpcClient("1067143901061853306");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
        }

        private async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                var title = Process.GetProcessesByName("pcsx2-qtx64-avx2").FirstOrDefault();

                if (title.MainWindowTitle.Contains("Mega Man X - Command Mission"))
                {
                    GetPID();
                    string eemem = $"0x{GetEEMem():X}";

                    var region = mem.ReadString($"{eemem}+53CC42");
                    if (region != "SLUS-20903XXX")
                    {
                        // PAL Region
                        int zenny = mem.ReadInt($"{eemem}+55E54C");
                        discord.UpdateLargeAsset("https://dekirai.crygod.de/rpc/multipresence/mmxcm/logo.jpg", $"Mega Man X: Command Mission");
                        discord.UpdateDetails($"Zenny: {zenny}");
                    }
                    else
                    {
                        // US Region
                        int zenny = mem.ReadInt($"{eemem}+54FCCC");
                        discord.UpdateLargeAsset("https://dekirai.crygod.de/rpc/multipresence/mmxcm/logo.jpg", $"Mega Man X: Command Mission");
                        discord.UpdateDetails($"Zenny: {zenny}");
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