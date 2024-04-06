using System.Runtime.CompilerServices;
using HarmonyLib;

namespace kohanis.ComfortableInventory.Reflected.Delegates
{
    internal static class PlayerInventoryExt
    {
        #region Delegate types

        [HarmonyDelegate(typeof(PlayerInventory), "FindSuitableSlot")]
        private delegate Slot FindSuitableSlot(PlayerInventory self, int startSlotIndex, int endSlotIndex,
            Item_Base stackableItem = null);

        [HarmonyDelegate(typeof(PlayerInventory), "MoveSlotToEmpty")]
        private delegate void MoveSlotToEmpty(PlayerInventory self, Slot fromSlot, Slot toSlot, int amount);

        [HarmonyDelegate(typeof(PlayerInventory), "SwitchSlots")]
        private delegate void SwitchSlots(PlayerInventory self, Slot fromSlot, Slot toSlot);

        #endregion

        #region Delegates

        private static readonly FindSuitableSlot FindSuitableSlotInvoker =
            AccessTools.HarmonyDelegate<FindSuitableSlot>();

        private static readonly MoveSlotToEmpty MoveSlotToEmptyInvoker =
            AccessTools.HarmonyDelegate<MoveSlotToEmpty>();

        private static readonly SwitchSlots SwitchSlotsInvoker =
            AccessTools.HarmonyDelegate<SwitchSlots>();

        #endregion

        #region Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Slot FindSuitableSlotReflected(this PlayerInventory self, int startSlotIndex, int endSlotIndex,
            Item_Base stackableItem = null)
        {
            return FindSuitableSlotInvoker(self, startSlotIndex, endSlotIndex, stackableItem);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveSlotToEmptyReflected(this PlayerInventory self, Slot fromSlot, Slot toSlot, int amount)
            => MoveSlotToEmptyInvoker(self, fromSlot, toSlot, amount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwitchSlotsReflected(this PlayerInventory self, Slot fromSlot, Slot toSlot) =>
            SwitchSlotsInvoker(self, fromSlot, toSlot);

        #endregion
    }
}