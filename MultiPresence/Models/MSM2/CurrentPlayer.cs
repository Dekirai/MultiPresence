namespace MultiPresence.Models.MSM2
{
    internal static partial class MemoryReader
    {
        private static void ReadCharacter(GameSnapshot snapshot, ulong data)
        {
            var manager = Pointer(data + 0x78);
            var config = Pointer(manager + 0x90);
            var index = Int(manager + 0x98);
            var count = Int(config + 0x38);
            if (count <= 0 || count > 256 || index < 0 || index >= count)
            {
                throw Invalid("character index");
            }

            var entries = ReadRelativePointer(config + 0x30);
            var character = Int(entries + (ulong)index * 0x2A0 + 0x1C);
            snapshot.CharacterId = character;
            snapshot.Character = GameText.GetCharacter(character);
            snapshot.CharacterIcon = GameText.GetCharacterIcon(character);
            if (snapshot.Character.Length == 0)
            {
                snapshot.Character = "Unknown Character";
            }
        }

        private static void ReadLocation(GameSnapshot snapshot, ulong entity)
        {
            var actor = Pointer(entity + 0x10);
            var count = BitConverter.ToUInt16(Bytes(actor + 0x88, 2));
            ulong component = 0;
            if (count > 0)
            {
                if (count > 4096 || (count & (count - 1)) != 0)
                {
                    throw Invalid("component table");
                }

                var entries = Pointer(actor + 0x80);
                var index = (districtTypeAddress >> 4) & (ulong)(count - 1);
                for (var attempt = 0; attempt < count; attempt++)
                {
                    var entry = entries + index * 16;
                    var key = Pointer(entry);
                    if (key == districtTypeAddress)
                    {
                        component = Pointer(entry + 8);
                        break;
                    }

                    if (key == 0)
                    {
                        break;
                    }

                    index = (index + 1) & (ulong)(count - 1);
                }
            }
            else
            {
                count = BitConverter.ToUInt16(Bytes(actor + 0x70, 2));
                if (count > 4096)
                {
                    throw Invalid("component list");
                }

                var entries = Pointer(actor + 0x68);
                for (var index = 0; index < count; index++)
                {
                    var entry = entries + (ulong)index * 16;
                    if (Pointer(entry) == districtTypeAddress)
                    {
                        component = Pointer(entry + 8);
                        break;
                    }
                }
            }

            snapshot.LocationId = component == 0 ? -1 : Int(component + 0x48);
            snapshot.Location = GameText.GetLocation(snapshot.LocationId);
        }

        private static ulong ReadRelativePointer(ulong address)
        {
            var value = Hypervisor.Read<ulong>(address, true);
            if ((value & 0x8000000000000000) != 0)
            {
                return checked((ulong)((long)address + unchecked((int)value)));
            }

            return Pointer(address);
        }
    }
}
