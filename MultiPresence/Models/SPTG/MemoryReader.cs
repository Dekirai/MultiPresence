using System.Text;

namespace MultiPresence.Models.SPTG
{
    internal static class MemoryReader
    {
        private static readonly Dictionary<ulong, string> typeNames = new Dictionary<ulong, string>();
        private static int processId, moduleSize;
        private static ulong moduleBase, managerSlot;
        private static DateTime nextScan;

        public static void Reset()
        {
            typeNames.Clear();
            processId = moduleSize = 0;
            moduleBase = managerSlot = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            for (var attempt = 0; ; attempt++)
            {
                try
                {
                    var frame = new FrameRead();
                    var snapshot = ReadFrame(frame);
                    frame.Validate();
                    return snapshot;
                }
                catch (SnapshotChangedException) when (attempt < 2)
                {
                }
            }
        }

        private static GameSnapshot ReadFrame(FrameRead frame)
        {
            var manager = frame.Pointer(managerSlot);
            if (manager == 0)
            {
                return new GameSnapshot() { State = "Default" };
            }

            var count = frame.Pointer(manager + 0x10);
            var entries = frame.Pointer(manager + 0x18);
            if (count > 128)
            {
                throw Invalid("bucket count");
            }

            ulong bucket = 0;
            for (ulong index = 0; index < count; index++)
            {
                var entry = entries + index * 16;
                if (frame.UInt(entry) == 0x07B73C94)
                {
                    bucket = frame.Pointer(entry + 8);
                    break;
                }
            }

            var objects = FindObjects(frame, bucket, 1);
            ulong catalog = 0, difficulty = 0, worldMap = 0;
            foreach (var instance in objects)
            {
                if (instance == 0 || (UInt(instance + 0x38) & 4) == 0)
                {
                    continue;
                }

                switch (TypeName(instance))
                {
                    case ".?AVControllablePlayerCatalog@SPTG@@":
                        catalog = instance;
                        break;
                    case ".?AVGameDifficultyAccessor@SPTG@@":
                        difficulty = instance;
                        break;
                    case ".?AVWorldMapInformationBroker@SPTG@@":
                        worldMap = instance;
                        break;
                }
            }

            if (catalog == 0)
            {
                return new GameSnapshot() { State = "Default" };
            }

            frame.UInt(catalog + 0x38);
            var players = frame.UInt(catalog + 0x88);
            var playerArray = frame.Pointer(catalog + 0x98);
            if (players == 0)
            {
                return new GameSnapshot() { State = "Default" };
            }

            if (players > 4 || playerArray == 0)
            {
                throw Invalid("player catalog");
            }

            ulong player = 0;
            var playerIndex = int.MaxValue;
            var character = "";
            for (uint index = 0; index < players; index++)
            {
                var candidate = frame.Pointer(playerArray + index * 8);
                if (candidate == 0 || (frame.UInt(candidate + 0x38) & 4) == 0)
                {
                    continue;
                }

                var name = GetCharacter(TypeName(candidate));
                var slot = Int(candidate + 0x1510);
                if (name.Length == 0 || slot < 0 || slot >= playerIndex)
                {
                    continue;
                }

                player = candidate;
                playerIndex = slot;
                character = name;
            }

            if (player == 0)
            {
                return new GameSnapshot();
            }

            if (difficulty == 0 || worldMap == 0)
            {
                throw Invalid("game context");
            }

            var characterId = frame.UInt(player + 0x1468);
            var difficultyId = (int)frame.UInt(difficulty + 0x58);
            var stageId = frame.UInt(worldMap + 0xB8);
            var experience = Number(player + 0x13F4);
            var money = Number(player + 0x13F0);
            var lives = Int(player + 0x878);
            if (characterId > 6 || lives is < 0 or > 9999 || money < 0 || experience < 0)
            {
                throw Invalid("player values");
            }

            return new GameSnapshot()
            {
                State = "Ingame",
                Character = character,
                CharacterId = (int)characterId,
                Level = ReadLevel(frame, player, experience),
                Stage = GetStage(stageId),
                StageId = stageId,
                Difficulty = GetDifficulty(difficultyId),
                DifficultyId = difficultyId,
                Health = Number(player + 0x178),
                GutPoints = Number(player + 0x334),
                Defense = Number(player + 0x868),
                Speed = Number(player + 0x86C),
                Willpower = Number(player + 0x870),
                Strength = Number(player + 0x874),
                Lives = lives,
                Money = Math.Floor((double)money * 100) / 100,
                TotalExperience = experience
            };
        }

        private static ulong[] FindObjects(FrameRead frame, ulong bucket, uint key)
        {
            if (bucket == 0)
            {
                return Array.Empty<ulong>();
            }

            var capacity = frame.UInt(bucket + 0x498);
            var heads = frame.Pointer(bucket + 0x4A0);
            if (capacity == 0 || heads == 0)
            {
                return Array.Empty<ulong>();
            }

            if (capacity > 65536)
            {
                throw Invalid("bucket capacity");
            }

            var node = frame.Pointer(heads + key % capacity * 8);
            var visited = new HashSet<ulong>();
            while (node != 0)
            {
                if (!visited.Add(node) || visited.Count > 16384)
                {
                    throw Invalid("bucket collision chain");
                }

                if (frame.UInt(node) == key)
                {
                    var count = frame.Pointer(node + 0x18);
                    var array = frame.Pointer(node + 0x28);
                    if (count > 4096)
                    {
                        throw Invalid("component count");
                    }

                    var result = new ulong[(int)count];
                    for (var index = 0; index < result.Length; index++)
                    {
                        result[index] = frame.Pointer(array + (ulong)index * 8);
                    }

                    return result;
                }

                node = frame.Pointer(node + 0x30);
            }

            return Array.Empty<ulong>();
        }

        private static int ReadLevel(FrameRead frame, ulong player, float experience)
        {
            var handle = frame.Pointer(player + 0x7E0);
            var info = handle == 0 ? 0 : frame.Pointer(handle + 8);
            if (info == 0 || TypeName(info) != ".?AVPlayerInfo@SPTG@@")
            {
                throw Invalid("player level info");
            }

            var start = frame.Pointer(info + 0x78);
            var end = frame.Pointer(info + 0x80);
            var multiplier = Number(info + 0x90);
            if (end <= start || end - start > 400 || ((end - start) & 3) != 0 || multiplier <= 0)
            {
                throw Invalid("level thresholds");
            }

            var count = (int)((end - start) / 4);
            float total = 0;
            for (var level = 1; level < count; level++)
            {
                var threshold = Number(start + (ulong)level * 4);
                if (threshold < 0)
                {
                    throw Invalid("negative level threshold");
                }

                total += threshold * multiplier;
                if (total > experience)
                {
                    return level;
                }
            }

            return count;
        }

        private static string TypeName(ulong instance)
        {
            var table = Pointer(instance);
            if (table < moduleBase + 8 || table >= moduleBase + (ulong)moduleSize)
            {
                throw Invalid("object type");
            }

            if (typeNames.TryGetValue(table, out var name))
            {
                return name;
            }

            var locator = Pointer(table - 8);
            if (locator < moduleBase || locator + 16 > moduleBase + (ulong)moduleSize)
            {
                throw Invalid("type locator");
            }

            var descriptor = UInt(locator + 12);
            if (descriptor > moduleSize - 144)
            {
                throw Invalid("type descriptor");
            }

            var bytes = Bytes(moduleBase + descriptor + 16, 128);
            var end = Array.IndexOf(bytes, (byte)0);
            name = Encoding.ASCII.GetString(bytes, 0, end < 0 ? bytes.Length : end);
            typeNames[table] = name;
            return name;
        }

        private static void ResolveAddresses()
        {
            var process = Hypervisor.Process ?? throw Invalid("process");
            var module = process.MainModule ?? throw Invalid("module");
            var address = (ulong)module.BaseAddress.ToInt64();
            if (processId != process.Id || moduleBase != address || moduleSize != module.ModuleMemorySize)
            {
                Reset();
                processId = process.Id;
                moduleBase = address;
                moduleSize = module.ModuleMemorySize;
            }

            if (managerSlot != 0)
            {
                return;
            }

            if (DateTime.UtcNow < nextScan)
            {
                throw Invalid("waiting to retry pattern scan");
            }

            nextScan = DateTime.UtcNow.AddSeconds(30);
            var header = Bytes(moduleBase, 4096);
            var pe = BitConverter.ToInt32(header, 0x3C);
            if (BitConverter.ToUInt16(header) != 0x5A4D || pe < 0x40 || pe > 0xE00 || BitConverter.ToUInt32(header, pe) != 0x4550)
            {
                throw Invalid("PE header");
            }

            var sections = BitConverter.ToUInt16(header, pe + 6);
            var table = pe + 24 + BitConverter.ToUInt16(header, pe + 20);
            if (sections is 0 or > 96 || table + sections * 40 > header.Length)
            {
                throw Invalid("PE sections");
            }

            var targets = new HashSet<ulong>();
            var length = PatternCatalog.BucketManager.Split(' ').Length;
            const int chunkSize = 1024 * 1024;
            for (var section = 0; section < sections; section++)
            {
                var entry = table + section * 40;
                var name = Encoding.ASCII.GetString(header, entry, 8).TrimEnd('\0');
                if ((BitConverter.ToUInt32(header, entry + 36) & 0x20000000) == 0 || name is not ".xtext" and not ".text")
                {
                    continue;
                }

                var size = BitConverter.ToUInt32(header, entry + 8);
                var rva = BitConverter.ToUInt32(header, entry + 12);
                if (rva >= moduleSize || size > moduleSize - rva)
                {
                    throw Invalid("code section range");
                }

                for (ulong offset = 0; offset < size; offset += chunkSize)
                {
                    var start = moduleBase + rva + offset;
                    var bytes = Bytes(start, (int)Math.Min((ulong)(chunkSize + length - 1), size - offset));
                    var cursor = 0;
                    while (cursor <= bytes.Length - length)
                    {
                        var found = Hypervisor.FindPattern(bytes.AsSpan(cursor), PatternCatalog.BucketManager);
                        if (found < 0)
                        {
                            break;
                        }

                        cursor += found;
                        var match = start + (ulong)cursor;
                        var target = checked((ulong)((long)match + 7 + BitConverter.ToInt32(bytes, cursor + 3)));
                        if (target < moduleBase || target + 8 > moduleBase + (ulong)moduleSize)
                        {
                            throw Invalid("pattern target");
                        }

                        targets.Add(target);
                        cursor++;
                    }
                }
            }

            if (targets.Count != 1)
            {
                throw Invalid("missing or ambiguous bucket manager pattern");
            }

            managerSlot = targets.Single();
        }

        private static string GetCharacter(string type)
        {
            return type switch
            {
                ".?AVControllableScott@SPTG@@" => "Scott Pilgrim",
                ".?AVControllableRamona@SPTG@@" => "Ramona Flowers",
                ".?AVControllableKim@SPTG@@" => "Kim Pine",
                ".?AVControllableStephen@SPTG@@" => "Stephen Stills",
                ".?AVControllableNegaScott@SPTG@@" => "NegaScott",
                ".?AVControllableKnives@SPTG@@" => "Knives Chau",
                ".?AVControllableWallace@SPTG@@" => "Wallace Wells",
                _ => ""
            };
        }

        private static string GetStage(uint id)
        {
            return Stages.GetName(id);
        }

        private static string GetDifficulty(int id)
        {
            return id switch
            {
                1 => "Average Joe",
                2 => "Rough & Tough",
                3 => "Supreme Master",
                _ => ""
            };
        }

        private static byte[] Bytes(ulong address, int count)
        {
            if (count < 0 || count > 2 * 1024 * 1024 || address < 0x10000 || address > 0x7FFFFFFFFFFFUL - (ulong)count)
            {
                throw Invalid("memory range");
            }

            return count == 0 ? Array.Empty<byte>() : Hypervisor.ReadArray(address, count, true);
        }

        private static ulong Pointer(ulong address)
        {
            return BitConverter.ToUInt64(Bytes(address, 8));
        }

        private static uint UInt(ulong address)
        {
            return BitConverter.ToUInt32(Bytes(address, 4));
        }

        private static int Int(ulong address)
        {
            return BitConverter.ToInt32(Bytes(address, 4));
        }

        private static float Number(ulong address)
        {
            var value = BitConverter.ToSingle(Bytes(address, 4));
            return float.IsFinite(value) && value is >= -10000 and <= 100000000 ? value : throw Invalid("numeric value");
        }

        private static InvalidDataException Invalid(string field)
        {
            return new InvalidDataException("SPTG: invalid or changing " + field + ".");
        }

        private sealed class FrameRead
        {
            private readonly Dictionary<(ulong Address, int Count), byte[]> reads = new Dictionary<(ulong Address, int Count), byte[]>();

            private byte[] Watch(ulong address, int count)
            {
                var key = (address, count);
                if (!reads.TryGetValue(key, out var value))
                {
                    reads[key] = value = Bytes(address, count);
                }

                return value;
            }

            public ulong Pointer(ulong address)
            {
                return BitConverter.ToUInt64(Watch(address, 8));
            }

            public uint UInt(ulong address)
            {
                return BitConverter.ToUInt32(Watch(address, 4));
            }

            public void Validate()
            {
                foreach (var (range, expected) in reads)
                {
                    if (!Bytes(range.Address, range.Count).AsSpan().SequenceEqual(expected))
                    {
                        throw new SnapshotChangedException();
                    }
                }
            }
        }

        private sealed class SnapshotChangedException : IOException
        {
            public SnapshotChangedException() : base("SPTG: game state changed during read.")
            {
            }
        }
    }
}
