using System.Reflection;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory
{
    internal static class Reflected
    {
        // Fields

        public static readonly FieldInfo Inventory__dragAmount__FieldInfo =
            AccessTools.Field(typeof(Inventory), "dragAmount");

        public static readonly FieldInfo ItemInstance__baseItem__FieldInfo =
            AccessTools.Field(typeof(ItemInstance), nameof(ItemInstance.baseItem));

        public static readonly FieldInfo SO_FuelValue__fuelValueOfType__FieldInfo =
            AccessTools.Field(typeof(SO_FuelValue), nameof(SO_FuelValue.fuelValueOfType));

        // Methods

        public static readonly MethodInfo Inventory__RemoveItemUses__MethodInfo =
            AccessTools.Method(typeof(Inventory), nameof(Inventory.RemoveItemUses));

        public static readonly MethodInfo Object__op_Inequality__MethodInfo =
            AccessTools.Method(typeof(Object), "op_Inequality");

        public static readonly MethodInfo PlayerInventory__FindSuitableSlot__MethodInfo =
            AccessTools.Method(typeof(PlayerInventory), "FindSuitableSlot",
                new[] { typeof(int), typeof(int), typeof(Item_Base) });

        public static readonly MethodInfo PlayerInventory__GetSelectedHotbarItem__MethodInfo =
            AccessTools.Method(typeof(PlayerInventory), nameof(PlayerInventory.GetSelectedHotbarItem));

        public static readonly MethodInfo PlayerInventory__MoveSlotToEmpty__MethodInfo =
            AccessTools.Method(typeof(PlayerInventory), "MoveSlotToEmpty");

        public static readonly MethodInfo PlayerInventory__StackSlots__MethodInfo =
            AccessTools.Method(typeof(PlayerInventory), "StackSlots");

        public static readonly MethodInfo PlayerInventory__SwitchSlots__MethodInfo =
            AccessTools.Method(typeof(PlayerInventory), "SwitchSlots");

        public static readonly MethodInfo Slot__IncrementUses__MethodInfo =
            AccessTools.Method(typeof(Slot), nameof(Slot.IncrementUses));

        public static readonly MethodInfo Slot_Equip__GetEquipSlotWithTag__MethodInfo =
            AccessTools.Method(typeof(Slot_Equip), nameof(Slot_Equip.GetEquipSlotWithTag));

        public static readonly MethodInfo Tank__HandleAddFuel__MethodInfo =
            AccessTools.Method(typeof(Tank), "HandleAddFuel");

        public static readonly MethodInfo Tank__ModifyTank__MethodInfo =
            AccessTools.Method(typeof(Tank), nameof(Tank.ModifyTank));


        // FieldRefs

        public static readonly AccessTools.FieldRef<Inventory, int> Inventory_dragAmount_Ref =
            AccessTools.FieldRefAccess<Inventory, int>(Inventory__dragAmount__FieldInfo);

        // FastInvokeHandlers
        // TODO: Move to AccessTools.MethodDelegate with Harmony 2.0.2

        public static readonly FastInvokeHandler PlayerInventory__FindSuitableSlot__Invoker =
            MethodInvoker.GetHandler(PlayerInventory__FindSuitableSlot__MethodInfo);

        public static readonly FastInvokeHandler PlayerInventory__MoveSlotToEmpty__Invoker =
            MethodInvoker.GetHandler(PlayerInventory__MoveSlotToEmpty__MethodInfo);

        public static readonly FastInvokeHandler PlayerInventory__StackSlots__Invoker =
            MethodInvoker.GetHandler(PlayerInventory__StackSlots__MethodInfo);

        public static readonly FastInvokeHandler PlayerInventory__SwitchSlots__Invoker =
            MethodInvoker.GetHandler(PlayerInventory__SwitchSlots__MethodInfo);
    }
}