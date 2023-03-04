using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Replenish slot if needed
    /// </summary>
    [HarmonyPatch(typeof(Slot), nameof(Slot.RemoveItem))]
    public static class Slot__RemoveItem__Patch
    {
        private static void Prefix(Slot __instance, out int? __state, Inventory ___inventory)
        {
            __state = __instance.IsEmpty || ___inventory == null || ___inventory.gameObject.activeSelf ||
                      Reflected.Inventory_dragAmount_Ref(___inventory) == -315
                ? (int?)null
                : __instance.itemInstance.UniqueIndex;
        }

        private static void Postfix(Slot __instance, int? __state, Inventory ___inventory)
        {
            if (!__state.HasValue)
                return;

            if (___inventory is PlayerInventory inventory)
                PatchHelpers.ReplenishSlotIfNeeded(__instance, __state.Value, inventory);
        }
    }
}