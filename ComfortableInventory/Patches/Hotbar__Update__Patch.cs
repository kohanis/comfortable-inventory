using System.Linq;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Equip with RMB from selected hotbar slot (when inventory closed)
    /// </summary>
    [HarmonyPatch(typeof(Hotbar), "Update")]
    internal static class Hotbar__Update__Patch
    {
        private static void Postfix(Hotbar __instance)
        {
            var networkPlayer = __instance.playerNetwork;
            if (networkPlayer == null || !networkPlayer.IsLocalPlayer || !MyInput.GetButtonDown("RMB"))
                return;

            var inventory = networkPlayer.Inventory;
            if (inventory.gameObject.activeSelf)
                return;

            var slot = inventory.GetSelectedHotbarSlot();
            var equipType = slot.itemInstance?.settings_equipment.EquipType ?? EquipSlotType.None;
            if (equipType == EquipSlotType.None)
                return;

            var equipSlot = inventory.GetEquipmentSlotFromEquipmentType(equipType);
            if (equipSlot != null)
            {
                if (equipType == EquipSlotType.Backpack)
                    return;
                inventory.SwitchSlots(slot, equipSlot);
            }
            else
            {
                equipSlot = inventory.equipSlots.First(x => x.IsEmpty);
                if (equipSlot == null)
                    return;

                inventory.MoveSlotToEmpty(slot, equipSlot, 1);
            }
        }
    }
}