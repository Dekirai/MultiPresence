using System.Numerics;

namespace MultiPresence.Models.FFVIIR
{
    internal static class LocationReader
    {
        private readonly record struct ArrayHeader(ulong Address, ulong Data, int Count, int Capacity);
        private readonly record struct ActorList(ArrayHeader Header, byte[] Entries);

        internal static bool TryRead(ulong world, ulong pawn, string locationId, out int layer, out int group)
        {
            layer = -1;
            group = -1;
            if (!MemoryReader.IsA(world, "World") || !MemoryReader.IsA(pawn, "EndCharacter") || Locations.GetName(locationId).Length == 0)
            {
                return false;
            }

            var rootAddress = MemoryReader.Field(pawn, "RootComponent", 8);
            var root = MemoryReader.Pointer(rootAddress);
            if (!MemoryReader.IsA(root, "SceneComponent"))
            {
                return false;
            }

            var point = ReadPoint(root + 0x1B0);
            var levels = ReadArray(MemoryReader.Field(world, "Levels", 16), 4096);
            var levelEntries = ReadEntries(levels, 8);
            var actors = new List<ActorList>();
            var types = new Dictionary<ulong, bool>();
            var seen = new HashSet<ulong>();
            int selectedLayer = -1, selectedGroup = -1;
            for (var n = 0; n < levels.Count; n++)
            {
                var level = BitConverter.ToUInt64(levelEntries, n * 8);
                if (level == 0)
                {
                    continue;
                }

                if (!MemoryReader.IsA(level, "Level") || MemoryReader.Field(level, "OwningWorld", 8) != level + 0xC0 || MemoryReader.Pointer(level + 0xC0) != world)
                {
                    return false;
                }

                var array = ReadArray(level + 0xA0, 65536);
                var entries = ReadEntries(array, 8);
                actors.Add(new ActorList(array, entries));
                for (var i = 0; i < array.Count; i++)
                {
                    var actor = BitConverter.ToUInt64(entries, i * 8);
                    if (actor == 0 || !seen.Add(actor))
                    {
                        continue;
                    }

                    var type = MemoryReader.Pointer(actor + 0x10);
                    if (!types.TryGetValue(type, out var isVolume))
                    {
                        isVolume = MemoryReader.IsA(actor, "EndNaviMapVolume");
                        types[type] = isVolume;
                    }

                    if (!isVolume)
                    {
                        continue;
                    }

                    if (MemoryReader.Pointer(actor + 0x20) != level)
                    {
                        return false;
                    }

                    var candidateLayer = MemoryReader.ReadInt(MemoryReader.Field(actor, "LayerNumber", 4));
                    var candidateGroup = MemoryReader.ReadInt(MemoryReader.Field(actor, "OpenGroupNumber", 4));
                    if (candidateLayer < 0 || candidateGroup < 0 || candidateLayer > 1024 || candidateGroup > 65536)
                    {
                        return false;
                    }

                    if (!Contains(actor, point, out var contains))
                    {
                        return false;
                    }

                    if (!contains)
                    {
                        continue;
                    }

                    if (Locations.GetArea(locationId, candidateLayer, candidateGroup).Length == 0 && Locations.GetLayer(locationId, candidateLayer).Length == 0)
                    {
                        return false;
                    }

                    if (selectedLayer >= 0 && (selectedLayer != candidateLayer || selectedGroup != candidateGroup))
                    {
                        return false;
                    }

                    selectedLayer = candidateLayer;
                    selectedGroup = candidateGroup;
                }
            }

            if (selectedLayer < 0 || MemoryReader.Pointer(rootAddress) != root || !Unchanged(levels, levelEntries, 8))
            {
                return false;
            }

            foreach (var array in actors)
            {
                if (!Unchanged(array.Header, array.Entries, 8))
                {
                    return false;
                }
            }

            layer = selectedLayer;
            group = selectedGroup;
            return true;
        }

        private static bool Contains(ulong volume, Vector3 point, out bool contains)
        {
            contains = false;
            var brushAddress = MemoryReader.Field(volume, "Brush", 8);
            var rootAddress = MemoryReader.Field(volume, "RootComponent", 8);
            var brush = MemoryReader.Pointer(brushAddress);
            var root = MemoryReader.Pointer(rootAddress);
            if (!MemoryReader.IsA(brush, "Model") || !MemoryReader.IsA(root, "BrushComponent"))
            {
                return false;
            }

            var transform = Hypervisor.ReadArray(root + 0x1A0, 48, true);
            var rotation = new Quaternion(BitConverter.ToSingle(transform, 0), BitConverter.ToSingle(transform, 4), BitConverter.ToSingle(transform, 8), BitConverter.ToSingle(transform, 12));
            var translation = Point(transform, 16);
            var scale = Point(transform, 32);
            if (!float.IsFinite(rotation.LengthSquared()) || Math.Abs(rotation.LengthSquared() - 1) > 0.001 || Math.Abs(scale.X) < 0.0001 || Math.Abs(scale.Y) < 0.0001 || Math.Abs(scale.Z) < 0.0001)
            {
                return false;
            }

            var local = Vector3.Transform(point - translation, Quaternion.Conjugate(rotation)) / scale;
            if (!Finite(local))
            {
                return false;
            }

            var vertices = ReadArray(brush + 0x58, 65536);
            if (vertices.Count < 4)
            {
                return false;
            }

            var vertexData = ReadEntries(vertices, 12);
            var min = new Vector3(float.PositiveInfinity);
            var max = new Vector3(float.NegativeInfinity);
            for (var i = 0; i < vertices.Count; i++)
            {
                var vertex = Point(vertexData, i * 12);
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }

            if (local.X < min.X - 0.01 || local.Y < min.Y - 0.01 || local.Z < min.Z - 0.01 || local.X > max.X + 0.01 || local.Y > max.Y + 0.01 || local.Z > max.Z + 0.01)
            {
                return true;
            }

            var nodes = ReadArray(brush + 0x28, 4096);
            if (nodes.Count < 4 || (long)nodes.Count * vertices.Count > 1000000)
            {
                return false;
            }

            var nodeData = ReadEntries(nodes, 64);
            var inside = true;
            for (var i = 0; i < nodes.Count; i++)
            {
                var normal = Point(nodeData, i * 64);
                var distance = BitConverter.ToSingle(nodeData, i * 64 + 12);
                if (!float.IsFinite(distance) || Math.Abs(normal.LengthSquared() - 1) > 0.001)
                {
                    return false;
                }

                for (var v = 0; v < vertices.Count; v++)
                {
                    if (Vector3.Dot(normal, Point(vertexData, v * 12)) - distance > 0.25)
                    {
                        return false;
                    }
                }

                if (Vector3.Dot(normal, local) - distance > 0.01)
                {
                    inside = false;
                }
            }

            if (MemoryReader.Pointer(brushAddress) != brush || MemoryReader.Pointer(rootAddress) != root || !transform.AsSpan().SequenceEqual(Hypervisor.ReadArray(root + 0x1A0, 48, true)) || !Unchanged(vertices, vertexData, 12) || !Unchanged(nodes, nodeData, 64))
            {
                return false;
            }

            contains = inside;
            return true;
        }

        private static ArrayHeader ReadArray(ulong address, int limit)
        {
            var header = Hypervisor.ReadArray(address, 16, true);
            var data = BitConverter.ToUInt64(header, 0);
            var count = BitConverter.ToInt32(header, 8);
            var capacity = BitConverter.ToInt32(header, 12);
            if (count < 0 || count > capacity || capacity > limit || count > 0 && data == 0)
            {
                throw new InvalidDataException("FFVIIR: invalid location array.");
            }

            return new ArrayHeader(address, data, count, capacity);
        }

        private static byte[] ReadEntries(ArrayHeader array, int stride)
        {
            return array.Count == 0 ? Array.Empty<byte>() : Hypervisor.ReadArray(array.Data, checked(array.Count * stride), true);
        }

        private static bool Unchanged(ArrayHeader array, byte[] entries, int stride)
        {
            return MemoryReader.Pointer(array.Address) == array.Data && MemoryReader.ReadInt(array.Address + 8) == array.Count && MemoryReader.ReadInt(array.Address + 12) == array.Capacity && entries.AsSpan().SequenceEqual(ReadEntries(array, stride));
        }

        private static Vector3 ReadPoint(ulong address)
        {
            return Point(Hypervisor.ReadArray(address, 12, true), 0);
        }

        private static Vector3 Point(byte[] data, int offset)
        {
            var point = new Vector3(BitConverter.ToSingle(data, offset), BitConverter.ToSingle(data, offset + 4), BitConverter.ToSingle(data, offset + 8));
            if (!Finite(point))
            {
                throw new InvalidDataException("FFVIIR: invalid location geometry.");
            }

            return point;
        }

        private static bool Finite(Vector3 point)
        {
            return float.IsFinite(point.X) && float.IsFinite(point.Y) && float.IsFinite(point.Z) && Math.Abs(point.X) < 100000000 && Math.Abs(point.Y) < 100000000 && Math.Abs(point.Z) < 100000000;
        }
    }
}
