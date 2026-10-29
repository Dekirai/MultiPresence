using DiscordRPC;
using MultiPresence.Infrastructure;
using MultiPresence.Models.PHAS;
using MultiPresence.Runtime;
using System.Text;

namespace MultiPresence.Presence
{
    public class PHAS
    {
        private const string GameName = "Phasmophobia";
        private static readonly Dictionary<string, (string Pattern, int Operand, int Length)> Patterns = new Dictionary<string, (string Pattern, int Operand, int Length)>
        {
            { "SeasonalEventType", ("48 8B 0D ?? ?? ?? ?? 48 8B 81 B8 00 00 00 48 83 38 00 0F 85 ?? ?? ?? ?? 48 89 5C 24 30 48 89 74 24 38 48 89 7C 24 20 E8 E4 48 00 FE", 3, 7) },
            { "LevelStatsType", ("48 8B 0D ?? ?? ?? ?? 48 89 5C 24 30 48 89 7C 24 20 48 8B 81 B8 00 00 00 48 83 38 00 0F 85 ?? ?? ?? ?? E8 FD 96 09 FE", 3, 7) },
            { "SeasonalEventAuthLayout", ("33 C9 E8 D7 BD F6 FF 48 85 C0 0F 84 ?? ?? ?? ?? 48 8B 88 D0 00 00 00 48 85 C9 0F 84 ?? ?? ?? ?? 33 D2 E8 B7 0E EC FF 84 C0 0F 84 ?? ?? ?? ??", 0, 0) },
            { "AuthStateLayout", ("83 79 28 02 0F 94 C0 C3", 0, 0) },
            { "SeasonalEventWriteLayout", ("E8 A5 B7 D3 01 F6 D8 1B DB 83 E3 05 89 5F 20", 0, 0) },
            { "MainManagerType", ("48 8B 05 ?? ?? ?? ?? 83 B8 E0 00 00 00 00 75 ?? 48 8B C8 E8 A7 F2 8A FF 48 8B 05 ?? ?? ?? ?? 48 8B 80 B8 00 00 00 48 8B D3 48 89 18 48 8B 0D ?? ?? ?? ?? 48 8B 89 B8 00 00 00 E8 D0 2E 8D FF", 3, 7) },
            { "PhotonType", ("48 8B 05 ?? ?? ?? ?? 83 B8 E0 00 00 00 00 75 ?? 48 8B C8 E8 88 4F 02 FC 48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 0F B6 41 38 48 83 C4 28 C3", 3, 7) },
            { "LevelValuesType", ("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 19 48 8B 0D ?? ?? ?? ?? 83 B9 E0 00 00 00 00 75 ?? E8 78 C1 06 FE 45 33 C0 33 D2 48 8B CB", 3, 7) },
            { "GameControllerType", ("48 8B 05 ?? ?? ?? ?? 48 8B 90 B8 00 00 00 48 89 1A 48 8B D3 48 8B 0D ?? ?? ?? ?? 48 8B 89 B8 00 00 00 E8 48 71 10 FE", 3, 7) },
            { "LevelControllerType", ("48 8B 05 ?? ?? ?? ?? 48 8B D6 48 89 5C 24 58 48 89 7C 24 68 48 8B 88 B8 00 00 00 48 89 31 48 8B 0D ?? ?? ?? ?? 48 8B 89 B8 00 00 00 E8 E2 A4 0C FE 48 8B 56 70 48 8D 4E 30 48 89 56 30 E8 D1 A4 0C FE", 3, 7) },
            { "PhotonCurrentRoomLayout", ("48 8B 81 B8 00 00 00 80 78 38 00 75 ?? 83 B9 E0 00 00 00 00 75 ?? E8 59 5B 02 FC 48 8B 0D ?? ?? ?? ?? 48 8B 81 B8 00 00 00", 0, 0) },
            { "PhotonOnlineRoomLayout", ("48 8B 81 B8 00 00 00 48 8B 40 08 48 85 C0 74 ?? 48 8B 80 08 01 00 00 48 83 C4 28 C3", 0, 0) },
            { "PhotonOfflineRoomLayout", ("83 B9 E0 00 00 00 00 75 ?? E8 FE 5A 02 FC 48 8B 0D ?? ?? ?? ?? 48 8B 81 B8 00 00 00 48 8B 40 40 48 83 C4 28 C3", 0, 0) },
            { "PhotonClientStateLayout", ("83 B9 E0 00 00 00 00 75 ?? E8 FE 50 02 FC 48 8B 0D ?? ?? ?? ?? 48 8B 81 B8 00 00 00 48 8B 48 08 48 85 C9 74 ?? 8B 81 90 00 00 00 48 83 C4 28 C3", 0, 0) },
            { "PhotonInRoomLayout", ("48 8B 0D ?? ?? ?? ?? 83 B9 E0 00 00 00 00 75 ?? E8 8B 59 02 FC 33 C9 E8 D4 07 00 00 83 F8 09 0F 94 C0 48 83 C4 28 C3", 0, 0) },
            { "PhotonLoadingLayout", ("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 C6 41 54 00 48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 C6 81 D1 00 00 00 01 48 8B 0D ?? ?? ?? ?? E8 42 7C 05 FC", 0, 0) },
            { "RoomPlayerCountLayout", ("48 83 7B 70 00 74 ?? 48 8B 15 ?? ?? ?? ?? 48 8B 4B 70 48 83 C4 20 5B E9 ?? ?? ?? ??", 0, 0) },
            { "DictionaryCountLayout", ("8B 41 20 2B 41 28 C3", 0, 0) },
            { "CurrentDifficultyLayout", ("48 8B D9 33 C9 E8 70 05 A9 01 48 85 C0 74 ?? 48 8B 50 38 45 33 C0 48 8B CB 48 83 C4 20 5B E9 ?? ?? ?? ??", 0, 0) },
            { "MapLayout", ("48 8B 0D ?? ?? ?? ?? 48 8B 7B 48 83 B9 E0 00 00 00 00 75 ?? E8 FA 30 07 FE 45 33 C0 33 D2 48 8B CF E8 FD 75 EE 02 84 C0", 0, 0) },
            { "DifficultyCustomLayout", ("83 79 18 06 74 ?? 83 79 18 07 74 ?? 83 79 18 08 0F 94 C0 C3 B0 01 C3", 0, 0) },
            { "Network", ("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 19 48 8B 0D ?? ?? ?? ?? 83 B9 E0 00 00 00 00 75 ?? E8 28 C8 03 FE 45 33 C0 33 D2 48 8B CB E8 2B 0D EB 02 84 C0 75 ?? 48 8B 05 ?? ?? ?? ?? 48 8B 80 B8 00 00 00 48 8B 00", 3, 7) },
            { "NetworkPlayerLayout", ("40 53 48 83 EC 20 80 3D ?? ?? ?? ?? 00 48 8B D9 75 ?? 48 8D 0D ?? ?? ?? ?? E8 52 0F 06 FE C6 05 ?? ?? ?? ?? 01 48 8B 53 28 48 85 D2 75 ??", 0, 0) },
            { "NetworkLocalPlayerLayout", ("40 53 48 83 EC 20 80 3D ?? ?? ?? ?? 00 48 8B D9 75 ?? 48 8D 0D ?? ?? ?? ?? E8 32 0E 06 FE C6 05 ?? ?? ?? ?? 01 48 8B 53 30 48 85 D2 75 ??", 0, 0) },
            { "PlayerRoomWriteLayout", ("48 8D 4B 50 48 89 7B 50 48 8B D7 E8 57 D9 AF FF 33 D2 48 8B CF E8 6D 8F 00 00 45 33 C0 48 8B D0 48 8B CF E8 1F B8 94 04", 0, 0) },
            { "CCTVController", ("48 8B 05 ?? ?? ?? ?? 48 8B 88 B8 00 00 00 48 8B 09 48 85 C9 74 ?? 0F B6 56 20 45 33 C0 E8 CD FD 05 01", 3, 7) },
            { "LocalTruckFlagLayout", ("88 91 A0 00 00 00 84 D2 75 07 33 D2 E9 3F 1D 00 00 33 D2 E9 E8 1B 00 00", 0, 0) }
        };
        private static Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static DiscordRpcClient? discord;
        private static DiscordStatusUpdater? updater;
        private static int cachedProcessId, cachedModuleSize;
        private static ulong cachedModuleBase;
        private static DateTime nextScan;
        public static Task DoAction()
        {
            ResetAddresses();
            GetPID();
            discord = new DiscordRpcClient("1562392149192871977");
            InitializeDiscord();
            updater = new DiscordStatusUpdater("Assets/Config/Phasmophobia.json");
            PresenceRuntime.Start(nameof(PHAS), GameName, RPC);
            return Task.CompletedTask;
        }

        private static void GetPID()
        {
            ProcessMonitor.TryAttach(GameName);
        }

        private static async Task RPC()
        {
            if (discord == null || updater == null)
            {
                throw new InvalidOperationException("PHAS: presence is not initialized.");
            }

            if (ProcessMonitor.IsRunning(GameName))
            {
                try
                {
                    if (!Hypervisor.IsAttached && !ProcessMonitor.TryAttach(GameName))
                    {
                        return;
                    }

                    var snapshot = ReadSnapshot();
                    if (snapshot.State is not "Ingame" and not "Lobby")
                    {
                        ShowStatus(snapshot.State == "Menu" ? "In Main Menu" : "Loading / Waiting for game data");
                        return;
                    }

                    var placeholders = await PlaceholderHelper.GetPlaceholders(() => GeneratePlaceholders(snapshot));
                    PlaceholderHelper.UpdateDiscordStatus(discord, updater, GameName, placeholders, snapshot.State);
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException)
                {
                    AppLog.Warning("Phasmophobia memory is not ready.", exception);
                    ShowStatus("Waiting for game data");
                }
            }
            else
            {
                ResetAddresses();
                discord.Deinitialize();
                updater.Dispose();
                PresenceRuntime.RequestDetection();
            }
        }

        private static Task<Dictionary<string, object>> GeneratePlaceholders(Snapshot snapshot)
        {
            if (snapshot.State is not "Ingame" and not "Lobby")
            {
                return Task.FromResult(new Dictionary<string, object>());
            }

            var mode = snapshot.Offline ? "Singleplayer" : "Multiplayer";
            var result = new Dictionary<string, object>
            {
                { "mode", mode },
                { "session", snapshot.Offline ? mode : $"{mode} ({snapshot.PlayerCount}/{snapshot.MaxPlayers})"},
                { "playercount", snapshot.PlayerCount },
                { "maxplayers", snapshot.MaxPlayers },
                { "map", snapshot.Map },
                { "mapid", snapshot.State == "Ingame" ? snapshot.MapId : "" },
                { "scene", snapshot.Scene },
                { "difficulty", snapshot.Difficulty },
                { "difficultyid", snapshot.State == "Ingame" ? snapshot.DifficultyId : "" },
                { "room", snapshot.Room },
                { "location", snapshot.Map.Length == 0 ? "" : $"{snapshot.Map} / {snapshot.Room}"}
            };
            return Task.FromResult(result);
        }

        private static void ShowStatus(string text)
        {
            if (discord == null)
            {
                throw new InvalidOperationException("PHAS: presence is not initialized.");
            }

            discord.SetPresence(new RichPresence()
            {
                Details = text,
                State = "",
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = GameName
                },
                Timestamps = PlaceholderHelper._startTimestamp
            });
        }

        private sealed record Snapshot(string State, bool Offline = false, int PlayerCount = 0, int MaxPlayers = 0, string Map = "", int MapId = 0, string Scene = "", string Difficulty = "", int DifficultyId = 0, string Room = "");
        private sealed record UnityObject(ulong Address, ulong Class, ulong Native);
        private sealed record Singleton(ulong Statics, UnityObject? Object);
        private sealed record ManagedObject(ulong Address, ulong Class);
        private sealed record ManagedSingleton(ulong Statics, ManagedObject? Object);
        private sealed record SeasonalEventData(ManagedSingleton Controller, ManagedSingleton Stats, ManagedObject? Status, int StatusValue, int CachedValue, int Value);
        private sealed record Session(ulong Statics, bool Offline, bool Loading, ulong Client, int ClientState, ulong Room, ulong Players, int Count, int FreeCount, int Version, int MaxPlayers);
        private sealed record Contract(Singleton Level, Singleton Game, Singleton Values, Singleton Network, Singleton Cctv, UnityObject Map, UnityObject Difficulty, UnityObject Player, UnityObject Local, UnityObject Room, ulong Rooms, ulong RoomCount, ulong Outside, int RoomIndex, bool InTruck, int MapId, int LevelType, bool Restricted, string MapName, string Scene, int DifficultyId, string DifficultyKey, string RoomName, SeasonalEventData SeasonalEvent);
        private sealed record Frame(Session? Session, Singleton? Level, Singleton? Game, Singleton? Lobby, Contract? Contract, Snapshot Snapshot);
        private static Snapshot ReadSnapshot()
        {
            ResolveAddresses();
            var first = ReadFrame();
            if (first != ReadFrame())
            {
                throw new InvalidDataException("Phasmophobia: state changed during read.");
            }

            return first.Snapshot;
        }

        private static Frame ReadFrame()
        {
            var session = ReadSession();
            if (session == null || session.Room == 0 || session.Loading)
            {
                return new Frame(session, null, null, null, null, new(session?.Loading == true ? "Loading" : "Menu"));
            }

            var level = ReadSingleton("LevelControllerType", "LevelController");
            var game = ReadSingleton("GameControllerType", "GameController");
            if (level.Object == null && game.Object == null)
            {
                var lobby = ReadSingleton("MainManagerType", "MainManager");
                var state = lobby.Object == null ? "Loading" : "Lobby";
                return new Frame(session, level, game, lobby, null, new(state, session.Offline, session.Count - session.FreeCount, session.MaxPlayers));
            }

            if (level.Object == null || game.Object == null || Pointer(level.Object.Address + 0x88) != game.Object.Address || Pointer(game.Object.Address + 0xF0) != level.Object.Address)
            {
                return new Frame(session, level, game, null, null, new("Loading"));
            }

            var contract = ReadContract(level, game);
            if (contract == null)
            {
                return new Frame(session, level, game, null, null, new("Loading"));
            }

            var baseMap = GameText.GetMap(contract.LevelType, contract.Restricted);
            var eventMap = GameText.GetMap(contract.LevelType, contract.Restricted, contract.SeasonalEvent.Value);
            var map = eventMap != baseMap ? eventMap : contract.MapName;
            if (map.Length == 0)
            {
                map = baseMap;
            }

            var room = contract.InTruck ? "Truck" : GameText.GetRoom(contract.RoomName);
            if (room.Length == 0)
            {
                return new Frame(session, level, game, null, contract, new("Loading"));
            }

            return new Frame(session, level, game, null, contract, new("Ingame", session.Offline, session.Count - session.FreeCount, session.MaxPlayers, map, contract.MapId, contract.Scene, GameText.GetDifficulty(contract.DifficultyId, contract.DifficultyKey), contract.DifficultyId, room));
        }

        private static Session? ReadSession()
        {
            var statics = ReadStatics("PhotonType", "PhotonNetwork", "Photon.Pun");
            if (statics == 0)
            {
                return null;
            }

            var offline = ReadBool(statics + 0x38);
            var loading = ReadBool(statics + 0xD1);
            var client = Pointer(statics + 0x08);
            var clientState = 0;
            ulong room;
            if (offline)
            {
                room = Pointer(statics + 0x40);
            }
            else
            {
                if (client != 0)
                {
                    RequireClass(client, "LoadBalancingClient", "Photon.Realtime");
                }

                clientState = client == 0 ? 0 : ReadInt(client + 0x90);
                room = clientState == 9 ? Pointer(client + 0x108) : 0;
            }

            if (loading || room == 0)
            {
                return new Session(statics, offline, loading, client, clientState, room, 0, 0, 0, 0, 0);
            }

            RequireClass(room, "Room", "Photon.Realtime");
            if (ReadBool(room + 0x68) != offline)
            {
                throw new InvalidDataException("Phasmophobia: room mode changed.");
            }

            var players = Pointer(room + 0x70);
            if (players == 0)
            {
                throw new InvalidDataException("Phasmophobia: missing room players.");
            }

            RequireClass(players, "Dictionary`2", "System.Collections.Generic");
            var count = ReadInt(players + 0x20);
            var free = ReadInt(players + 0x28);
            var version = ReadInt(players + 0x2C);
            var max = Hypervisor.Read<byte>(room + 0x20, true);
            ValidatePlayerCount(count, free, max);
            return new Session(statics, offline, loading, client, clientState, room, players, count, free, version, max);
        }

        private static void ValidatePlayerCount(int count, int free, int max)
        {
            if (count is < 0 or > 16 || free < 0 || free > count || max is < 1 or > 4 || count - free < 1 || count - free > max)
            {
                throw new InvalidDataException("Phasmophobia: invalid player count.");
            }
        }

        private static Contract? ReadContract(Singleton level, Singleton game)
        {
            var values = ReadSingleton("LevelValuesType", "LevelValues");
            var network = ReadSingleton("Network", "Network");
            var cctv = ReadSingleton("CCTVController", "CCTVController");
            if (level.Object == null || game.Object == null || values.Object == null || network.Object == null || cctv.Object == null)
            {
                return null;
            }

            var map = ReadUnityObject(Pointer(values.Object.Address + 0x48), "Map");
            var difficulty = ReadUnityObject(Pointer(values.Object.Address + 0x38), "Difficulty");
            var player = ReadUnityObject(Pointer(network.Object.Address + 0x28), "PCPlayer", "VRPlayer");
            var local = ReadUnityObject(Pointer(network.Object.Address + 0x30), "LocalPCPlayer", "LocalVRPlayer");
            if (map == null || difficulty == null || player == null || local == null || Pointer(player.Address + 0x110) != local.Address || Pointer(local.Address + 0x20) != player.Address)
            {
                return null;
            }

            var room = ReadUnityObject(Pointer(player.Address + 0x50), "LevelRoom");
            if (room == null)
            {
                return null;
            }

            var rooms = Pointer(level.Object.Address + 0x50);
            var outside = Pointer(level.Object.Address + 0x70);
            var roomCount = rooms == 0 ? 0 : Pointer(rooms + 0x18);
            if (roomCount > 1024)
            {
                throw new InvalidDataException("Phasmophobia: invalid room array.");
            }

            var index = -1;
            for (ulong n = 0; n < roomCount; n++)
            {
                if (Pointer(rooms + 0x20 + n * 8) == room.Address)
                {
                    index = (int)n;
                    break;
                }
            }

            if (outside != room.Address && index < 0)
            {
                return null;
            }

            var inTruck = ReadBool(cctv.Object.Address + 0xA0);
            return new Contract(level, game, values, network, cctv, map, difficulty, player, local, room, rooms, roomCount, outside, index, inTruck, ReadInt(map.Address + 0x68), ReadInt(map.Address + 0x18), ReadBool(map.Address + 0x20), ReadManagedString(Pointer(map.Address + 0x28)), ReadManagedString(Pointer(map.Address + 0x58)), ReadInt(difficulty.Address + 0x18), ReadManagedString(Pointer(difficulty.Address + 0x20)), ReadManagedString(Pointer(room.Address + 0x70)), ReadSeasonalEvent());
        }

        private static SeasonalEventData ReadSeasonalEvent()
        {
            var controller = ReadManagedSingleton("SeasonalEventType", "SeasonalEventController");
            var stats = ReadManagedSingleton("LevelStatsType", "LevelStats");
            var status = stats.Object == null ? null : ReadManagedObject(Pointer(stats.Object.Address + 0xD0), "ഹദലഭഥഭയറഭ");
            var statusValue = status == null ? 0 : ReadInt(status.Address + 0x28);
            var cachedValue = controller.Object == null ? 0 : ReadInt(controller.Object.Address + 0x20);
            if (statusValue is < 0 or > 3 || cachedValue is < 0 or > 5)
            {
                throw new InvalidDataException("Phasmophobia: invalid seasonal event state.");
            }

            return new SeasonalEventData(controller, stats, status, statusValue, cachedValue, statusValue == 2 ? cachedValue : 0);
        }

        private static ManagedSingleton ReadManagedSingleton(string key, string expectedClass)
        {
            var statics = ReadStatics(key, expectedClass);
            return new ManagedSingleton(statics, statics == 0 ? null : ReadManagedObject(Pointer(statics), expectedClass));
        }

        private static ManagedObject? ReadManagedObject(ulong address, string expectedClass)
        {
            if (address == 0)
            {
                return null;
            }

            var klass = Pointer(address);
            RequireClassInfo(klass, expectedClass, "");
            return new ManagedObject(address, klass);
        }

        private static Singleton ReadSingleton(string key, string expectedClass)
        {
            var statics = ReadStatics(key, expectedClass);
            return new Singleton(statics, statics == 0 ? null : ReadUnityObject(Pointer(statics), expectedClass));
        }

        private static ulong ReadStatics(string key, string expectedClass, string expectedNamespace = "")
        {
            var klass = Pointer(addresses[key]);
            if (klass == 0)
            {
                return 0;
            }

            RequireClassInfo(klass, expectedClass, expectedNamespace);
            return Pointer(klass + 0xB8);
        }

        private static UnityObject? ReadUnityObject(ulong address, params string[] classes)
        {
            if (address == 0)
            {
                return null;
            }

            var klass = Pointer(address);
            if (klass == 0 || !classes.Contains(ReadCString(Pointer(klass + 0x10)), StringComparer.Ordinal))
            {
                throw new InvalidDataException("Phasmophobia: unexpected Unity object class.");
            }

            var native = Pointer(address + 0x10);
            return native == 0 ? null : new(address, klass, native);
        }

        private static void RequireClass(ulong obj, string expected, string ns)
        {
            RequireClassInfo(Pointer(obj), expected, ns);
        }

        private static void RequireClassInfo(ulong klass, string expected, string ns)
        {
            if (klass == 0 || ReadCString(Pointer(klass + 0x10)) != expected || ReadCString(Pointer(klass + 0x18)) != ns)
            {
                throw new InvalidDataException("Phasmophobia: unexpected managed class.");
            }
        }

        private static string ReadCString(ulong pointer)
        {
            if (pointer == 0)
            {
                return "";
            }

            var bytes = new List<byte>();
            for (ulong offset = 0; offset < 256; offset++)
            {
                var value = Hypervisor.Read<byte>(pointer + offset, true);
                if (value == 0)
                {
                    return Encoding.UTF8.GetString(bytes.ToArray());
                }

                bytes.Add(value);
            }

            throw new InvalidDataException("Phasmophobia: unterminated class name.");
        }

        private static string ReadManagedString(ulong pointer)
        {
            if (pointer == 0)
            {
                return "";
            }

            var length = ReadInt(pointer + 0x10);
            if (length is < 0 or > 256)
            {
                throw new InvalidDataException("Phasmophobia: invalid string length.");
            }

            if (length == 0)
            {
                return "";
            }

            var value = Encoding.Unicode.GetString(Hypervisor.ReadArray(pointer + 0x14, length * 2, true));
            if (value.Any(char.IsControl))
            {
                throw new InvalidDataException("Phasmophobia: invalid string characters.");
            }

            return value.Trim();
        }

        private static bool ReadBool(ulong address)
        {
            var value = Hypervisor.Read<byte>(address, true);
            if (value > 1)
            {
                throw new InvalidDataException("Phasmophobia: invalid boolean.");
            }

            return value != 0;
        }

        private static ulong Pointer(ulong address)
        {
            return Hypervisor.Read<ulong>(address, true);
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static void ResetAddresses()
        {
            cachedProcessId = cachedModuleSize = 0;
            cachedModuleBase = 0;
            addresses = new();
            nextScan = DateTime.MinValue;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw new InvalidDataException("PHAS: game is not attached.");
            var module = process.Modules.Cast<System.Diagnostics.ProcessModule>().FirstOrDefault(value => string.Equals(value.ModuleName, "GameAssembly.dll", StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidDataException("Phasmophobia: module not loaded.");
            var moduleBase = (ulong)module.BaseAddress.ToInt64();
            if (cachedProcessId != process.Id || cachedModuleBase != moduleBase || cachedModuleSize != module.ModuleMemorySize)
            {
                ResetAddresses();
                cachedProcessId = process.Id;
                cachedModuleBase = moduleBase;
                cachedModuleSize = module.ModuleMemorySize;
            }

            if (addresses.Count != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw new InvalidDataException("Phasmophobia: waiting to retry patterns.");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var resolved = new Dictionary<string, ulong>();
            foreach (var entry in Patterns)
            {
                var spec = entry.Value;
                var match = FindUniquePattern(moduleBase, module.ModuleMemorySize, spec.Pattern);
                var target = spec.Length == 0 ? match : checked((ulong)((long)match + spec.Length + ReadInt(match + (ulong)spec.Operand)));
                if (target < moduleBase || target + 8 > moduleBase + (ulong)module.ModuleMemorySize)
                {
                    throw new InvalidDataException("Phasmophobia: AOB target is outside the module.");
                }

                resolved.Add(entry.Key, target);
            }

            addresses = resolved;
        }

        private static ulong FindUniquePattern(ulong moduleBase, int moduleSize, string signature)
        {
            if (moduleSize < 64)
            {
                throw new InvalidDataException("Phasmophobia: invalid module size.");
            }

            var peOffset = Hypervisor.Read<uint>(moduleBase + 0x3C, true);
            if (peOffset > (uint)moduleSize - 24)
            {
                throw new InvalidDataException("Phasmophobia: invalid PE offset.");
            }

            var pe = moduleBase + peOffset;
            if (peOffset > (uint)moduleSize - 24 || Hypervisor.Read<uint>(pe, true) != 0x4550 || Hypervisor.Read<ushort>(pe + 4, true) != 0x8664)
            {
                throw new InvalidDataException("Phasmophobia: invalid PE header.");
            }

            var sectionCount = Hypervisor.Read<ushort>(pe + 6, true);
            var optionalSize = Hypervisor.Read<ushort>(pe + 20, true);
            var sectionTable = pe + 24 + optionalSize;
            if (sectionTable + (ulong)sectionCount * 40 > moduleBase + (ulong)moduleSize)
            {
                throw new InvalidDataException("Phasmophobia: invalid section table.");
            }

            var pattern = SignaturePattern.Parse(signature);
            var prefix = pattern.Bytes.TakeWhile(value => value.HasValue).Select(value => value.GetValueOrDefault()).ToArray();
            if (prefix.Length == 0)
            {
                throw new InvalidDataException("Phasmophobia: pattern needs a fixed prefix.");
            }

            const int chunkSize = 1024 * 1024;
            ulong match = 0;
            for (var section = 0; section < sectionCount; section++)
            {
                var header = sectionTable + (ulong)section * 40;
                var size = Hypervisor.Read<uint>(header + 8, true);
                var rva = Hypervisor.Read<uint>(header + 12, true);
                var flags = Hypervisor.Read<uint>(header + 36, true);
                if ((flags & 0x20000000) == 0 || size < pattern.Length)
                {
                    continue;
                }

                if ((ulong)rva + size > (ulong)moduleSize)
                {
                    throw new InvalidDataException("Phasmophobia: invalid executable section.");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var count = (int)Math.Min((ulong)(chunkSize + pattern.Length - 1), size - offset);
                    var bytes = Hypervisor.ReadArray(moduleBase + rva + offset, count, true);
                    var start = 0;
                    while (start <= bytes.Length - pattern.Length)
                    {
                        var found = bytes.AsSpan(start).IndexOf(prefix);
                        if (found < 0)
                        {
                            break;
                        }

                        var candidate = start + found;
                        start = candidate + 1;
                        if (candidate + pattern.Length > bytes.Length)
                        {
                            break;
                        }

                        var matches = true;
                        for (var part = 0; part < pattern.Length; part++)
                        {
                            if (pattern.Bytes[part] is byte expected && bytes[candidate + part] != expected)
                            {
                                matches = false;
                                break;
                            }
                        }

                        if (!matches)
                        {
                            continue;
                        }

                        var address = moduleBase + rva + offset + (ulong)candidate;
                        if (match != 0 && match != address)
                        {
                            throw new InvalidDataException("Phasmophobia: manager pattern is ambiguous.");
                        }

                        match = address;
                    }
                }
            }

            return match != 0 ? match : throw new InvalidDataException("Phasmophobia: manager pattern was not found.");
        }

        private static void InitializeDiscord()
        {
            if (discord == null)
            {
                throw new InvalidOperationException("PHAS: presence is not initialized.");
            }

            discord.Initialize();
        }
    }
}
