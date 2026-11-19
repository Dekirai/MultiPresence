namespace MultiPresence.Models.FFRS
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadUnits(GameSnapshot snapshot, ulong instance, List<int> party)
        {
            int leader = party[0];
            snapshot = snapshot with
            {
                Character = GameText.Unit(leader),
                Party = string.Join(", ", party.Skip(1).Select(GameText.Unit).Where(name => name.Length > 0))
            };

            ulong units = ObjectProperty(Subsystem(instance, "CPP_ManualViewModelManager"), "UnitVMManager");
            int mapField = Field(units, "UnitParamVMMap");
            if (mapField == 0) return snapshot;

            // TMap<int32, UCPP_VM_UnitParam*>: unit id at +0, view model at +8.
            ulong unit = 0;
            foreach (int entry in Entries(units + (ulong)mapField, 24, out var elements))
            {
                if (BitConverter.ToInt32(elements, entry) == leader) unit = BitConverter.ToUInt64(elements, entry + 8);
            }
            int idField = Field(unit, "UnitId");
            if (idField == 0 || Hypervisor.Read<int>(unit + (ulong)idField, true) != leader) return snapshot;

            ulong progression = ObjectProperty(unit, "LevelProgressionVM");
            int level = Point(ObjectProperty(progression, "StageVM"), "CurrentPoint");
            ulong experience = ObjectProperty(progression, "PointsVM");
            int current = Point(experience, "CurrentPoint"), needed = Point(experience, "MaxPoint");
            ulong health = ObjectProperty(unit, "HitPointVM"), mana = ObjectProperty(unit, "MagicPointVM"), limit = ObjectProperty(unit, "LimitPointVM");
            int hp = Point(health, "CurrentPoint"), hpMax = Point(health, "MaxPoint"), mp = Point(mana, "CurrentPoint"), mpMax = Point(mana, "MaxPoint");
            int lb = Point(limit, "CurrentPoint"), lbMax = Point(limit, "MaxPoint");
            if (level is < 1 or > 999 || hpMax <= 0 || hp < 0 || hp > hpMax || mpMax < 0 || mp < 0 || mp > mpMax) return snapshot;

            return snapshot with
            {
                Level = level,
                NextLevel = needed > 0 && current >= 0 && current <= needed ? needed - current : null,
                Health = hp,
                MaxHealth = hpMax,
                Mana = mp,
                MaxMana = mpMax,
                LimitBurst = lbMax > 0 && lb >= 0 && lb <= lbMax ? lb * 100 / lbMax : null
            };
        }
    }
}
