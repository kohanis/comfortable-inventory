using System.Reflection;
using HarmonyLib;

namespace kohanis.ComfortableInventory.Patches
{
    public static class AugmentedEquipment__Hack
    {
        private static readonly HarmonyMethod PrefixMethod__MethodInfo =
            new HarmonyMethod(typeof(AugmentedEquipment__Hack), nameof(Prefix));

        private static readonly MethodInfo IsAcceptableSlot__MethodInfo =
            AccessTools.Method("AugmentedEquipment+PlayerInventoryFindSuitableSlot_Patch:IsAcceptableSlot");

        public static void Patch(Harmony harmony)
        {
            if (IsAcceptableSlot__MethodInfo == null)
                return;
            harmony.Patch(IsAcceptableSlot__MethodInfo, prefix: PrefixMethod__MethodInfo);
        }

        private static bool Prefix(Slot slot, ref bool __result)
        {
            __result = slot != null && slot.active;
            return false;
        }
    }
}