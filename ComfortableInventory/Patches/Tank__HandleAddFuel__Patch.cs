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
        // TODO: move this to HarmonyPatchCategory with release
        private static readonly HarmonyMethod TranspilerMethod =
            new HarmonyMethod(typeof(Tank__HandleAddFuel__Patch), nameof(Transpiler));

        private static readonly MethodInfo CalcAmount_MethodInfo =
            AccessTools.Method(typeof(Tank__HandleAddFuel__Patch), nameof(Calculate));

        public static void Patch(Harmony harmony)
        {
            try
            {
                harmony.Patch(MethodInfos.Tank__HandleAddFuel, transpiler: TranspilerMethod);
            }
            catch (TranspilerException e)
            {
                ComfortableInventory.LogError(e.Message);
                return;
            }

            Tank__ModifyTank__Patch.Patch(harmony);
        }

        private static int Calculate(int original, Tank self, Item_Base item, ItemInstance hotslot)
        {
            if (!Input.GetKey(KeyCode.LeftShift) || hotslot.baseItem != item) return original;

            return original * Math.Min((int)(self.maxCapacity - self.CurrentTankAmount) / original,
                hotslot.UsesInStack);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeInstruction itemInstanceStloc = null;
            CodeInstruction baseItemStloc = null;
            var lastInstruction = new CodeInstruction(OpCodes.Nop);

            foreach (var instruction in instructions)
            {
                if (itemInstanceStloc == null &&
                    lastInstruction.Calls(MethodInfos.PlayerInventory__GetSelectedHotbarItem) &&
                    instruction.IsStloc())
                    itemInstanceStloc = instruction;
                else if (baseItemStloc == null &&
                         lastInstruction.LoadsField(FieldInfos.ItemInstance__baseItem) && instruction.IsStloc())
                    baseItemStloc = instruction;

                lastInstruction = instruction;
                yield return instruction;

                if (itemInstanceStloc != null && baseItemStloc != null &&
                    instruction.LoadsField(FieldInfos.SO_FuelValue__fuelValueOfType))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return baseItemStloc.Clone(PatchHelpers.MirroredOpcodes[baseItemStloc.opcode]);
                    yield return itemInstanceStloc.Clone(PatchHelpers.MirroredOpcodes[itemInstanceStloc.opcode]);
                    yield return new CodeInstruction(OpCodes.Call, CalcAmount_MethodInfo);
                }
            }

            if (itemInstanceStloc == null || baseItemStloc == null)
                throw new TranspilerException(
                    "Unexpected method structure for tanks multi-input functionality will not work");
        }
    }
}