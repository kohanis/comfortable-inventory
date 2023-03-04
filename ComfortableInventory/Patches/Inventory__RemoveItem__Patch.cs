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
        private static void Prefix(Inventory __instance, out (int?, int) __state, ref int ___dragAmount)
        {
            __state = ((__instance as PlayerInventory)?.GetSelectedHotbarItem()?.UniqueIndex, ___dragAmount);
            ___dragAmount = -315;
        }

        private static void Postfix(Inventory __instance, (int?, int) __state, out int ___dragAmount)
        {
            var (uniqueIndex, dragAmount) = __state;
            ___dragAmount = dragAmount;

            if (!uniqueIndex.HasValue)
                return;

            var playerInventory = (PlayerInventory)__instance;
            PatchHelpers.ReplenishSlotIfNeeded(playerInventory.GetSelectedHotbarSlot(), uniqueIndex.Value,
                playerInventory);
        }
    }
}