using System.Globalization;
using System.Text;

namespace MultiPresence.Models.REV2
{
    internal static class GameMessages
    {
        public static Dictionary<int, string> ReadLabels(string executable, string fileName, string prefix)
        {
            if (Path.GetDirectoryName(executable) is not string directory) return [];
            string path = Path.Combine(directory, "nativePCNext", "message", "win", fileName);
            try
            {
                using var stream = File.OpenRead(path);
                if (stream.Length is < 40 or > 1024 * 1024) return [];
                var data = new byte[(int)stream.Length];
                stream.ReadExactly(data);
                return ParseLabels(data, prefix);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                return [];
            }
        }

        private static Dictionary<int, string> ParseLabels(byte[] data, string prefix)
        {
            var labels = new Dictionary<int, string>();
            if (!data.AsSpan(0, 4).SequenceEqual("GMD\0"u8) || BitConverter.ToInt32(data, 4) != 0x10302) return labels;
            int entries = BitConverter.ToInt32(data, 20);
            int keySize = BitConverter.ToInt32(data, 28);
            int textSize = BitConverter.ToInt32(data, 32);
            int nameSize = BitConverter.ToInt32(data, 36);
            if (keySize <= 0 || textSize <= 0 || (long)textSize + keySize > data.Length ||
                nameSize < 0 || entries < 0) return labels;
            int textStart = data.Length - textSize;
            int keyStart = textStart - keySize;
            if (41L + nameSize + 20L * entries > keyStart) return labels;

            byte[] prefixBytes = Encoding.ASCII.GetBytes(prefix);
            string[] messages = Encoding.UTF8.GetString(data, textStart, textSize).Split('\0');
            for (int i = 0; i < entries; i++)
            {
                int row = 41 + nameSize + i * 20;
                int index = BitConverter.ToInt32(data, row);
                int offset = BitConverter.ToInt32(data, row + 12);
                if ((uint)index >= messages.Length || (uint)offset >= keySize) continue;
                var key = data.AsSpan(keyStart + offset, keySize - offset);
                int end = key.IndexOf((byte)0);
                if (end <= prefixBytes.Length || !key.StartsWith(prefixBytes)) continue;
                if (int.TryParse(key[prefixBytes.Length..end], NumberStyles.None, CultureInfo.InvariantCulture, out int id))
                    labels[id] = messages[index];
            }
            return labels;
        }
    }
}
