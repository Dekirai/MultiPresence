using DiscordRPC;
using MultiPresence.Models.PYRE;
using System.Diagnostics;

namespace MultiPresence.Presence
{
    public class PYRE
    {

        static string process = "ProjectG";
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        public static void DoAction()
        {
            GetPID();
            discord = new DiscordRpcClient("1226462236181004338");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("config.json");
            Thread thread = new Thread(RPC);
            thread.Start();
        }

        private static void GetPID()
        {
            try
            {
                var _myProcess = Process.GetProcessesByName("ProjectG")[0];
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
            Process[] game = Process.GetProcessesByName(process);
            if (game.Length > 0)
            {
                string nickname = Hypervisor.ReadString(Hypervisor.GetPointer32(0xA7D6A4, [0x5AC]), 16, true);
                var stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10950]), true);
                var level_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA7D6A4, [0x711]), true);
                var mode_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094C]), true);
                var isIngame = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10B27]), true);
                int isOpen = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10934]), true);

                var stage = await Stages.GetStage(stage_get);
                var mode = await Modes.GetMode(mode_get);
                var level = await Levels.GetLevel(level_get);

                if (mode_get == 23 || mode_get == 255 || mode_get == 40)
                {
                    var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersLobby);
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Lobby");
                }
                else
                {
                    if (isIngame == 1)
                    {
                        if (mode_get == 0 || mode_get == 1 || mode_get == 7 || mode_get == 10)
                        {
                            if (isOpen == 1)
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersIngame);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Ingame_Match");
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersIngame);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Ingame_Match_Private");
                            }
                        }
                        else if (mode_get == 4)
                        {
                            if (isOpen == 1)
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersTourney);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Ingame_Tourney");
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersTourney);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Ingame_Tourney_Private");
                            }
                        }
                        else if (mode_get == 2)
                        {
                            if (isOpen == 1)
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersLounge);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Ingame_Lounge");
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersLounge);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Ingame_Lounge_Private");
                            }
                        }
                        else
                        {
                            discord.UpdateDetails($"{nickname} - {level[0]}");
                            discord.UpdateState($"{mode[0]}");
                        }
                    }
                    else
                    {
                        if (mode_get == 2)
                        {
                            var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersLobby);
                            PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Lobby");
                        }
                        else
                        {
                            if (isOpen == 1)
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersWaiting);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Waiting_Room");
                            }
                            else
                            {
                                var placeholders = await PlaceholderHelper.GetPlaceholders(GeneratePlaceholdersWaiting);
                                PlaceholderHelper.UpdateDiscordStatus(discord, updater, "Pangya Reborn", placeholders, "Waiting_Room_Private");
                            }
                        }
                    }
                }


                await Task.Delay(300);
                Thread thread = new Thread(RPC);
                thread.Start();
            }
            else
            {
                discord.Deinitialize();
                MainForm.gameUpdater.Start();
            }
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersLobby()
        {
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer32(0xA7D6A4, [0x5AC]), 16, true);
            var level_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA7D6A4, [0x711]), true);

            var level = await Levels.GetLevel(level_get);

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "level", level[0] },
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersIngame()
        {
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer32(0xA7D6A4, [0x5AC]), 16, true);
            var stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10950]), true);
            var mode_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094C]), true);
            var level_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA7D6A4, [0x711]), true);
            var room = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094D]), true);
            var players = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10938]), true);
            var playersmax = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10937]), true);
            int currenthole = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0xFFEC]), true);
            int maxholes = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094B]), true);

            var stage = await Stages.GetStage(stage_get);
            var mode = await Modes.GetMode(mode_get);
            var level = await Levels.GetLevel(level_get);

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "level", level[0] },
                { "room", room },
                { "map", stage[0] },
                { "map_icon_name", stage_get },
                { "players", players },
                { "playersmax", playersmax },
                { "currenthole", currenthole },
                { "maxholes", maxholes },
                { "mode", mode[0] }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersTourney()
        {
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer32(0xA7D6A4, [0x5AC]), 16, true);
            var stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10950]), true);
            var mode_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094C]), true);
            var level_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA7D6A4, [0x711]), true);
            var room = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094D]), true);
            var players = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10938]), true);
            var playersmax = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10937]), true);
            int currenthole = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0xFFEC]), true);
            int maxholes = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094B]), true);
            var score = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xB006E8, [0x0, 0x40, 0x18, 0x0, 0x14, 0xC8, 0x4F4]), true);

            var stage = await Stages.GetStage(stage_get);
            var mode = await Modes.GetMode(mode_get);
            var level = await Levels.GetLevel(level_get);

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "level", level[0] },
                { "room", room },
                { "map", stage[0] },
                { "map_icon_name", stage_get },
                { "players", players },
                { "playersmax", playersmax },
                { "currenthole", currenthole },
                { "maxholes", maxholes },
                { "score", score },
                { "mode", mode[0] }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersLounge()
        {
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer32(0xA7D6A4, [0x5AC]), 16, true);
            var stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10950]), true);
            var mode_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094C]), true);
            var level_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA7D6A4, [0x711]), true);
            var room = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094D]), true);
            var players = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10938]), true);
            var playersmax = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10937]), true);

            var stage = await Stages.GetStage(stage_get);
            var mode = await Modes.GetMode(mode_get);
            var level = await Levels.GetLevel(level_get);

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "level", level[0] },
                { "room", room },
                { "map", stage[0] },
                { "map_icon_name", stage_get },
                { "players", players },
                { "playersmax", playersmax },
                { "mode", mode[0] }
            };
        }

        private static async Task<Dictionary<string, object>> GeneratePlaceholdersWaiting()
        {
            string nickname = Hypervisor.ReadString(Hypervisor.GetPointer32(0xA7D6A4, [0x5AC]), 16, true);
            var stage_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10950]), true);
            var mode_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094C]), true);
            var level_get = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA7D6A4, [0x711]), true);
            var room = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094D]), true);
            var players = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10938]), true);
            var playersmax = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x10937]), true);
            int maxholes = Hypervisor.Read<byte>(Hypervisor.GetPointer32(0xA47E2C, [0x1094B]), true);

            var stage = await Stages.GetStage(stage_get);
            var mode = await Modes.GetMode(mode_get);
            var level = await Levels.GetLevel(level_get);

            return new Dictionary<string, object>
            {
                { "nickname", nickname },
                { "level", level[0] },
                { "room", room },
                { "map", stage[0] },
                { "map_icon_name", stage_get },
                { "players", players },
                { "playersmax", playersmax },
                { "maxholes", maxholes },
                { "mode", mode[0] }
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