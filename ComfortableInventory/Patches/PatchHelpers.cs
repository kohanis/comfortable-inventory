// ReSharper disable InconsistentNaming

using kohanis.ComfortableInventory.Reflected.Delegates;

namespace kohanis.ComfortableInventory.Patches
{
    internal static class PatchHelpers
    {
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