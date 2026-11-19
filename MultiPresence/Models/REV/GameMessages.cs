using System.Text;
using System.Text.RegularExpressions;

namespace MultiPresence.Models.REV
{
    internal static partial class GameMessages
    {
        public static string[] ReadMessages(string executable, string fileName)
        {
            if (Path.GetDirectoryName(executable) is not string directory) return [];
            string path = Path.Combine(directory, "nativePC", "msg", fileName);
            try
            {
                using var stream = File.OpenRead(path);
                if (stream.Length is < 40 or > 1024 * 1024) return [];
                var data = new byte[(int)stream.Length];
                stream.ReadExactly(data);
                return ParseMessages(data);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                return [];
            }
        }

        private static string[] ParseMessages(byte[] data)
        {
            if (!data.AsSpan(0, 4).SequenceEqual("GMD\0"u8) || BitConverter.ToInt32(data, 4) != 0x10201) return [];
            int entries = BitConverter.ToInt32(data, 20);
            int textSize = BitConverter.ToInt32(data, 32);
            if (entries <= 0 || textSize <= 0 || textSize > data.Length - 40) return [];

            string[] messages = Encoding.UTF8.GetString(data, data.Length - textSize, textSize).Split('\0');
            if (messages.Length <= entries) return [];
            return messages.Take(entries).Select(message => MarkupRegex().Replace(message, "").Trim()).ToArray();
        }

        [GeneratedRegex("<[^>]*>", RegexOptions.CultureInvariant)]
        private static partial Regex MarkupRegex();
    }
}
