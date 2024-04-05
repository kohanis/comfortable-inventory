using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Replenish slot if needed
    /// </summary>
    [HarmonyPatch(typeof(Slot), nameof(Slot.RemoveItem))]
    internal static class Slot__RemoveItem__Patch
    {
        public static bool Skip = false;

        private static void Prefix(Slot __instance, ref int? __state, Inventory ___inventory)
        {
            if (Skip || __instance.IsEmpty || !(___inventory is PlayerInventory playerInventory) ||
                playerInventory.gameObject.activeSelf || !playerInventory.hotbar.ContainsSlot(__instance))
            {
                return;
            }

            __state = __instance.itemInstance.UniqueIndex;
        }

        private static void Postfix(Slot __instance, int? __state, Inventory ___inventory)
        {
            if (__state.HasValue && ___inventory is PlayerInventory inventory)
                PatchHelpers.ReplenishSlotIfNeeded(__instance, __state.GetValueOrDefault(), inventory);
        }
    }
}