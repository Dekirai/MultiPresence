namespace MultiPresence.Models.RE0
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
        private static readonly string[] ScenePrefixes = { "tr", "sh", "dm", "ch", "bt", "pl" };
        private static int processId, moduleSize;
        private static ulong moduleBase;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            processId = moduleSize = 0;
            moduleBase = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            ulong gameplay = Pointer(addresses["Gameplay"]);
            if (gameplay == 0)
            {
                return new GameSnapshot() { State = "Default" };
            }
            var player = ReadPlayer();
            if (player.Player == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            ulong game = Pointer(addresses["GameInfo"]);
            ulong scenes = Pointer(addresses["SceneInfo"]);
            ulong screen = Pointer(addresses["Screen"]);
            ulong scene = scenes == 0 ? 0 : Pointer(scenes + 0x20);
            ulong texture = screen == 0 ? 0 : Pointer(screen + 0x78);
            if (game == 0 || scene == 0 || texture == 0)
            {
                return new GameSnapshot() { State = "Loading" };
            }

            int difficulty = ReadInt(game + 0x34);
            int roomId = ReadInt(scene + 8);
            int renderedRoomId = ReadInt(scene + 0xC);
            int stage = ReadInt(texture + 0x30);
            int room = ReadInt(texture + 0x34);
            int cut = ReadInt(texture + 0x38);
            if (difficulty is < 0 or > 2 || stage < 0 || stage >= ScenePrefixes.Length || room is < 0 or > 255 || cut is < 0 or > 255)
            {
                throw Invalid("invalid game context");
            }

            string currentScene = ScenePrefixes[stage] + room.ToString("x2");
            if (Locations.GetScene(renderedRoomId) != currentScene)
            {
                return new GameSnapshot() { State = "Loading" };
            }
            string region = Locations.GetArea(currentScene);
            string floor = Locations.GetFloor(currentScene, cut);
            string roomName = Locations.GetRoom(roomId);
            string location = region;
            if (roomName.Length != 0)
            {
                location += " / " + roomName + (floor.Length == 0 ? "" : " (" + floor + ")");
            }
            else if (floor.Length != 0)
            {
                location += " / " + floor;
            }
            if (Pointer(addresses["Gameplay"]) != gameplay || Pointer(addresses["GameInfo"]) != game || Pointer(addresses["SceneInfo"]) != scenes ||
                Pointer(addresses["Screen"]) != screen || Pointer(scenes + 0x20) != scene || Pointer(screen + 0x78) != texture ||
                ReadInt(scene + 8) != roomId || ReadInt(scene + 0xC) != renderedRoomId || ReadInt(texture + 0x30) != stage || ReadInt(texture + 0x34) != room ||
                ReadInt(texture + 0x38) != cut || Pointer(addresses["Player"]) != player.Manager || Pointer(player.Manager + 0x2C) != player.Player)
            {
                throw Invalid("scene changed during read");
            }

            return new GameSnapshot()
            {
                State = "Ingame",
                Character = player.Character,
                CharacterId = player.CharacterId,
                Difficulty = difficulty switch { 0 => "Easy", 1 => "Normal", 2 => "Hard", _ => "" },
                DifficultyId = difficulty,
                Location = location,
                Region = region,
                Floor = floor,
                Room = roomName,
                RoomId = roomId,
                Weapon = player.Weapon,
                WeaponId = player.WeaponId,
                Ammo = player.Ammo,
                ReserveAmmo = player.ReserveAmmo,
                InfiniteAmmo = player.InfiniteAmmo,
                Health = player.Health
            };
        }

        private static InvalidDataException Invalid(string value)
        {
            return new InvalidDataException("RE0: " + value + ".");
        }

        private static ulong Pointer(ulong address)
        {
            uint value = ReadUInt(address);
            if (value != 0 && value < 0x10000)
            {
                throw Invalid("invalid pointer");
            }
            return value;
        }

        private static int ReadInt(ulong address) => Hypervisor.Read<int>(address, true);
        private static uint ReadUInt(ulong address) => Hypervisor.Read<uint>(address, true);
        private static byte ReadByte(ulong address) => Hypervisor.Read<byte>(address, true);
        private static byte[] Bytes(ulong address, int count) => Hypervisor.ReadArray(address, count, true);
    }
}
