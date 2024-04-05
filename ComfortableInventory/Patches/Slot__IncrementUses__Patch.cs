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
        private static void Prefix(Slot __instance, int amountOfUsesToAdd, ref int? __state, Inventory ___inventory)
        {
            if (amountOfUsesToAdd < 0 && !__instance.IsEmpty &&
                (___inventory as PlayerInventory)?.hotbar.IsSelectedHotSlot(__instance) == true)
            {
                __state = __instance.itemInstance.UniqueIndex;
            }
        }

        private static void Postfix(Slot __instance, int? __state, Inventory ___inventory)
        {
            if (!__state.HasValue)
                return;

            PatchHelpers.ReplenishSlotIfNeeded(__instance, __state.GetValueOrDefault(), (PlayerInventory)___inventory);
        }
    }
}