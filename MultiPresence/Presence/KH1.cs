using System.Diagnostics;
using DiscordRPC;
using KHMemLibrary;
using Button = DiscordRPC.Button;
using MultiPresence.Models.KH1;

namespace MultiPresence.Presence
{
    public class KH1
    {
        KH1FM kh1 = new KH1FM();
        string process = "KINGDOM HEARTS FINAL MIX";
        private static DiscordRpcClient discord;
        public void DoAction()
        {
            discord = new DiscordRpcClient("827214883190734889");
            InitializeDiscord();
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private async void RPC()
        {
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                int isGummi = kh1.ReadByte(0x50421D);
                int world_get = kh1.ReadByte(0x233CB4C);
                int room_get = kh1.ReadByte(0x233CB44);
                int difficulty_get = kh1.ReadByte(0x2DFBDFC);
                int level = kh1.ReadByte(0x2DE59D4);
                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var room = await Rooms.GetRoom(world[0]);


                if (room_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"Main Menu");
                    discord.UpdateState($"Main Menu");
                    discord.UpdateDetails(null);
                }
                if (world_get == 255)
                {
                    discord.UpdateLargeAsset($"logo", $"Main Menu");
                    discord.UpdateState($"Main Menu");
                    discord.UpdateDetails(null);
                }
                else
                {
                    if (isGummi == 1)
                    {
                        discord.UpdateLargeAsset($"worldmap", $"Gummi Ship");
                        discord.UpdateState($"Gummi Ship");
                        discord.UpdateDetails($"Lv. {level} ({difficulty[0]})");
                    }
                    else
                    {
                        discord.UpdateLargeAsset($"{world[1]}", $"{world[0]}");
                        discord.UpdateState($"{room[room_get]}");
                        discord.UpdateDetails($"Lv. {level} ({difficulty[0]})");
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
                    new Button() { Label = $"View on EGS", Url = "https://www.epicgames.com/store/en-US/p/kingdom-hearts-hd-1-5-2-5-remix" },
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
