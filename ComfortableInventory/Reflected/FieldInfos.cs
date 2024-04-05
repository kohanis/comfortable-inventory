using System.Reflection;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Reflected
{
    internal static class FieldInfos
    {
        public static readonly FieldInfo ItemInstance__baseItem =
            AccessTools.Field(typeof(ItemInstance), nameof(ItemInstance.baseItem));

        public static readonly FieldInfo SO_FuelValue__fuelValueOfType =
            AccessTools.Field(typeof(SO_FuelValue), nameof(SO_FuelValue.fuelValueOfType));
    }
}