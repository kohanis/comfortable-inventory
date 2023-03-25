using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Prefer inventory functional
    /// </summary>
    [HarmonyPatch(typeof(Inventory), "FindSuitableSlot")]
    internal static class Inventory__FindSuitableSlot__Patch
    {
        private static bool Prefix(Inventory __instance, Item_Base stackableItem, ref Slot __result)
        {
            if (!Config.PreferInventory || !(__instance is PlayerInventory self))
                return true;

            var subCategory = stackableItem.settings_recipe.SubCategory;
            if ((Config.PreferHotbarHealing && subCategory == "Healing") ||
                (Config.PreferHotbarEquipment && stackableItem.settings_equipment.EquipType != EquipSlotType.None) ||
                (Config.PreferHotbarTools && stackableItem.IsTool()) ||
                (Config.PreferHotbarCookedFood && stackableItem.IsCookedFood()) ||
                (Config.PreferHotbarBuildable &&
                 (stackableItem.settings_buildable.Placeable || subCategory == "Batteries")))
                return true;


            if (stackableItem.settings_Inventory.Stackable)
                __result = (Slot)Reflected.PlayerInventory__FindSuitableSlot__Invoker(self,
                    new object[] { 0, self.hotslotCount, stackableItem });

            if (__result == null || __result.IsEmpty)
            {
                var slot = (Slot)Reflected.PlayerInventory__FindSuitableSlot__Invoker(self,
                    new object[] { self.hotslotCount, self.allSlots.Count, stackableItem });

                if (slot != null)
                    __result = slot;
            }

            return false;
        }
    }
}