namespace MultiPresence.Models.VOM
{
    internal static partial class MemoryReader
    {
        private static GameSnapshot ReadObjective(GameSnapshot snapshot, ulong mode, ulong save)
        {
            ulong quests = ObjectProperty(mode, "BP_EventQuestComponent");
            if (!IsA(quests, "SakuraEventQuestComponent")) return snapshot;
            var (paramsField, paramsProperty) = Member(Pointer(quests + 0x10), "DataParams");
            ulong param = Struct(Struct(paramsProperty));
            int task = Member(param, "TaskTextID").Offset, main = Member(param, "MainStory").Offset;
            int type = Member(param, "StateType").Offset, paramSize = StructSize(param);

            var (events, eventsProperty) = Member(Pointer(save + 0x10), "EventInformation");
            var (stateField, stateProperty) = Member(Struct(eventsProperty), "QuestData");
            ulong state = Struct(Struct(stateProperty));
            int parent = Member(state, "ParentState").Offset, child = Member(state, "ChildState").Offset, stateSize = StructSize(state);
            if (paramsField < 0x28 || task < 0 || main < 0 || type < 0 || paramSize is < 0x10 or > 0x1000 || events < 0x28 ||
                stateField < 0 || parent < 0 || child < 0 || stateSize is < 2 or > 64)
            {
                return snapshot;
            }

            ulong paramData = Pointer(quests + (ulong)paramsField);
            int paramCount = Hypervisor.Read<int>(quests + (ulong)paramsField + 8, true);
            ulong states = save + (ulong)(events + stateField);
            ulong stateData = Pointer(states);
            int stateCount = Hypervisor.Read<int>(states + 8, true);
            if (paramData == 0 || stateData == 0 || paramCount is < 1 or > 8192 || stateCount != paramCount) return snapshot;

            // The quest component keeps DT_Quest rows in the same order as the save's quest states.
            // Offered quests keep their rows active, so only rows of entered main-story quests count and the latest active row wins.
            var rows = Hypervisor.ReadArray(paramData, paramCount * paramSize, true);
            var progress = Hypervisor.ReadArray(stateData, stateCount * stateSize, true);
            bool entered = false;
            string objective = "";
            for (int index = 0; index < stateCount; index++)
            {
                int row = index * paramSize, entry = index * stateSize;
                bool story = BitConverter.ToInt32(rows, row + main) == 1;
                if (story || rows[row + type] == 1) entered = story && progress[entry + parent] == 2;
                if (!entered || progress[entry + child] != 1) continue;
                string text = Objectives.Get(NameOf(BitConverter.ToUInt64(rows, row + task)));
                if (text.Length > 0) objective = text;
            }
            return snapshot with { Objective = objective };
        }
    }
}
