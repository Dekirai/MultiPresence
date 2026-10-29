namespace MultiPresence.Models.SXSG
{
    internal static class GameText
    {
        public static bool IsKnownStage(string stage)
        {
            return !string.IsNullOrEmpty(stage) && Stages.ContainsKey(stage);
        }

        public static string GetLocation(string stage)
        {
            return Stages.TryGetValue(stage, out var value) ? value.Location : "";
        }

        public static string GetAct(string stage)
        {
            return Stages.TryGetValue(stage, out var value) ? value.Act : "";
        }

        public static string GetMode(string stage)
        {
            return Stages.TryGetValue(stage, out var value) ? value.Mode : "";
        }

        private static readonly Dictionary<string, (string Location, string Act, string Mode)> Stages = new(StringComparer.OrdinalIgnoreCase)
        {
            { "w09a10", ("White Space", "", "White Space") },
            { "w01a11", ("Space Colony ARK", "Act 1", "Act") },
            { "w01a20", ("Space Colony ARK", "Act 2", "Act") },
            { "w02a10", ("Rail Canyon", "Act 1", "Act") },
            { "w02a20", ("Rail Canyon", "Act 2", "Act") },
            { "w03a10", ("Kingdom Valley", "Act 1", "Act") },
            { "w03a20", ("Kingdom Valley", "Act 2", "Act") },
            { "w04a10", ("Sunset Heights", "Act 1", "Act") },
            { "w04a20", ("Sunset Heights", "Act 2", "Act") },
            { "w05a10", ("Chaos Island", "Act 1", "Act") },
            { "w05a20", ("Chaos Island", "Act 2", "Act") },
            { "w06a10", ("Radical Highway", "Act 1", "Act") },
            { "w06a20", ("Radical Highway", "Act 2", "Act") },
            { "w11b10", ("Biolizard", "Boss", "Boss") },
            { "w12b10", ("Metal Overlord", "Boss", "Boss") },
            { "w13b10", ("Mephiles", "Boss", "Boss") },
            { "w14b10", ("Devil Doom", "Boss", "Boss") },
            { "w11b11", ("Biolizard", "Boss (Hard)", "Boss") },
            { "w12b11", ("Metal Overlord", "Boss (Hard)", "Boss") },
            { "w13b11", ("Mephiles", "Boss (Hard)", "Boss") },
            { "w01c11", ("Space Colony ARK", "Act 1 / Challenge 1", "Challenge Act") },
            { "w01c12", ("Space Colony ARK", "Act 1 / Challenge 2", "Challenge Act") },
            { "w01c21", ("Space Colony ARK", "Act 2 / Challenge 1", "Challenge Act") },
            { "w01c22", ("Space Colony ARK", "Act 2 / Challenge 2", "Challenge Act") },
            { "w02c11", ("Rail Canyon", "Act 1 / Challenge 1", "Challenge Act") },
            { "w02c12", ("Rail Canyon", "Act 1 / Challenge 2", "Challenge Act") },
            { "w02c21", ("Rail Canyon", "Act 2 / Challenge 1", "Challenge Act") },
            { "w02c22", ("Rail Canyon", "Act 2 / Challenge 2", "Challenge Act") },
            { "w03c11", ("Kingdom Valley", "Act 1 / Challenge 1", "Challenge Act") },
            { "w03c12", ("Kingdom Valley", "Act 1 / Challenge 2", "Challenge Act") },
            { "w03c21", ("Kingdom Valley", "Act 2 / Challenge 1", "Challenge Act") },
            { "w03c22", ("Kingdom Valley", "Act 2 / Challenge 2", "Challenge Act") },
            { "w04c11", ("Sunset Heights", "Act 1 / Challenge 1", "Challenge Act") },
            { "w04c12", ("Sunset Heights", "Act 1 / Challenge 2", "Challenge Act") },
            { "w04c21", ("Sunset Heights", "Act 2 / Challenge 1", "Challenge Act") },
            { "w04c22", ("Sunset Heights", "Act 2 / Challenge 2", "Challenge Act") },
            { "w05c11", ("Chaos Island", "Act 1 / Challenge 1", "Challenge Act") },
            { "w05c12", ("Chaos Island", "Act 1 / Challenge 2", "Challenge Act") },
            { "w05c21", ("Chaos Island", "Act 2 / Challenge 1", "Challenge Act") },
            { "w05c22", ("Chaos Island", "Act 2 / Challenge 2", "Challenge Act") },
            { "w01h11", ("Space Colony ARK", "Act 1 / Hard Challenge", "Challenge Act") },
            { "w01h21", ("Space Colony ARK", "Act 2 / Hard Challenge", "Challenge Act") },
            { "w02h11", ("Rail Canyon", "Act 1 / Hard Challenge", "Challenge Act") },
            { "w02h21", ("Rail Canyon", "Act 2 / Hard Challenge", "Challenge Act") },
            { "w03h11", ("Kingdom Valley", "Act 1 / Hard Challenge", "Challenge Act") },
            { "w03h21", ("Kingdom Valley", "Act 2 / Hard Challenge", "Challenge Act") },
            { "w04h11", ("Sunset Heights", "Act 1 / Hard Challenge", "Challenge Act") },
            { "w04h21", ("Sunset Heights", "Act 2 / Hard Challenge", "Challenge Act") },
            { "w05h11", ("Chaos Island", "Act 1 / Hard Challenge", "Challenge Act") },
            { "w05h21", ("Chaos Island", "Act 2 / Hard Challenge", "Challenge Act") },
            { "w01m15", ("Space Colony ARK", "", "Cutscene") },
            { "w20a10", ("Tokyo", "Act 1", "Act") },
            { "w20c11", ("Tokyo", "Challenge 1", "Challenge Act") },
            { "w20c12", ("Tokyo", "Challenge 2", "Challenge Act") },
        };
    }
}
