using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Memory;
using DiscordRPC;
using KHMemLibrary;
using Button = DiscordRPC.Button;
using System.Net;
using Newtonsoft.Json;
using MultiPresence.Models.KH2;

namespace MultiPresence.Presence
{
    public class KH2
    {
        Mem mem = new Mem();
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
                int _world = kh2.ReadByte(0x714DB8);
                int _room = kh2.ReadByte(0x714DB9);
                int _level = kh2.ReadByte(0x9A95AF);

                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://dekirai.crygod.de/games/kh2/rpc/worlds.json");
                using (StreamReader sr = new StreamReader(stream))
                {
                    var response = sr.ReadToEnd();
                    Worlds result = JsonConvert.DeserializeObject<Worlds>(response);

                    try
                    {
                        var _worldname = result.ReadWorld[_world - 1].Name;
                        var _roomname = result.ReadWorld[_world - 1].Rooms[_room];
                        var _image = result.ReadWorld[_world - 1].Image;
                        discord.UpdateLargeAsset(_image, _worldname);
                        discord.UpdateDetails($"Lv. {_level} ({await kh2.GetDifficultyText()})");
                        discord.UpdateState(_roomname);
                    }
                    catch
                    {
                        discord.UpdateLargeAsset("logo", "Main Menu");
                        discord.UpdateDetails(null);
                        discord.UpdateState("Main Menu");
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
                    new Button() { Label = $"View in Epic Games Store", Url = "https://www.epicgames.com/store/en-US/p/kingdom-hearts-hd-1-5-2-5-remix" }
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}
