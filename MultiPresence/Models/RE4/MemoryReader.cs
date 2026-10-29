using System.Globalization;

namespace MultiPresence.Models.RE4
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new Dictionary<string, ulong>();
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
            var context = ReadContext();
            if (context.State is "Title_Screen" or "Loading")
            {
                return new GameSnapshot() { State = context.State };
            }

            var core = ReadCore(context.Global, context.Player);
            var stage = context.Room >> 8;
            var roomId = context.Room & 255;
            var room = GameText.GetRoom(stage, roomId);
            var area = GameText.GetArea(stage, roomId);
            var location = area.Length == 0 ? room : room.Length == 0 || room == area ? area : area + " / " + room;
            var chapter = context.State is "Main" or "Separate_Ways" ? Chapters.GetChapter(context.Chapter).GetAwaiter().GetResult() : "";
            var score = context.State == "Mercenaries" ? ReadInt(addresses["Score"]) : -1;
            if (score < -1 || score > 100000000)
            {
                throw Invalid("invalid score");
            }

            if (context != ReadContext())
            {
                throw Invalid("scene changed during read");
            }

            return new GameSnapshot()
            {
                State = context.State,
                Mode = context.State switch
                {
                    "Mercenaries" => "The Mercenaries",
                    "Assignment_Ada" => "Assignment Ada",
                    "Separate_Ways" => "Separate Ways",
                    _ => "Main Story"
                },
                CharacterId = context.Character,
                Character = context.Character == 6 ? "Leon and Ashley" : Characters.GetCharacter(context.Character).GetAwaiter().GetResult(),
                DifficultyId = context.Difficulty,
                Difficulty = Difficulties.GetDifficulty(context.Difficulty).GetAwaiter().GetResult(),
                Health = core.Health,
                MaxHealth = core.MaxHealth,
                HealthStatus = core.HealthStatus,
                WeaponId = core.WeaponId,
                Weapon = core.WeaponId < 0 ? "" : core.WeaponId == 0 ? "Unarmed" : Weapons.GetWeapon(core.WeaponId).GetAwaiter().GetResult(),
                Ammo = core.Ammo,
                ReserveAmmo = core.ReserveAmmo,
                InfiniteAmmo = core.InfiniteAmmo,
                Money = context.State is "Main" or "Separate_Ways" ? core.Money : -1,
                Score = score,
                StageId = stage,
                RoomId = roomId,
                ChapterId = context.State is "Main" or "Separate_Ways" ? context.Chapter : -1,
                Chapter = chapter,
                Area = area,
                Room = room,
                Location = location,
                AreaIconName = GameText.GetAreaIcon(stage, roomId)
            };
        }

        private sealed record Context(ulong Global, ulong Player, byte Routine, uint ModeFlags, ushort Room, byte Chapter, byte Character, byte Difficulty, string State);

        private static Context ReadContext()
        {
            var global = Pointer(addresses["Global"]);
            if (global == 0)
            {
                return new Context(0, 0, 0, 0, 0, 0, 0, 0, "Loading");
            }

            var routine = ReadByte(global + 0x20);
            var flags = ReadUInt(global + 0x54) & 0xC0001000;
            var room = ReadUShort(global + 0x4FAC);
            var chapter = ReadByte(global + 0x4F9A);
            var character = ReadByte(global + 0x4FC8);
            var difficulty = ReadByte(global + 0x847C);
            var player = Pointer(addresses["Player"]);
            var state = "Loading";
            if (room == 0x120)
            {
                state = "Title_Screen";
            }
            else if (routine is 3 or 5 or 6 && room is >= 0x100 and < 0x600 && player != 0)
            {
                if (character > 6 || difficulty > 6 || chapter > 24 || Pointer(player) < moduleBase || Pointer(player) >= moduleBase + (ulong)moduleSize)
                {
                    throw Invalid("invalid game context");
                }

                state = (flags & 0x1000) != 0 ? "Separate_Ways" : (flags & 0x80000000) != 0 ? "Assignment_Ada" : (flags & 0x40000000) != 0 ? "Mercenaries" : "Main";
            }

            return new Context(global, player, routine, flags, room, chapter, character, difficulty, state);
        }

        private static InvalidDataException Invalid(string value)
        {
            return new InvalidDataException("RE4: " + value + ".");
        }

        private static ulong Pointer(ulong address)
        {
            var value = ReadUInt(address);
            if (value != 0 && value < 0x10000)
            {
                throw Invalid("invalid pointer");
            }

            return value;
        }

        private static int ReadInt(ulong address)
        {
            return Hypervisor.Read<int>(address, true);
        }

        private static uint ReadUInt(ulong address)
        {
            return Hypervisor.Read<uint>(address, true);
        }

        private static short ReadShort(ulong address)
        {
            return Hypervisor.Read<short>(address, true);
        }

        private static ushort ReadUShort(ulong address)
        {
            return Hypervisor.Read<ushort>(address, true);
        }

        private static byte ReadByte(ulong address)
        {
            return Hypervisor.Read<byte>(address, true);
        }
    }
}
