namespace MultiPresence.Models.SH2
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadPlayer(GameSnapshot snapshot, ulong pawn)
        {
            ulong health = ObjectProperty(pawn, "Health");
            int healthField = Field(health, "HealthValue");
            if (healthField != 0 && ObjectProperty(health, "OwnerCharacter") == pawn &&
                Hypervisor.TryRead<float>(health + (ulong)healthField, out var current, true) &&
                float.IsFinite(current) && current is >= 0 and <= 100)
            {
                // The health setter clamps to 100. GetHealthState returns the cached injury enum.
                byte injury = Hypervisor.Read<byte>(health + (ulong)healthField + 0x1C, true);
                snapshot = snapshot with { Health = current, MaxHealth = 100, HealthStatus = injury switch
                {
                    0 => "Healthy", 1 => "Low Injury", 2 => "Heavy Injury", 3 => "Critical Injury", 4 => "Dead", _ => ""
                } };
            }

            ulong combat = ObjectProperty(pawn, "Combat");
            if (ObjectProperty(combat, "OwnerCharacter") != pawn) return snapshot;
            ulong manager = FindWeaponManager(combat);
            int equipped = Field(manager, "EquippedWeapon");
            if (equipped == 0 || ObjectProperty(manager, "OwnerCharacter") != pawn) return snapshot;
            ulong slot = manager + (ulong)equipped;
            ulong weapon = Pointer(slot);
            if (weapon == 0) return snapshot with { Weapon = "Unarmed" };

            int owner = Field(weapon, "OwnerCharacter");
            if (owner == 0 || Pointer(weapon + (ulong)owner) != pawn || !IsA(weapon, "SHItemWeapon")) return snapshot;
            // GetEquippedWeaponName returns the item row name immediately after OwnerCharacter.
            string weaponName = FName(weapon + (ulong)owner + 8) switch
            {
                "WoodenPlank" => "Wooden Plank",
                "IronPipe" => "Steel Pipe",
                "Chainsaw" => "Chainsaw",
                "LabOW_GreatKnife" => "Great Knife",
                "Pistol" => "Handgun",
                "Shotgun" => "Shotgun",
                "Rifle" => "Hunting Rifle",
                "None" => "",
                var id => id
            };

            int? ammo = null, reserve = null;
            if (IsA(weapon, "SHItemWeaponRanged"))
            {
                int firePoint = Field(weapon, "RegisteredFirePoint");
                int capacity = Field(weapon, "WeaponClipSize");
                if (firePoint > 0 && capacity > 0)
                {
                    // HasAnyAmmoInClip reads the count after the fire point pointer and its state flags.
                    int loaded = Hypervisor.Read<int>(weapon + (ulong)firePoint + 12, true);
                    int clipSize = Hypervisor.Read<int>(weapon + (ulong)capacity, true);
                    if (clipSize is > 0 and <= 1000 && loaded >= 0 && loaded <= clipSize + 1) ammo = loaded;
                }
                int item = Field(weapon, "WeaponAmmoItem");
                if (item > 0)
                {
                    ulong ammunitionRow = Pointer(weapon + (ulong)item + 8);
                    reserve = ReadItemCount(ObjectProperty(pawn, "Items"), ammunitionRow);
                }
            }
            if (Pointer(slot) != weapon) return snapshot;
            return snapshot with { Weapon = weaponName, Ammo = ammo, ReserveAmmo = reserve };
        }

        private static ulong FindWeaponManager(ulong combat)
        {
            int field = Field(combat, "OwnedSubcomponents");
            if (field == 0) return 0;
            ulong set = combat + (ulong)field;
            ulong entries = Pointer(set);
            int count = Hypervisor.Read<int>(set + 8, true);
            if (entries == 0 || count is < 1 or > 128 || Hypervisor.Read<int>(set + 0x28, true) != count) return 0;

            ulong bits = Pointer(set + 0x20);
            if (bits == 0) bits = set + 0x10;
            var allocated = Hypervisor.Read<uint>(bits, (count + 31) / 32, true);
            for (int i = 0; i < count; i++)
            {
                if ((allocated[i / 32] & (1u << (i % 32))) == 0) continue;
                ulong component = Pointer(entries + (ulong)(i * 16));
                if (ClassName(component) == "SHWeaponManageCmbSubcomp" && ObjectProperty(component, "OwnerComponent") == combat)
                    return component;
            }
            return 0;
        }

        private static int? ReadItemCount(ulong items, ulong row)
        {
            int field = Field(items, "CollectedItems");
            if (row == 0 || field == 0) return null;
            ulong array = items + (ulong)field;
            ulong data = Pointer(array);
            int count = Hypervisor.Read<int>(array + 8, true);
            int capacity = Hypervisor.Read<int>(array + 12, true);
            if (count < 0 || count > capacity || capacity > 4096 || (count > 0 && data == 0)) return null;
            if (count == 0) return 0;
            // GetItemCount compares each FName and returns the following quantity, or zero for an absent row.
            var bytes = Hypervisor.ReadArray(data, count * 12, true);
            for (int i = 0; i < bytes.Length; i += 12)
            {
                if (BitConverter.ToUInt64(bytes, i) != row) continue;
                int quantity = BitConverter.ToInt32(bytes, i + 8);
                return quantity >= 0 ? quantity : null;
            }
            return 0;
        }
    }
}
