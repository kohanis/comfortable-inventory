using System.Reflection;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Reflected
{
    internal static class MethodInfos
    {
        public static readonly MethodInfo Inventory__RemoveItemUses =
            AccessTools.Method(typeof(Inventory), nameof(Inventory.RemoveItemUses));

        public static readonly MethodInfo Object__op_Inequality =
            AccessTools.Method(typeof(Object), "op_Inequality");

        public static readonly MethodInfo PlayerInventory__FindSuitableSlot =
            AccessTools.Method(typeof(PlayerInventory), "FindSuitableSlot", 
                new[] { typeof(int), typeof(int), typeof(Item_Base) });

        public static readonly MethodInfo PlayerInventory__GetSelectedHotbarItem =
            AccessTools.Method(typeof(PlayerInventory), nameof(PlayerInventory.GetSelectedHotbarItem));

        public static readonly MethodInfo Slot__IncrementUses =
            AccessTools.Method(typeof(Slot), nameof(Slot.IncrementUses));

        public static readonly MethodInfo Slot_Equip__GetEquipSlotWithTag =
            AccessTools.Method(typeof(Slot_Equip), nameof(Slot_Equip.GetEquipSlotWithTag));

        public static readonly MethodInfo Tank__HandleAddFuel =
            AccessTools.Method(typeof(Tank), "HandleAddFuel");

        public static readonly MethodInfo Tank__ModifyTank =
            AccessTools.Method(typeof(Tank), nameof(Tank.ModifyTank));
    }
}