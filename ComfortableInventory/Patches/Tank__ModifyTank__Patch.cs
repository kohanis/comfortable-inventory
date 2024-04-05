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
        private static readonly HarmonyMethod TranspilerMethod =
            new HarmonyMethod(typeof(Tank__ModifyTank__Patch), nameof(Transpiler));

        private static readonly MethodInfo IncrementUsesReplacement_MethodInfo =
            AccessTools.Method(typeof(Tank__ModifyTank__Patch), nameof(IncrementUsesReplacement));

        private static readonly MethodInfo RemoveItemUsesReplacement_MethodInfo =
            AccessTools.Method(typeof(Tank__ModifyTank__Patch), nameof(RemoveItemUsesReplacement));

        public static void Patch(Harmony harmony)
        {
            harmony.Patch(MethodInfos.Tank__ModifyTank, transpiler: TranspilerMethod);
        }

        private static (int, int) Calculate(this Tank tank, float fuelAmount, Item_Base item)
        {
            var fuelValue = LiquidFuelManager.GetFilteredFuelValueSoFromItem(item, tank.tankFiltrationType)
                .fuelValueOfType;
            var uses = (int)fuelAmount / fuelValue;
            var amount = 0;

            if (item.settings_Inventory.StackSize != 1)
            {
                var maxUses = item.MaxUses;
                amount = uses / maxUses;
                if (amount > 0) uses %= maxUses;
            }

            return (uses, amount);
        }

        private static void IncrementUsesReplacement(Slot self, int uses, bool arg2, Tank tank, float fuelAmount,
            Item_Base item)
        {
            if (uses != -1)
            {
                self.IncrementUses(uses, arg2);
                return;
            }

            int amount;
            (uses, amount) = tank.Calculate(fuelAmount, item);
            if (amount > 0)
                self.RemoveItem(amount);
            if (uses > 0)
                self.IncrementUses(-uses, arg2);
        }

        private static void RemoveItemUsesReplacement(Inventory self, string uniqueItemName, int uses,
            bool addItemAfterUseToInventory, Tank tank, float fuelAmount, Item_Base item)
        {
            int amount;
            (uses, amount) = tank.Calculate(fuelAmount, item);
            if (amount > 0)
                self.RemoveItem(uniqueItemName, amount);
            if (uses > 0)
                self.RemoveItemUses(uniqueItemName, uses, addItemAfterUseToInventory);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
                if (instruction.Calls(MethodInfos.Slot__IncrementUses))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    yield return new CodeInstruction(OpCodes.Call, IncrementUsesReplacement_MethodInfo);
                }
                else if (instruction.Calls(MethodInfos.Inventory__RemoveItemUses))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Ldarg_3);
                    yield return new CodeInstruction(OpCodes.Call, RemoveItemUsesReplacement_MethodInfo);
                }
                else
                {
                    yield return instruction;
                }
        }
    }
}