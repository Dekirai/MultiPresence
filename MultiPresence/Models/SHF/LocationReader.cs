namespace MultiPresence.Models.SHF
{
    internal static partial class MemoryReader
    {
        // WorldCenterPos, HalfRange, Yaw and FloorHeightRange from the game's map area widgets.
        private sealed record MapArea(string Id, string Label, double X, double Y, double HalfX, double HalfY,
            double Yaw = 0, double Bottom = double.NegativeInfinity, double Top = double.PositiveInfinity)
        {
            public bool Contains(double x, double y, double z)
            {
                var (sin, cos) = Math.SinCos(-Yaw * Math.PI / 180);
                double dx = x - X, dy = y - Y;
                return Math.Abs(dx * cos - dy * sin) < HalfX && Math.Abs(dx * sin + dy * cos) < HalfY && z >= Bottom && z < Top;
            }
        }

        private static readonly MapArea[] MapAreas =
        {
            new("FogTown", "Ebisugaoka", -81000, -108000, 34000, 25000),
            new("School1F", "Ebisugaoka Middle School - 1st Floor", -93201, -95162.5, 5000, 4500, 219, 990, 1400),
            new("School2F", "Ebisugaoka Middle School - 2nd Floor", -93201, -95162.5, 5000, 4500, 219, 1400, 1610),
            new("Shimizu1", "Shimizu Residence I", -143764, 81734, 3072, 3276.8),
            new("Shimizu2", "Shimizu Residence II", -133764, 81734, 3072, 3276.8),
            new("Shimizu3", "Shimizu Residence III", -123764, 81734, 3072, 3276.8),
            new("Torii", "Shrine Path", 70000, 100000, 8192, 16384),
            new("OuterCorridors", "Corridors", 69500, 74200, 10649, 10649.6),
            new("WorshipHall", "Worship Hall", 68285.47161, 58601, 7390, 6200),
            new("Underground2F", "Main Hall - 2nd Floor", 69990, 36950, 6800, 13926.4, 0, -150, 1000),
            new("Underground1F", "Main Hall - 1st Floor", 69990, 36950, 6800, 13926.4, 0, -1000, -150),
            new("PotionRoom", "Hermitage of Crimson Water", 72415, 48044, 2048, 2048),
            new("WeddingHall", "Main Hall", 69347, 29627, 2268, 7987)
        };

        private static GameSnapshot ReadLocation(GameSnapshot snapshot, ulong world, ulong controller, ulong pawn)
        {
            ulong root = ObjectProperty(pawn, "RootComponent");
            int offset = Field(root, "RelativeLocation");
            if (offset == 0 || ObjectProperty(root, "AttachParent") != 0) return snapshot;

            var pos = Hypervisor.Read<double>(root + (ulong)offset, 3, true);
            var matches = MapAreas.Where(area => area.Contains(pos[0], pos[1], pos[2])).ToArray();
            if (matches.Length == 0) return snapshot;
            MapArea? area = matches[0];
            if (matches.Length > 1)
            {
                ulong progress = ObjectProperty(ObjectProperty(world, "GameState"), "GameProgress");
                int tagOffset = Field(progress, "ProgressTag");
                string tag = tagOffset == 0 ? "" : FName(progress + (ulong)tagOffset);
                area = matches.FirstOrDefault(match => MatchesStoryArea(match.Id, tag)) ??
                    matches.FirstOrDefault(match => match.Id == "FogTown");
            }
            if (area == null) return snapshot;
            return snapshot with { Location = area.Label, AreaId = area.Id };
        }

        private static bool MatchesStoryArea(string id, string tag) => id switch
        {
            "Torii" => tag.Contains("_Torii.", StringComparison.Ordinal),
            "OuterCorridors" => tag.StartsWith("Progress.Story.Ch04.", StringComparison.Ordinal) || tag.Contains("_OCR.", StringComparison.Ordinal),
            "School1F" or "School2F" => tag.Contains("_School.", StringComparison.Ordinal),
            "WorshipHall" => tag.StartsWith("Progress.Story.Ch06.", StringComparison.Ordinal) || tag.Contains("_WorshipHall.", StringComparison.Ordinal),
            "WeddingHall" => tag.StartsWith("Progress.Story.Ch15.", StringComparison.Ordinal) || tag.Contains("_WeddingRoom.", StringComparison.Ordinal),
            "Underground1F" or "Underground2F" => tag.StartsWith("Progress.Story.Ch08.", StringComparison.Ordinal) && !tag.Contains("_Potion.", StringComparison.Ordinal),
            "PotionRoom" => tag.Contains("_Potion.", StringComparison.Ordinal),
            _ => false
        };
    }
}
