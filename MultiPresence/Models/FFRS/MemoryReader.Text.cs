using System.Text;

namespace MultiPresence.Models.FFRS
{
    internal static partial class MemoryReader
    {
        private const int TextKeysPerChunk = 32768 / 24;

        private static (string Table, string Key) TextKey(ulong text)
        {
            // String table texts reference FStringTableReferenceData (TSharedFromThis): TableId at +0x10, FTextKey at +0x18.
            ulong data = Pointer(text);
            if (data == 0 || !Hypervisor.TryRead<ulong>(data + 0x18, out var reference, true) || reference == 0 ||
                !Hypervisor.TryRead<ulong>(reference, out var self, true) || self != reference)
            {
                return ("", "");
            }
            string table = FName(reference + 0x10);
            return (table[(table.LastIndexOf('.') + 1)..], KeyString(Hypervisor.Read<int>(reference + 0x18, true)));
        }

        private static string KeyString(int index)
        {
            if (index < 0) return "";
            if (textKeys.TryGetValue(index, out var key)) return key;
            int chunks = Hypervisor.Read<int>(textKeyState + 0x10, true), total = Hypervisor.Read<int>(textKeyState + 0x18, true);
            if (index >= total || index / TextKeysPerChunk >= chunks) throw Invalid("text key");
            ulong chunk = Pointer(Pointer(textKeyState + 8) + (ulong)(index / TextKeysPerChunk) * 8);
            ulong entry = chunk + (ulong)(index % TextKeysPerChunk) * 24;
            ulong text = Pointer(entry);
            int length = Hypervisor.Read<int>(entry + 8, true);
            if (chunk == 0 || text == 0 || length is <= 0 or > 256) throw Invalid("text key entry");
            return textKeys[index] = Encoding.UTF8.GetString(Hypervisor.ReadArray(text, length, true));
        }
    }
}
