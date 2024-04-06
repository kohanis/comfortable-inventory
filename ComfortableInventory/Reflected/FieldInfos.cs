using System.Reflection;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Reflected
{
    internal static class FieldInfos
    {
        public static readonly FieldInfo PlayerInventory__hotbar =
            AccessTools.Field(typeof(PlayerInventory), nameof(PlayerInventory.hotbar));

        public static readonly FieldInfo ItemInstance__baseItem =
            AccessTools.Field(typeof(ItemInstance), nameof(ItemInstance.baseItem));

        public static readonly FieldInfo Tank__acceptableTypes =
            AccessTools.Field(typeof(Tank), "acceptableTypes");

        public static readonly FieldInfo Tank__tankFiltrationType =
            AccessTools.Field(typeof(Tank), nameof(Tank.tankFiltrationType));

        public static readonly FieldInfo SO_FuelValue__fuelValueOfType =
            AccessTools.Field(typeof(SO_FuelValue), nameof(SO_FuelValue.fuelValueOfType));
    }
}