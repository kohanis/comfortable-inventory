using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Replenish slot if needed
    /// </summary>
    [HarmonyPatch(typeof(Slot), nameof(Slot.IncrementUses))]
    internal static class Slot__IncrementUses__Patch
    {
        private static void Prefix(Slot __instance, int amountOfUsesToAdd, out int? __state, Inventory ___inventory)
        {
            __state = amountOfUsesToAdd < 0 && !__instance.IsEmpty &&
                      (___inventory as PlayerInventory)?.hotbar.IsSelectedHotSlot(__instance) == true
                ? __instance.itemInstance.UniqueIndex
                : (int?)null;
        }

        private static void Postfix(Slot __instance, int? __state, Inventory ___inventory)
        {
            if (!__state.HasValue)
                return;

            PatchHelpers.ReplenishSlotIfNeeded(__instance, __state.Value, (PlayerInventory)___inventory);
        }
    }
}