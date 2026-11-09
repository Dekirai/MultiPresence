using System.ComponentModel;
using System.Text;

namespace MultiPresence.Models.LOKD
{
    internal static class GameText
    {
        private const uint EnglishTableHash = 0xDFE4292C;
        private static byte[]? table;
        private static DateTime nextLoad;

        public static void Reset()
        {
            table = null;
            nextLoad = DateTime.MinValue;
        }

        public static string English(int id)
        {
            if (id <= 0)
            {
                return "";
            }

            if (table == null && DateTime.UtcNow >= nextLoad)
            {
                nextLoad = DateTime.UtcNow.AddSeconds(30);
                try
                {
                    var executable = Hypervisor.Process?.MainModule?.FileName;
                    if (executable != null && Path.IsPathFullyQualified(executable) && Path.GetDirectoryName(executable) is { } directory)
                    {
                        table = LoadTable(Path.Combine(directory, "bigfile.x64.dat"));
                    }
                }
                catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or Win32Exception or InvalidOperationException)
                {
                    return "";
                }
            }

            if (table == null)
            {
                return "";
            }

            int count = BitConverter.ToInt32(table, 4);
            if (id >= count)
            {
                return "";
            }

            int offset = BitConverter.ToInt32(table, 8 + 4 * id);
            if (offset < 8 + 4 * count || offset >= table.Length)
            {
                return "";
            }

            int end = Array.IndexOf(table, (byte)0, offset);
            if (end < offset)
            {
                return "";
            }

            string value = Encoding.UTF8.GetString(table, offset, end - offset);
            return string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static byte[]? LoadTable(string path)
        {
            using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var reader = new BinaryReader(file);
            int entries = reader.ReadInt32();
            if (entries < 1 || 4L + 28L * entries > file.Length)
            {
                return null;
            }

            int first = 0, last = entries - 1;
            while (first <= last)
            {
                int index = first + (last - first) / 2;
                file.Position = 4L + 4L * index;
                uint hash = reader.ReadUInt32();
                if (hash < EnglishTableHash)
                {
                    first = index + 1;
                }
                else if (hash > EnglishTableHash)
                {
                    last = index - 1;
                }
                else
                {
                    file.Position = 4L + 4L * entries + 24L * index;
                    long size = reader.ReadInt64();
                    long offset = reader.ReadInt64();
                    if (size < 12 || size > 16 * 1024 * 1024 || offset < 4L + 28L * entries || offset > file.Length - size)
                    {
                        return null;
                    }

                    file.Position = offset;
                    var data = new byte[(int)size];
                    file.ReadExactly(data);
                    int count = BitConverter.ToInt32(data, 4);
                    return count > 0 && count <= (data.Length - 8) / 4 ? data : null;
                }
            }

            return null;
        }
    }
}
