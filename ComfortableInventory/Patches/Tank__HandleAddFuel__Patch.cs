using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using kohanis.ComfortableInventory.Reflected;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    internal class Tank__HandleAddFuel__Patch
    {
        private static bool done;

        // TODO: move this to HarmonyPatchCategory with release
        private static readonly HarmonyMethod TranspilerMethod =
            new HarmonyMethod(typeof(Tank__HandleAddFuel__Patch), nameof(Transpiler));

        private static readonly MethodInfo CalcAmount_MethodInfo =
            AccessTools.Method(typeof(Tank__HandleAddFuel__Patch), nameof(Calculate));

        public static void Patch(Harmony harmony)
        {
            if (!Tank__ModifyTank__Patch.Patch(harmony))
            {
                ComfortableInventory.LogError(
                    "Tank.ModifyTank patch failed. Tanks multi-input functionality will not work");
                return;
            }

            harmony.Patch(MethodInfos.Tank__HandleAddFuel, transpiler: TranspilerMethod);
            if (!done)
                ComfortableInventory.LogError(
                    "Tank.HandleAddFuel patch failed. Tanks multi-input functionality will not work");
        }

        private static int Calculate(int original, Tank self, Item_Base item, ItemInstance hotslot)
        {
            if (hotslot == null || !Input.GetKey(KeyCode.LeftShift)) 
                return original;

            return original * Math.Min((int)(self.maxCapacity - self.CurrentTankAmount) / original,
                hotslot.UsesInStack);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            done = false;

            var codeMatcher = new CodeMatcher(instructions)
                .MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, FieldInfos.Tank__acceptableTypes),
                    new CodeMatch(ins => ins.IsLdloc(), "target"),
                    new CodeMatch(OpCodes.Ldfld, FieldInfos.ItemInstance__baseItem),
                    new CodeMatch(OpCodes.Callvirt)
                );

            if (codeMatcher.IsInvalid)
                return null;

            var itemInstanceLdloc = codeMatcher.NamedMatch("target").Clone();

            codeMatcher
                .MatchEndForward(
                    new CodeMatch(ins => ins.IsLdloc(), "target"),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, FieldInfos.Tank__tankFiltrationType),
                    new CodeMatch(OpCodes.Call, MethodInfos.LiquidFuelManager__GetFilteredFuelValueSoFromItem),
                    new CodeMatch(ins => ins.IsStloc())
                );

            if (codeMatcher.IsInvalid)
                return null;

            var baseItemLdloc = codeMatcher.NamedMatch("target").Clone();

            codeMatcher
                .SearchForward(ins => ins.LoadsField(FieldInfos.SO_FuelValue__fuelValueOfType))
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    baseItemLdloc,
                    itemInstanceLdloc,
                    new CodeInstruction(OpCodes.Call, CalcAmount_MethodInfo)
                );

            done = true;
            return codeMatcher.Instructions();
        }
    }
}