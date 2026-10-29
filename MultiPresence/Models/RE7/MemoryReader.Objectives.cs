namespace MultiPresence.Models.RE7
{
    internal static partial class MemoryReader
    {
        private static readonly (string Name, string Pattern, int Operand, int Length)[] ObjectivePatterns =
        [
            ("Objectives", "C7 83 F0 00 00 00 00 00 00 00 48 8B 46 50 48 8B 15 ?? ?? ?? ?? 48 83 78 18 00", 17, 21),
            ("ObjectRegistry", "40 53 48 83 EC 30 48 8B 05 ?? ?? ?? ?? 48 8B D9 48 8B 88 90 00 00 00 48 8B 43 50 48 83 78 18 00 75 ?? 8B 02 44 0F B7 42 04", 9, 13)
        ];

        private sealed record ObjectiveState(string Primary = "", string Secondary = "", string Current = "", string Path = "", string All = "", string PrimaryId = "", string SecondaryId = "", string CurrentId = "");

        private sealed record ObjectiveNode(ulong Owner, ulong ParentOwner, Guid Id, string Text, int Order, int Index);

        private static ObjectiveState ReadObjectives()
        {
            var memory = new ObjectiveRead();
            var manager = memory.Pointer(addresses["Objectives"]);
            var dictionary = manager == 0 ? 0 : memory.Pointer(manager + 0x58);
            if (dictionary == 0)
            {
                memory.Validate();
                return new ObjectiveState();
            }

            var entries = memory.Pointer(dictionary + 0x18);
            var count = memory.Int(dictionary + 0x20);
            var capacity = entries == 0 ? 0 : memory.Int(entries + 0x1C);
            if (count < 0 || count > 256 || capacity < count || capacity > 1024)
            {
                throw Invalid("invalid objective dictionary");
            }

            var registryRoot = memory.Pointer(addresses["ObjectRegistry"]);
            var registry = registryRoot == 0 ? 0 : memory.Pointer(registryRoot + 0x90);
            var rows = new List<ObjectiveNode>();
            for (var index = 0; index < count; index++)
            {
                var entry = entries + 0x20 + (ulong)index * 0x18;
                if (memory.Int(entry) < 0)
                {
                    continue;
                }

                var control = memory.Pointer(entry + 0x10);
                var owner = control == 0 ? 0 : memory.Pointer(control + 0x10);
                if (owner == 0)
                {
                    continue;
                }

                var id = memory.Guid(control + 0x50);
                var parentGuid = memory.Guid(control + 0x68);
                var parent = ResolveObjectiveParent(memory, registry, parentGuid);
                var text = GameText.Get(memory.Guid(control + 0x78));
                var order = parent == 0 ? memory.Int(control + 0x60) : 0;
                rows.Add(new ObjectiveNode(owner, parent, id, text, order, index));
            }

            var ordered = rows.OrderBy(row => row.Order).ThenBy(row => row.Index).ToArray();
            var flattened = new List<ObjectiveNode>();
            var firstPath = new List<ObjectiveNode>();
            foreach (var root in ordered.Where(row => row.ParentOwner == 0))
            {
                VisitObjective(root, ordered, [], flattened, firstPath);
            }

            memory.Validate();
            var visiblePath = firstPath.Where(row => row.Text.Length != 0).ToArray();
            if (visiblePath.Length == 0)
            {
                return new ObjectiveState(All: string.Join(" | ", flattened.Where(row => row.Text.Length != 0).Select(row => row.Text).Distinct()));
            }

            var primary = visiblePath[0];
            var secondary = visiblePath.Length > 1 ? visiblePath[1] : null;
            var current = visiblePath[^1];
            return new ObjectiveState(
                primary.Text,
                secondary?.Text ?? "",
                current.Text,
                string.Join(" / ", visiblePath.Select(row => row.Text)),
                string.Join(" | ", flattened.Where(row => row.Text.Length != 0).Select(row => row.Text).Distinct()),
                primary.Id.ToString(),
                secondary?.Id.ToString() ?? "",
                current.Id.ToString());
        }

        private static void VisitObjective(ObjectiveNode node, ObjectiveNode[] rows, List<ObjectiveNode> path, List<ObjectiveNode> flattened, List<ObjectiveNode> firstPath)
        {
            if (path.Count >= 16 || path.Any(row => row.Owner == node.Owner))
            {
                throw Invalid("invalid objective hierarchy");
            }

            path.Add(node);
            flattened.Add(node);
            var children = rows.Where(row => row.ParentOwner == node.Owner).ToArray();
            if (children.Length == 0 && firstPath.Count == 0 && path.Any(row => row.Text.Length != 0))
            {
                firstPath.AddRange(path);
            }

            foreach (var child in children)
            {
                VisitObjective(child, rows, path, flattened, firstPath);
            }

            path.RemoveAt(path.Count - 1);
        }

        private static ulong ResolveObjectiveParent(ObjectiveRead memory, ulong registry, Guid id)
        {
            if (id == Guid.Empty)
            {
                return 0;
            }

            if (registry == 0)
            {
                throw Invalid("objective registry unavailable");
            }

            var bytes = id.ToByteArray();
            for (var offset = 0; offset < bytes.Length; offset += 2)
            {
                var slot = BitConverter.ToUInt16(bytes, offset);
                if (slot == 0)
                {
                    continue;
                }

                var entry = registry + 0x28 + (ulong)slot * 0x18;
                if (memory.Guid(entry + 8) == id)
                {
                    var owner = memory.Pointer(entry);
                    return owner != 0 && memory.Pointer(owner + 0x18) != 0 ? owner : 0;
                }
            }

            var overflow = memory.Pointer(registry + 0x180028);
            var count = memory.Int(registry + 0x180034);
            if (count < 0 || count > 65536 || (count != 0 && overflow == 0))
            {
                throw Invalid("invalid objective registry overflow");
            }

            for (var index = 0; index < count; index++)
            {
                var entry = overflow + (ulong)index * 0x18;
                if (memory.Guid(entry + 8) == id)
                {
                    var owner = memory.Pointer(entry);
                    return owner != 0 && memory.Pointer(owner + 0x18) != 0 ? owner : 0;
                }
            }

            return 0;
        }

        private sealed class ObjectiveRead
        {
            private readonly Dictionary<ulong, ulong> pointers = new Dictionary<ulong, ulong>();
            private readonly Dictionary<ulong, int> integers = new Dictionary<ulong, int>();
            private readonly Dictionary<ulong, Guid> guids = new Dictionary<ulong, Guid>();

            public ulong Pointer(ulong address)
            {
                if (!pointers.TryGetValue(address, out var value))
                {
                    value = MemoryReader.Pointer(address);
                    pointers.Add(address, value);
                }

                return value;
            }

            public int Int(ulong address)
            {
                if (!integers.TryGetValue(address, out var value))
                {
                    value = ReadInt(address);
                    integers.Add(address, value);
                }

                return value;
            }

            public Guid Guid(ulong address)
            {
                if (!guids.TryGetValue(address, out var value))
                {
                    value = ReadGuid(address);
                    guids.Add(address, value);
                }

                return value;
            }

            public void Validate()
            {
                if (pointers.Any(entry => MemoryReader.Pointer(entry.Key) != entry.Value) ||
                    integers.Any(entry => ReadInt(entry.Key) != entry.Value) ||
                    guids.Any(entry => ReadGuid(entry.Key) != entry.Value))
                {
                    throw Invalid("objectives changed during read");
                }
            }
        }
    }
}
