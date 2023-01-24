using System.Diagnostics;
using DiscordRPC;
using Memory;
//using Button = DiscordRPC.Button;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Linq;

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
                        int area_get = mem.ReadInt($"{eemem}+55D974");
                        int map_get = mem.ReadInt($"{eemem}+55D97C");
                        int chapter = mem.ReadInt($"{eemem}+55D984");
                        int menu = mem.ReadInt($"{eemem}+55D970");
                        int state = mem.ReadInt($"{eemem}+55D988");
                        int level_x = mem.ReadByte($"{eemem}+55E560");

                        string url = "https://dekirai.crygod.de/rpc/multipresence/mmxcm/Locations.json";

                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                string json = client.DownloadString(url);

                                dynamic jsonData = JsonConvert.DeserializeObject(json);

                                string name = jsonData[area_get.ToString()]["Name"];
                                string area = jsonData[area_get.ToString()]["Areas"][map_get];
                                discord.UpdateLargeAsset("logo", $"Mega Man X: Command Mission");
                                if (state == 8)
                                    discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x} (In Battle)");
                                else
                                    discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x}");
                                discord.UpdateState($"{name}: {area}");
                            }
                        }
                        catch
                        {
                            discord.UpdateLargeAsset("logo", $"Mega Man X: Command Mission");
                            if (state == 8)
                                discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x} (In Battle)");
                            else
                                discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x}");
                            discord.UpdateState("");
                        }
                    }
                    else
                    {
                        // US Region
                        int zenny = mem.ReadInt($"{eemem}+54FCCC");
                        int area_get = mem.ReadInt($"{eemem}+54f0f4");
                        int map_get = mem.ReadInt($"{eemem}+54f0fc");
                        int chapter = mem.ReadInt($"{eemem}+54f104");
                        int menu = mem.ReadInt($"{eemem}+54f0f0");
                        int state = mem.ReadInt($"{eemem}+54f108");
                        int level_x = mem.ReadByte($"{eemem}+54fce0");

                        string url = "https://dekirai.crygod.de/rpc/multipresence/mmxcm/Locations.json";

                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                string json = client.DownloadString(url);

                                dynamic jsonData = JsonConvert.DeserializeObject(json);

                                string name = jsonData[area_get.ToString()]["Name"];
                                string area = jsonData[area_get.ToString()]["Areas"][map_get];
                                discord.UpdateLargeAsset("logo", $"Mega Man X: Command Mission");
                                if (state == 8)
                                    discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x} (In Battle)");
                                else
                                    discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x}");
                                discord.UpdateState($"{name}: {area}");
                            }
                        }
                        catch
                        {
                            discord.UpdateLargeAsset("logo", $"Mega Man X: Command Mission");
                            if (state == 8)
                                discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x} (In Battle)");
                            else
                                discord.UpdateDetails($"[Chapter {chapter}] Lv. {level_x}");
                            discord.UpdateState("");
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
//                Buttons = new Button[]
//                {
//#if DEBUG
//                    new Button() { Label = $"Powered by MultiPresence", Url = "https://github.com/Dekirai/MultiPresence" }
//#endif
//                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}