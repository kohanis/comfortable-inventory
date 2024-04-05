using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory
{
    internal static class Reflected
    {
        // Fields

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

        public static readonly MethodInfo Slot__IncrementUses__MethodInfo =
            AccessTools.Method(typeof(Slot), nameof(Slot.IncrementUses));

        public static readonly MethodInfo Slot_Equip__GetEquipSlotWithTag__MethodInfo =
            AccessTools.Method(typeof(Slot_Equip), nameof(Slot_Equip.GetEquipSlotWithTag));

        public static readonly MethodInfo Tank__HandleAddFuel__MethodInfo =
            AccessTools.Method(typeof(Tank), "HandleAddFuel");

        public static readonly MethodInfo Tank__ModifyTank__MethodInfo =
            AccessTools.Method(typeof(Tank), nameof(Tank.ModifyTank));


        // FieldRefs

        public static readonly AccessTools.FieldRef<int> Inventory_dragAmount_Ref =
            AccessTools.StaticFieldRefAccess<int>(AccessTools.Field(typeof(Inventory), "dragAmount"));
        
        // Delegate types
        
        private delegate Slot PlayerInventory__FindSuitableSlot(PlayerInventory self, int startSlotIndex, int endSlotIndex, Item_Base stackableItem = null);
        
        [HarmonyDelegate(typeof(PlayerInventory), "MoveSlotToEmpty")]
        private delegate void PlayerInventory__MoveSlotToEmpty(PlayerInventory self, Slot fromSlot, Slot toSlot, int amount);
        
        [HarmonyDelegate(typeof(PlayerInventory), "StackSlots")]
        private delegate void PlayerInventory__StackSlots(PlayerInventory self, Slot fromSlot, Slot toSlot, int dragAmount);
        
        [HarmonyDelegate(typeof(PlayerInventory), "SwitchSlots")]
        private delegate void PlayerInventory__SwitchSlots(PlayerInventory self, Slot fromSlot, Slot toSlot);
        
        // Delegates

        private static readonly PlayerInventory__FindSuitableSlot PlayerInventory__FindSuitableSlot__Delegate =
            AccessTools.MethodDelegate<PlayerInventory__FindSuitableSlot>(PlayerInventory__FindSuitableSlot__MethodInfo);

        private static readonly PlayerInventory__MoveSlotToEmpty PlayerInventory__MoveSlotToEmpty__Delegate =
            AccessTools.HarmonyDelegate<PlayerInventory__MoveSlotToEmpty>();

        private static readonly PlayerInventory__StackSlots PlayerInventory__StackSlots__Delegate =
            AccessTools.HarmonyDelegate<PlayerInventory__StackSlots>();

        private static readonly PlayerInventory__SwitchSlots PlayerInventory__SwitchSlots__Delegate =
            AccessTools.HarmonyDelegate<PlayerInventory__SwitchSlots>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Slot FindSuitableSlot(this PlayerInventory self, int startSlotIndex, int endSlotIndex, Item_Base stackableItem = null) => 
            PlayerInventory__FindSuitableSlot__Delegate(self, startSlotIndex, endSlotIndex, stackableItem);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveSlotToEmpty(this PlayerInventory self, Slot fromSlot, Slot toSlot, int amount) => 
            PlayerInventory__MoveSlotToEmpty__Delegate(self, fromSlot, toSlot, amount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StackSlots(this PlayerInventory self, Slot fromSlot, Slot toSlot, int dragAmount) => 
            PlayerInventory__StackSlots__Delegate(self, fromSlot, toSlot, dragAmount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwitchSlots(this PlayerInventory self, Slot fromSlot, Slot toSlot) => 
            PlayerInventory__SwitchSlots__Delegate(self, fromSlot, toSlot);
    }
}