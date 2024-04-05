// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Reflection.Emit;
using kohanis.ComfortableInventory.Reflected.Delegates;

namespace kohanis.ComfortableInventory.Patches
{
    internal static class PatchHelpers
    {
        public static readonly Dictionary<OpCode, OpCode> MirroredOpcodes = new Dictionary<OpCode, OpCode>
        {
            { OpCodes.Ldloc, OpCodes.Stloc },
            { OpCodes.Ldloc_0, OpCodes.Stloc_0 },
            { OpCodes.Ldloc_1, OpCodes.Stloc_1 },
            { OpCodes.Ldloc_2, OpCodes.Stloc_2 },
            { OpCodes.Ldloc_3, OpCodes.Stloc_3 },
            { OpCodes.Ldloc_S, OpCodes.Stloc_S },
            { OpCodes.Stloc, OpCodes.Ldloc },
            { OpCodes.Stloc_0, OpCodes.Ldloc_0 },
            { OpCodes.Stloc_1, OpCodes.Ldloc_1 },
            { OpCodes.Stloc_2, OpCodes.Ldloc_2 },
            { OpCodes.Stloc_3, OpCodes.Ldloc_3 },
            { OpCodes.Stloc_S, OpCodes.Ldloc_S }
        };

        public static void ReplenishSlotIfNeeded(Slot slot, int originalUniqueIndex, PlayerInventory playerInventory)
        {
            if (slot.itemInstance?.UniqueIndex == originalUniqueIndex)
                return;

            var allSlots = playerInventory.allSlots;
            for (int index = playerInventory.hotslotCount, count = allSlots.Count; index < count; index++)
            {
                var localSlot = allSlots[index];

                if (!localSlot.gameObject.activeSelf || localSlot.IsEmpty)
                    continue;

                var slotItemInstance = localSlot.itemInstance;
                if (slotItemInstance.UniqueIndex != originalUniqueIndex)
                    continue;

                if (slot.IsEmpty)
                    playerInventory.MoveSlotToEmptyReflected(localSlot, slot, slotItemInstance.Amount);
                else
                    playerInventory.SwitchSlotsReflected(localSlot, slot);

                break;
            }
        }
    }
}