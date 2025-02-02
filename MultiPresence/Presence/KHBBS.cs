﻿using DiscordRPC;
using MultiPresence.Models.KHBBS;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class KHBBS
    {
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("839545395368820806");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX")[0];
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
            Process[] game = Process.GetProcessesByName("KINGDOM HEARTS Birth by Sleep FINAL MIX");
            if (game.Length > 0)
            {
                int world_get = Hypervisor.Read<byte>(0x818120);
                int room_get = Hypervisor.Read<byte>(0x818121);
                int difficulty_get = Hypervisor.Read<byte>(0x10FA0881);
                int character_get = Hypervisor.Read<byte>(0x10F9EE4C);
                int level = Hypervisor.Read<byte>(0x10F9EEE1);

                var world = await Worlds.GetWorld(world_get);
                var difficulty = await Difficulties.GetDifficulty(difficulty_get);
                var character = await Characters.GetCharacter(character_get);
                var room = await Rooms.GetRoom(world[0]);

                try
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholders);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Kingdom Hearts Birth by Sleep Final Mix", placeholders);
                }
                catch
                {
                    discord.UpdateState($"{room[0]}");
                    discord.UpdateDetails("");
                    discord.UpdateSmallAsset($"");
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

        private static async Task<Dictionary<string, object>> GeneratePlaceholders()
        {
            int world_get = Hypervisor.Read<byte>(0x818120);
            int room_get = Hypervisor.Read<byte>(0x818121);
            int difficulty_get = Hypervisor.Read<byte>(0x10FA0881);
            int character_get = Hypervisor.Read<byte>(0x10F9EE4C);
            int level = Hypervisor.Read<byte>(0x10F9EEE1);

            var world = await Worlds.GetWorld(world_get);
            var difficulty = await Difficulties.GetDifficulty(difficulty_get);
            var character = await Characters.GetCharacter(character_get);
            var room = await Rooms.GetRoom(world[0]);

            return new Dictionary<string, object>
            {
                { "level", level },
                { "room", room[room_get] },
                { "world", world[0] },
                { "world_icon_name", world[1] },
                { "difficulty", difficulty },
                { "character", character },
                { "character_icon_name", character.ToLower() }
            };
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
