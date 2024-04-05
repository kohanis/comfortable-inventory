using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Replenish slot if needed
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.RemoveItem))]
    internal static class Inventory__RemoveItem__Patch
    {
        private static void Prefix(Inventory __instance, out int? __state)
        {
            Slot__RemoveItem__Patch.Skip = true;

            __state = (__instance as PlayerInventory)?.GetSelectedHotbarItem()?.UniqueIndex;
        }

        private static void Postfix(Inventory __instance, int? __state)
        {
            Slot__RemoveItem__Patch.Skip = false;

            if (!__state.HasValue)
                return;

            var playerInventory = (PlayerInventory)__instance;
            PatchHelpers.ReplenishSlotIfNeeded(playerInventory.GetSelectedHotbarSlot(), __state.GetValueOrDefault(),
                playerInventory);
        }
    }
}