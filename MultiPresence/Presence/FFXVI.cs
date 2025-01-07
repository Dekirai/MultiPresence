﻿using DiscordRPC;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class FFXVI
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static async void DoAction()
        {
            await Task.Delay(20000);
            GetPID();
            discord = new DiscordRpcClient("1285884197084336161");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ffxvi")[0];
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
            Process[] game = Process.GetProcessesByName("ffxvi");
            if (game.Length > 0)
            {
                int hp = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x1813CE8, [0x50]), true);
                int level = Hypervisor.Read<int>(Hypervisor.GetPointer64(0x1813CE8, [0x40]), true);
                int gil = Hypervisor.Read<int>(0x10A4072E6C);
                int difficulty_get = Hypervisor.Read<byte>(Hypervisor.GetPointer64(0x1813CE8, [0xCB50]), true);
                string difficulty = "";

                if (difficulty_get == 0)
                    difficulty = "Easy";
                else if (difficulty_get == 1)
                    difficulty = "Normal";
                else if (difficulty_get == 2)
                    difficulty = "Final Fantasy";
                else if (difficulty_get == 3)
                    difficulty = "Ultimaniac";

                var placeholders = new Dictionary<string, object>
                {
                    { "level", level },
                    { "hp", hp },
                    { "gil", gil },
                    { "difficulty", difficulty }
                };

                if (hp > 0)
                {
                    string details = updater.UpdateDetails("Final Fantasy XVI", placeholders);
                    string state = updater.UpdateState("Final Fantasy XVI", placeholders);
                    string largeasset = updater.UpdateLargeAsset("Final Fantasy XVI", placeholders);
                    string largeassettext = updater.UpdateLargeAssetText("Final Fantasy XVI", placeholders);
                    string smallasset = updater.UpdateSmallAsset("Final Fantasy XVI", placeholders);
                    string smallassettext = updater.UpdateSmallAssetText("Final Fantasy XVI", placeholders);
                    string button1text = updater.UpdateButton1Text("Final Fantasy XVI", placeholders);
                    string button2text = updater.UpdateButton2Text("Final Fantasy XVI", placeholders);
                    string button1url = updater.UpdateButton1URL("Final Fantasy XVI", placeholders);
                    string button2url = updater.UpdateButton2URL("Final Fantasy XVI", placeholders);
                    discord.UpdateLargeAsset(largeasset, largeassettext);
                    discord.UpdateSmallAsset(smallasset, smallassettext);
                    discord.UpdateDetails(details);
                    discord.UpdateState(state);

                    if (button1url.Length > 0 && button2url.Length == 0)
                    {
                        discord.UpdateButtons(new DiscordRPC.Button[]
                        {
                            new DiscordRPC.Button() { Label = button1text, Url = button1url }
                        });
                    }
                    else if (button1url.Length > 0 && button2url.Length > 0)
                    {
                        discord.UpdateButtons(new DiscordRPC.Button[]
                        {
                            new DiscordRPC.Button() { Label = button1text, Url = button1url },
                            new DiscordRPC.Button() { Label = button2text, Url = button2url }
                        });
                    }
                    else
                    {
                        discord.UpdateButtons(null);
                    }
                }
                else
                {
                    discord.UpdateDetails("In Main Menu");
                    discord.UpdateState("");
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
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow.AddSeconds(1)
                }
            });
        }
    }
}