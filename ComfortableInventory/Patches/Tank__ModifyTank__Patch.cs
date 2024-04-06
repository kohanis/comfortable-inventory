using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using kohanis.ComfortableInventory.Reflected;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    internal static class Tank__ModifyTank__Patch
    {
        private static bool done;

        private static readonly HarmonyMethod TranspilerMethod =
            new HarmonyMethod(typeof(Tank__ModifyTank__Patch), nameof(Transpiler));

        private static readonly MethodInfo IncrementUsesInject_MethodInfo =
            AccessTools.Method(typeof(Tank__ModifyTank__Patch), nameof(IncrementUsesInject));

        private static readonly MethodInfo RemoveItemUsesInject_MethodInfo =
            AccessTools.Method(typeof(Tank__ModifyTank__Patch), nameof(RemoveItemUsesInject));

        public static bool Patch(Harmony harmony)
        {
            harmony.Patch(MethodInfos.Tank__ModifyTank, transpiler: TranspilerMethod);
            return done;
        }

        private static (int, int) Calculate(this Tank tank, float fuelAmount, Item_Base item)
        {
            var fuelValue = LiquidFuelManager.GetFilteredFuelValueSoFromItem(item, tank.tankFiltrationType)
                .fuelValueOfType;
            var uses = Math.Max((int)fuelAmount / fuelValue - 1, 0);
            var amount = 0;

            if (uses > 0 && item.settings_Inventory.StackSize != 1)
            {
                var maxUses = item.MaxUses;
                amount = uses / maxUses;
                if (amount > 0) uses %= maxUses;
            }

            return (uses, amount);
        }

        private static void IncrementUsesInject(Slot self, Tank tank, float fuelAmount, Item_Base item)
        {
            var (uses, amount) = tank.Calculate(fuelAmount, item);
            if (amount > 0)
                self.RemoveItem(amount);
            if (uses > 0)
                self.IncrementUses(-uses);
        }

        private static void RemoveItemUsesInject(Inventory self, string uniqueItemName, Tank tank,
            float fuelAmount, Item_Base item)
        {
            var (uses, amount) = tank.Calculate(fuelAmount, item);
            if (amount > 0)
                self.RemoveItem(uniqueItemName, amount);
            if (uses > 0)
                self.RemoveItemUses(uniqueItemName, uses);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            done = false;

            var codeMatcher = new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldc_I4_M1),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Callvirt, MethodInfos.Slot__IncrementUses)
                );

            if (codeMatcher.IsInvalid)
                return null;

            var localSlot = generator.DeclareLocal(typeof(Slot));

            codeMatcher
                .InsertAndAdvance(
                    new CodeInstruction(OpCodes.Stloc, localSlot),
                    new CodeInstruction(OpCodes.Ldloc, localSlot),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, IncrementUsesInject_MethodInfo),
                    new CodeInstruction(OpCodes.Ldloc, localSlot)
                )
                .Advance(3)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Ldc_I4_1),
                    new CodeMatch(OpCodes.Callvirt, MethodInfos.Inventory__RemoveItemUses)
                );

            if (codeMatcher.IsInvalid)
                return null;

            var localInventory = generator.DeclareLocal(typeof(Inventory));
            var localString = generator.DeclareLocal(typeof(string));

            codeMatcher
                .Insert(
                    new CodeInstruction(OpCodes.Stloc, localString),
                    new CodeInstruction(OpCodes.Stloc, localInventory),
                    new CodeInstruction(OpCodes.Ldloc, localInventory),
                    new CodeInstruction(OpCodes.Ldloc, localString),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, RemoveItemUsesInject_MethodInfo),
                    new CodeInstruction(OpCodes.Ldloc, localInventory),
                    new CodeInstruction(OpCodes.Ldloc, localString)
                );

            done = true;
            return codeMatcher.Instructions();
        }
    }
}