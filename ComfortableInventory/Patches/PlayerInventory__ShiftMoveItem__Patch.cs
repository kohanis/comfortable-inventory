using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using kohanis.ComfortableInventory.Reflected;
using kohanis.ComfortableInventory.Reflected.Delegates;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Shift-move whole stack
    /// </summary>
    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.ShiftMoveItem))]
    internal static class PlayerInventory__ShiftMoveItem__Patch
    {
        private static readonly MethodInfo FindSuitableSlotReplacement_MethodInfo =
            AccessTools.Method(typeof(PlayerInventory__ShiftMoveItem__Patch), "FindSuitableSlotReplacement");

        private static Slot FindSuitableSlotReplacement(this PlayerInventory self, int startSlotIndex,
            int endSlotIndex, Item_Base item, Slot slot)
        {
            var allSlots = self.allSlots;
            var count = allSlots.Count;
            if (startSlotIndex < 0 || endSlotIndex > count || item == null)
                return null;

            var uniqueIndex = item.UniqueIndex;
            var isStackable = item.settings_Inventory.Stackable;

            for (var index = startSlotIndex; index < endSlotIndex; index++)
            {
                if (slot.IsEmpty)
                    break;

                var iterSlot = allSlots[index];

                if (!iterSlot.gameObject.activeSelf)
                    continue;

                if (iterSlot.IsEmpty)
                {
                    self.MoveSlotToEmptyReflected(slot, iterSlot, slot.itemInstance.Amount);
                    break;
                }

                if (isStackable && !iterSlot.StackIsFull() && iterSlot.itemInstance.UniqueIndex == uniqueIndex)
                    self.StackSlotsReflected(slot, iterSlot, slot.itemInstance.Amount);
            }

            return null;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var codeInstruction in instructions)
                if (codeInstruction.Calls(MethodInfos.PlayerInventory__FindSuitableSlot))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, FindSuitableSlotReplacement_MethodInfo);
                }
                else
                {
                    yield return codeInstruction;
                }
        }
    }
}