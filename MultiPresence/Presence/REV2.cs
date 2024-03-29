﻿using System.Diagnostics;
using DiscordRPC;
using Memory;
using MultiPresence.Models.REV2;

namespace MultiPresence.Presence
{
    public class REV2
    {
        static Mem mem = new Mem();
        static string process = "rerev2";
        private static DiscordRpcClient discord;
        static int mission = 0;
        public static async void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1213180163446149121");
            InitializeDiscord();
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
                int raid_character_get = mem.ReadByte("rerev2.exe+117ED54,0x4A58");
                int raid_chapter_get = mem.ReadByte("rerev2.exe+117ED54,0x30");
                int raid_character_level = mem.ReadByte("rerev2.exe+117ED54,0x4A59");
                int raid_money = mem.ReadInt("rerev2.exe+117D120,0xBA08");
                int stage_get = mem.Read2Byte("rerev2.exe+115AACC");

                var stage = await Stages.GetStage(stage_get);
                var chapter = await Chapters.GetChapter(raid_chapter_get);

                var raid_character = await Characters.GetCharacter(raid_character_get);

                if (stage[0] == "Raid Mode")
                {
                    if (stage[1] == "In Lobby")
                    {
                        mission = mem.ReadByte("rerev2.exe+011DE690,0x1E0,0x4C,0x3C,0x14,0x3C,0x74,0x7C4") + 1;
                        discord.UpdateLargeAsset($"logo", $"Resident Evil Revelations 2");
                        discord.UpdateDetails($"Raid Mode: In Lobby");
                        discord.UpdateState($"{raid_character} (Lv. {raid_character_level})");
                    }
                    else
                    {
                        discord.UpdateLargeAsset($"logo", $"Resident Evil Revelations 2");
                        discord.UpdateDetails($"Raid Mode: Mission {chapter}-0{mission}");
                        discord.UpdateState($"{raid_character} (Lv. {raid_character_level})");
                    }
                }
                else
                {
                    discord.UpdateLargeAsset($"logo", $"Resident Evil Revelations 2");
                    discord.UpdateDetails($"");
                    discord.UpdateState($"");
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