using System.Numerics;

namespace MultiPresence.Models.SH2
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadLocation(GameSnapshot snapshot, ulong pawn)
        {
            ulong maps = ObjectProperty(pawn, "MapData");
            int field = Field(maps, "MapsVolumes");
            if (field == 0 || ObjectProperty(maps, "OwnerCharacter") != pawn) return snapshot;

            ulong array = maps + (ulong)field;
            int count = Hypervisor.Read<int>(array + 8, true);
            ulong data = Pointer(array);
            if (data == 0 || count is < 1 or > 64) return snapshot;

            var volumes = Hypervisor.Read<ulong>(data, count, true);
            ulong root = ObjectProperty(pawn, "RootComponent");
            if (root == 0) return snapshot;
            var point = ReadMapVector(root + 0x260);
            int priority = -1;
            byte mapId = 0;
            foreach (ulong volume in volumes)
            {
                if (!IsA(volume, "SHGameplayMapVolume")) continue;
                int mapField = Field(volume, "Map");
                if (mapField == 0) return snapshot;
                byte id = Hypervisor.Read<byte>(volume + (ulong)mapField, true);
                if (id == 0 || id == mapId || !ContainsMapPoint(volume, point)) continue;
                int candidatePriority = MapPriority(maps, id);
                if (candidatePriority > priority)
                {
                    priority = candidatePriority;
                    mapId = id;
                }
            }

            var (area, room) = MapLabel(mapId);
            if (area.Length == 0) return snapshot;
            if (mapId == 10) room = EastCityStreet(pawn);
            if (Pointer(array) != data || Hypervisor.Read<int>(array + 8, true) != count ||
                !volumes.AsSpan().SequenceEqual(Hypervisor.Read<ulong>(data, count, true))) return snapshot;
            return snapshot with
            {
                Area = area,
                Room = room,
                AreaId = mapId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Location = room.Length > 0 ? area + " / " + room : area
            };
        }

        private static int MapPriority(ulong maps, byte id)
        {
            int field = Field(maps, "MapsPriorities");
            if (field == 0) return 0;
            ulong map = maps + (ulong)field;
            int count = Hypervisor.Read<int>(map + 8, true);
            int buckets = Hypervisor.Read<int>(map + 0x48, true);
            if (count is < 1 or > 256 || buckets is < 1 or > 512) return 0;
            ulong hash = Pointer(map + 0x40);
            if (hash == 0) hash = map + 0x38;
            int index = Hypervisor.Read<int>(hash + (ulong)((buckets - 1) & id) * 4, true);
            ulong data = Pointer(map);
            for (int i = 0; data != 0 && i < count && index >= 0 && index < count; i++)
            {
                ulong pair = data + (ulong)index * 16;
                if (Hypervisor.Read<byte>(pair, true) == id) return Hypervisor.Read<int>(pair + 4, true);
                index = Hypervisor.Read<int>(pair + 8, true);
            }
            return 0;
        }

        private static bool ContainsMapPoint(ulong volume, Vector3 point)
        {
            ulong brush = ObjectProperty(volume, "BrushComponent");
            ulong body = ObjectProperty(brush, "BrushBodySetup");
            int aggregate = Field(body, "AggGeom");
            if (aggregate == 0 || !MapLocalPoint(brush + 0x240, point, out var brushPoint)) return false;
            ulong hulls = body + (ulong)aggregate + 0x30;
            int count = Hypervisor.Read<int>(hulls + 8, true);
            ulong data = Pointer(hulls);
            if (count is < 1 or > 64 || data == 0) return false;
            for (int i = 0; i < count; i++)
            {
                ulong hull = data + (ulong)i * 0x100;
                if (MapLocalPoint(hull + 0x90, brushPoint, out var local) && ContainsHullPoint(hull, local)) return true;
            }
            return false;
        }

        private static bool ContainsHullPoint(ulong hull, Vector3 point)
        {
            int count = Hypervisor.Read<int>(hull + 0x38, true);
            int indices = Hypervisor.Read<int>(hull + 0x48, true);
            ulong points = Pointer(hull + 0x30), triangles = Pointer(hull + 0x40);
            if (points == 0 || triangles == 0 || count is < 4 or > 1024 || indices is < 12 or > 8192 || indices % 3 != 0) return false;
            var vertices = new Vector3[count];
            var positions = Hypervisor.Read<double>(points, count * 3, true);
            Vector3 center = Vector3.Zero;
            for (int i = 0; i < count; i++)
            {
                vertices[i] = MapVector(positions, 3 * i);
                center += vertices[i];
            }
            center /= count;
            var faces = Hypervisor.Read<int>(triangles, indices, true);
            int valid = 0;
            for (int i = 0; i < faces.Length; i += 3)
            {
                int a = faces[i], b = faces[i + 1], c = faces[i + 2];
                if ((uint)a >= count || (uint)b >= count || (uint)c >= count) return false;
                Vector3 normal = Vector3.Cross(vertices[b] - vertices[a], vertices[c] - vertices[a]);
                float side = Vector3.Dot(normal, center - vertices[a]);
                if (!float.IsFinite(side) || side == 0) continue;
                float distance = Vector3.Dot(normal, point - vertices[a]);
                if (!float.IsFinite(distance) || (side > 0 ? distance < 0 : distance > 0)) return false;
                valid++;
            }
            return valid >= 4;
        }

        private static Vector3 MapVector(double[] values, int offset) =>
            new((float)values[offset], (float)values[offset + 1], (float)values[offset + 2]);

        private static Vector3 ReadMapVector(ulong address) => MapVector(Hypervisor.Read<double>(address, 3, true), 0);

        private static bool MapLocalPoint(ulong transform, Vector3 point, out Vector3 local)
        {
            var t = Hypervisor.Read<double>(transform, 12, true);
            var rotation = new Quaternion((float)t[0], (float)t[1], (float)t[2], (float)t[3]);
            local = Vector3.Transform(point - MapVector(t, 4), Quaternion.Conjugate(rotation)) / MapVector(t, 8);
            return Math.Abs(rotation.LengthSquared() - 1) < 0.001f && float.IsFinite(local.X) && float.IsFinite(local.Y) && float.IsFinite(local.Z);
        }

        private static string EastCityStreet(ulong pawn)
        {
            ulong root = ObjectProperty(pawn, "RootComponent");
            int offset = Field(root, "RelativeLocation");
            if (offset == 0 || ObjectProperty(root, "AttachParent") != 0) return "";
            var position = Hypervisor.Read<double>(root + (ulong)offset, 3, true);
            // Posted segments between the Neely and Lindsay junctions and the eastern end of Sanders St.
            bool onSegment = position[0] is >= -14093 and <= -3605 or >= -2116 and <= 6856;
            if (!onSegment || position[1] is < 47390 or > 48714 || position[2] is < -200 or > 250) return "";

            int based = Field(pawn, "BasedMovement");
            if (based == 0) return "";
            ulong floor = Pointer(pawn + (ulong)based);
            if (IsA(floor, "LandscapeHeightfieldCollisionComponent")) return "Sanders St.";
            string mesh = Name(ObjectProperty(floor, "StaticMesh"));
            return mesh.StartsWith("CurbBase", StringComparison.OrdinalIgnoreCase) ||
                mesh.StartsWith("SideWalk", StringComparison.OrdinalIgnoreCase) || mesh.StartsWith("RoadBase", StringComparison.OrdinalIgnoreCase)
                ? "Sanders St." : "";
        }

        private static (string Area, string Room) MapLabel(byte id) => id switch
        {
            9 => ("Road to Silent Hill", ""),
            10 or 12 => ("Eastern South Vale", ""),
            11 or 13 => ("Western South Vale", ""),
            >= 20 and <= 22 => ("Wood Side Apartments", $"{id - 19}F"),
            >= 30 and <= 32 => ("Brookhaven Hospital", $"{id - 29}F"),
            33 or 53 => ("Brookhaven Hospital", "Basement"),
            34 or 54 => ("Brookhaven Hospital", "Rooftop"),
            >= 40 and <= 42 => ("Lakeview Hotel", $"{id - 39}F"),
            43 or 81 => ("Lakeview Hotel", "Employee Section"),
            44 or 80 => ("Lakeview Hotel", "Basement"),
            45 => ("Lakeview Hotel", "Garden"),
            46 => ("Lakeview Hotel", "Venus Tears Bar"),
            >= 50 and <= 52 => ("Brookhaven Hospital", $"{id - 49}F"),
            >= 60 and <= 63 => ("Blue Creek Apartments", $"{id - 59}F"),
            70 => ("Toluca Prison", "Basement"),
            72 => ("Toluca Prison", "2F"),
            73 => ("Toluca Prison", "3F"),
            74 => ("Toluca Prison", "1F"),
            75 => ("Toluca Prison", "1F Boar Section"),
            76 => ("Toluca Prison", "1F Serpent Section"),
            >= 82 and <= 84 => ("Lakeview Hotel", $"{id - 81}F"),
            90 => ("Labyrinth", "Center"),
            91 => ("Labyrinth", "Rotten Area"),
            92 or 93 => ("Labyrinth", "Desolate Area"),
            94 => ("Labyrinth", "Ruined Area"),
            _ => ("", "")
        };
    }
}
