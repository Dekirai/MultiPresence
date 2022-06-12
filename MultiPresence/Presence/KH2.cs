using System.Diagnostics;
using DiscordRPC;
using KHMemLibrary;
using Button = DiscordRPC.Button;
using MultiPresence.Models.KH2;

namespace MultiPresence.Presence
{
    public class KH2
    {
        KH2FM kh2 = new KH2FM();
        string process = "KINGDOM HEARTS II FINAL MIX";
        private static DiscordRpcClient discord;
        public void DoAction()
        {
            discord = new DiscordRpcClient("826145131152408625");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                int world_get = kh2.ReadByte(0x714DB8);
                int room_get = kh2.ReadByte(0x714DB9);
                int difficulty_get = kh2.ReadByte(0x9A9548);
                int level = kh2.ReadByte(0x9A95AF);
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var room = await Rooms.GetRoom(world[0]);

                discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                
                try
                {
                    discord.UpdateState($"{room[room_get]}");
                    discord.UpdateDetails($"Lv. {level} ({difficulty})");
                }
                catch
                {
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails(null);
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
                    new Button() { Label = $"Open Store Page", Url = "https://www.epicgames.com/store/en-US/p/kingdom-hearts-hd-1-5-2-5-remix" },
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
