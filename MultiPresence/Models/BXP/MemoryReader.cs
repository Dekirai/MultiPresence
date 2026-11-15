namespace MultiPresence.Models.BXP
{
    internal static partial class MemoryReader
    {
        private static readonly Dictionary<string, ulong> addresses = new();
        private static int processId;
        private static DateTime nextScan;

        public static void Reset()
        {
            addresses.Clear();
            processId = 0;
            nextScan = DateTime.MinValue;
        }

        public static GameSnapshot ReadSnapshot()
        {
            ResolveAddresses();
            var loading = new GameSnapshot { State = "Loading" };
            ulong scene = Singleton("SceneMgr");
            if (scene == 0 || Value<byte>(scene + 0x50) != 0) return loading;
            int current = Value<int>(scene + 0x34);
            if (current == 1) return new GameSnapshot();
            if (current is not (2 or 3)) return loading;

            ulong save = Singleton("SaveMgr");
            if (save == 0) return loading;
            ulong meta = Ptr(save + 0x68);
            var snapshot = ReadFarm(new GameSnapshot(), meta);
            if (!snapshot.Gold.HasValue) return loading;
            if (current == 2)
            {
                snapshot = snapshot with { State = "Farm", Location = "New Ballbylon" };
            }
            else
            {
                ulong battle = Ptr(save + 0x60);
                if (battle == 0 || ClassName(battle) != "BattleSaveData") return loading;
                snapshot = ReadRun(snapshot with { State = "Ingame" }, battle);
                snapshot = ReadCharacterProgress(snapshot, meta, battle);
                if (Ptr(save + 0x60) != battle) return loading;
            }

            if (Singleton("SceneMgr") != scene || Value<byte>(scene + 0x50) != 0 ||
                Value<int>(scene + 0x34) != current || Singleton("SaveMgr") != save || Ptr(save + 0x68) != meta) return loading;
            return snapshot;
        }

        private static T Value<T>(ulong address) where T : struct => Hypervisor.Read<T>(address, true);
        private static ulong Ptr(ulong address) => Value<ulong>(address);

        private static string ClassName(ulong obj)
        {
            ulong klass = Ptr(obj);
            return klass == 0 ? "" : Hypervisor.ReadString(Ptr(klass + 0x10), 64, true);
        }

        private static ulong Singleton(string name)
        {
            ulong klass = Ptr(addresses[name]);
            // IL2CPP leaves a tagged metadata index here until the class is initialized.
            if (klass < 0x10000 || (klass & 7) != 0) return 0;
            ulong fields = Ptr(klass + 0xB8);
            ulong instance = fields == 0 ? 0 : Ptr(fields);
            if (instance == 0 || Ptr(instance) != klass || ClassName(instance) != name || Ptr(instance + 0x10) == 0) return 0;
            return instance;
        }
    }
}
