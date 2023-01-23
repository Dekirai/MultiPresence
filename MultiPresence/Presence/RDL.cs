using System.Diagnostics;
using DiscordRPC;
using Memory;
using Button = DiscordRPC.Button;
using MultiPresence.Models.RDL;
using System.Runtime.InteropServices;

namespace MultiPresence.Presence
{
    public class RDL
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
            discord = new DiscordRpcClient("1067138479814877265");
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
                GetPID();
                string eemem = $"0x{GetEEMem():X}";

                var bolts = mem.ReadInt($"{eemem}+171b40");
                var weapon = await Weapons.GetWeapon(mem.ReadByte($"{eemem}+0x171b67"));
                var difficulty = mem.ReadByte($"{eemem}+21df64");

                discord.UpdateLargeAsset("https://dekirai.crygod.de/rpc/multipresence/rdl/logo.png", $"Ratchet: Deadlocked");
                discord.UpdateDetails($"🔫{weapon} - 🔩{bolts}");
                discord.UpdateState($"Difficulty: Couch Potato");

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