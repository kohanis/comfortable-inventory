using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using kohanis.ComfortableInventory.Reflected;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Allows switching equipment of same type (except backpacks)
    /// </summary>
    [HarmonyPatch(typeof(Inventory), "SwitchSlots")]
    internal static class Inventory__SwitchSlots__Patch
    {
        private static readonly MethodInfo CustomCheck_MethodInfo =
            AccessTools.Method(typeof(Inventory__SwitchSlots__Patch), nameof(CustomCheck));

        private static bool? CustomCheck(bool swapNeeded, Slot slot, Slot equipSlot)
        {
            if (swapNeeded)
                (slot, equipSlot) = (equipSlot, slot);

            var slotEquipType = slot.itemInstance.settings_equipment.EquipType;
            var equipSlotEquipType = equipSlot.itemInstance.settings_equipment.EquipType;

            if (slotEquipType == EquipSlotType.Backpack || equipSlotEquipType == EquipSlotType.Backpack)
                return true;

            if (slotEquipType == equipSlotEquipType)
                return false;

            return null;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            var codeMatcher = new CodeMatcher(instructions, generator)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Brfalse_S),
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(OpCodes.Br_S),
                    new CodeMatch(OpCodes.Ldarg_2),
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Ldfld),
                    new CodeMatch(OpCodes.Callvirt),
                    new CodeMatch(OpCodes.Call, MethodInfos.Slot_Equip__GetEquipSlotWithTag),
                    new CodeMatch(OpCodes.Ldnull),
                    new CodeMatch(OpCodes.Call, MethodInfos.UnityEngine_Object__op_Inequality)
                );

            if (codeMatcher.IsInvalid)
            {
                ComfortableInventory.LogError("Inventory.SwitchSlots patch failed");
                return null;
            }

            var localVar = generator.DeclareLocal(typeof(bool?));

            return codeMatcher
                .CreateLabelWithOffsets(10, out var longJump)
                .CreateLabel(out var shortJump)
                .Insert(
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, CustomCheck_MethodInfo),
                    new CodeInstruction(OpCodes.Stloc, localVar),
                    new CodeInstruction(OpCodes.Ldloca, localVar),
                    new CodeInstruction(OpCodes.Call, MethodInfos.Nullable__Bool__HasValue__Getter),
                    new CodeInstruction(OpCodes.Brfalse_S, shortJump),
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Ldloca, localVar),
                    new CodeInstruction(OpCodes.Call, MethodInfos.Nullable__Bool__GetValueOrDefault),
                    new CodeInstruction(OpCodes.Br, longJump)
                )
                .Instructions();
        }
    }
}