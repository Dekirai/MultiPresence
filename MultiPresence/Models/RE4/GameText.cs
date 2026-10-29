namespace MultiPresence.Models.RE4
{
    public static class GameText
    {
        private static readonly string[][] Rooms = Enumerable.Range(0, 6)
            .Select(stage => Stages.GetStage(stage).GetAwaiter().GetResult())
            .ToArray();

        private static readonly string[][] GermanRooms = Enumerable.Range(0, 6)
            .Select(stage => Stages.GetStageDE(stage).GetAwaiter().GetResult())
            .ToArray();

        public static string GetRoom(int stage, int room, bool german = false)
        {
            string[][] stages = german ? GermanRooms : Rooms;
            if (stage < 0 || stage >= stages.Length || room < 0 || room >= stages[stage].Length)
            {
                return string.Empty;
            }

            string name = stages[stage][room];
            return string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        }

        public static string GetArea(int stage, int room)
        {
            if (room < 0 || room > byte.MaxValue)
            {
                return string.Empty;
            }

            return stage switch
            {
                1 => "Village",
                2 => "Castle",
                3 => "Island",
                4 when room == 0 => "Village",
                4 when room == 2 => "Castle",
                4 when room is 3 or 4 or 5 or 6 or >= 10 and <= 17 => "Island",
                5 when room <= 11 => "Village",
                5 when room <= 16 => "Castle",
                5 when room <= 30 => "Island",
                _ => string.Empty
            };
        }

        public static string GetAreaIcon(int stage, int room)
        {
            string area = GetArea(stage, room);
            return area.Length == 0 ? "logo" : area.ToLowerInvariant();
        }
    }
}
