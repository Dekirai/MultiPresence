using System.Globalization;

namespace MultiPresence.Models.RE7
{
    internal static partial class MemoryReader
    {
        private static readonly (string Name, string Pattern, int Operand, int Length)[] LocationPatterns =
        [
            ("Map", "48 8B 15 ?? ?? ?? ?? 48 8B 41 50 45 84 C0 75 ?? 48 83 78 18 00 75 ?? 48 85 D2 74 ?? E9 ?? ?? ?? ?? 48 83 78 18 00 75 ?? 48 85 D2 75 ?? 45 33 C0 41 8D 50 38 E9 ?? ?? ?? ?? E9 ?? ?? ?? ??", 3, 7),
            ("MapCurrentLayout", "45 8B 88 A0 00 00 00 49 8B D2 45 8B 80 A4 00 00 00 48 8B CF E8 ?? ?? ?? ??", 0, 0),
            ("MapSettingsLayout", "4C 8B 82 C8 00 00 00 41 8B E9 48 8B D9 4D 85 C0 75 ?? 45 33 C0 41 8D 50 38 E8 ?? ?? ?? ??", 0, 0),
            ("MapSettingsListLayout", "48 8B 41 50 4D 8B 40 20 48 39 70 18 0F 85 ?? ?? ?? ?? 4D 85 C0 74 ?? 48 8B D3 48 8D 4C 24 28 E8 ?? ?? ?? ?? 0F 10 00 F2 0F 10 48 10 48 8B 43 50 48 39 70 18 0F 85 ?? ?? ?? ?? 4C 89 7C 24 60", 0, 0),
            ("MapSheetCategoryLayout", "4C 8B 47 70 E8 ?? ?? ?? ?? 0F B6 C8 48 8B 43 50 48 39 70 18 75 ?? 85 C9 0F 84 ?? ?? ?? ?? 3B 6F 78 0F 85 ?? ?? ?? ??", 0, 0),
            ("MapGuidLayout", "41 8B 40 28 41 0F B7 48 2C 41 0F B7 50 2E 4D 8B 40 30 EB ??", 0, 0),
        ];

        private sealed record LocationState(ulong Manager, int Category, int Level, Guid Id, string MapId, string Name, string Area, string Room);

        private static string MapCategory(int category)
        {
            return category switch
            {
                0 => "None",
                1 => "Chapter3_Main",
                2 => "Chapter3_Main_East",
                3 => "Chapter3_Garden",
                4 => "Chapter3_3_OldHouse",
                5 => "Chapter3_4_Lucas",
                6 => "Chapter3_5_Boat",
                7 => "Chapter4_1",
                8 => "Chapter4_2",
                9 => "Chapter4_3",
                10 => "Chapter1",
                11 => "Chapter1_Out",
                12 => "Chapter0",
                13 => "Chapter8_1",
                14 => "Chapter8_2",
                15 => "Chapter8_3",
                16 => "Chapter8_4",
                17 => "Chapter8_5",
                18 => "Chapter8_6",
                19 => "Chapter8_7",
                _ => ""
            };
        }

        private static LocationState ReadLocation()
        {
            var manager = Pointer(addresses["Map"]);
            if (!NativeObject(manager))
            {
                return new LocationState(manager, 0, 0, Guid.Empty, "", "", "", "");
            }

            var level = ReadInt(manager + 0xA0);
            var category = ReadInt(manager + 0xA4);
            var categoryName = MapCategory(category);
            var settings = Pointer(manager + 0xC8);
            if (settings == 0 || categoryName.Length == 0)
            {
                return new LocationState(manager, category, level, Guid.Empty, "", "", "", "");
            }

            var list = Pointer(settings + 0x20);
            foreach (var sheet in ReadList(list, 512))
            {
                if (sheet == 0)
                {
                    throw Invalid("missing map sheet");
                }

                if (ReadInt(sheet + 0x78) != level || ReadString(Pointer(sheet + 0x70)) != categoryName)
                {
                    continue;
                }

                var id = ReadGuid(sheet + 0x28);
                var name = GameText.Get(id);
                var mapId = ReadString(Pointer(sheet + 0x10));
                var lastSpace = name.LastIndexOf(' ');
                var area = name;
                var room = "";
                if (lastSpace > 0)
                {
                    var floor = name[(lastSpace + 1)..];
                    if (floor.EndsWith("F", StringComparison.Ordinal) && int.TryParse(floor.TrimStart('B')[..^1], NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    {
                        area = name[..lastSpace];
                        room = floor;
                    }
                }

                if (manager != Pointer(addresses["Map"]) || level != ReadInt(manager + 0xA0) || category != ReadInt(manager + 0xA4) || settings != Pointer(manager + 0xC8) || list != Pointer(settings + 0x20))
                {
                    throw Invalid("location changed during read");
                }

                return new LocationState(manager, category, level, id, mapId, name, area, room);
            }

            return new LocationState(manager, category, level, Guid.Empty, "", "", "", "");
        }
    }
}
