namespace MultiPresence.Models.VOM
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadPlayer(GameSnapshot snapshot, ulong pawn)
        {
            int idField = Field(pawn, "CharacterId");
            string id = idField == 0 ? "" : FName(pawn + (ulong)idField);
            if (id.Length < 4) return snapshot;
            snapshot = snapshot with { Character = GameText.Hero(id[..4]), Class = GameText.Class(id.ToUpperInvariant()) };

            ulong abilities = ObjectProperty(pawn, "ASC");
            if (!IsA(abilities, "AbilitySystemComponent") || ObjectProperty(abilities, "AvatarActor") != pawn) return snapshot;
            int setsField = Field(abilities, "SpawnedAttributes");
            if (setsField == 0) return snapshot;
            ulong sets = Pointer(abilities + (ulong)setsField);
            int count = Hypervisor.Read<int>(abilities + (ulong)setsField + 8, true);
            if (sets == 0 || count is < 1 or > 32) return snapshot;

            ulong attributes = 0;
            foreach (ulong set in Hypervisor.Read<ulong>(sets, count, true))
            {
                if (ClassName(set) == "ActBaseAttributeSet") attributes = set;
            }
            if (attributes == 0) return snapshot;

            float level = Attribute(attributes, "Lv"), health = Attribute(attributes, "HP"), maxHealth = Attribute(attributes, "HPMax");
            float mana = Attribute(attributes, "MP"), maxMana = Attribute(attributes, "MPMax");
            float strike = Attribute(attributes, "SP"), maxStrike = Attribute(attributes, "SPMax");
            if (level is < 1 or > 999 || maxHealth is <= 0 or > 999999 || health < 0 || health > maxHealth ||
                maxMana is < 0 or > 99999 || mana < 0 || mana > maxMana || maxStrike <= 0 || strike < 0 || strike > maxStrike)
            {
                return snapshot;
            }

            return snapshot with
            {
                Level = (int)level,
                Health = (int)MathF.Ceiling(health),
                MaxHealth = (int)MathF.Ceiling(maxHealth),
                Mana = (int)MathF.Ceiling(mana),
                MaxMana = (int)MathF.Ceiling(maxMana),
                ClassStrike = (int)(strike * 100 / maxStrike)
            };
        }

        private static float Attribute(ulong set, string name)
        {
            var (offset, property) = Member(Pointer(set + 0x10), name);
            int current = Member(Struct(property), "CurrentValue").Offset;
            if (offset < 0x28 || current < 0) throw Invalid("attribute " + name);
            float value = Hypervisor.Read<float>(set + (ulong)offset + (ulong)current, true);
            return float.IsFinite(value) ? value : -1;
        }
    }
}
