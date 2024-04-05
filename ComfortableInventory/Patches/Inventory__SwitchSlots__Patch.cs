using System.Collections.Generic;
using System.Linq;
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

        private static bool CustomCheck(Slot slot, Slot equipSlot)
        {
            var slotEquipType = slot.itemInstance.settings_equipment.EquipType;
            var equipSlotEquipType = equipSlot.itemInstance.settings_equipment.EquipType;

            if (slotEquipType == EquipSlotType.Backpack || equipSlotEquipType == EquipSlotType.Backpack)
                return true;

            if (slotEquipType == equipSlotEquipType)
                return false;

            return Slot_Equip.GetEquipSlotWithTag(slotEquipType) != null;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            var count = list.Count - 11;

            for (var i = 0; i < count; i++)
            {
                if (!list[i].IsLdloc() ||
                    list[i + 1].opcode != OpCodes.Brfalse_S ||
                    list[i + 2].opcode != OpCodes.Ldarg_1 ||
                    list[i + 3].opcode != OpCodes.Br_S ||
                    list[i + 4].opcode != OpCodes.Ldarg_2 ||
                    list[i + 5].opcode != OpCodes.Ldfld ||
                    list[i + 6].opcode != OpCodes.Ldfld ||
                    list[i + 7].opcode != OpCodes.Callvirt ||
                    !list[i + 8].Calls(MethodInfos.Slot_Equip__GetEquipSlotWithTag) ||
                    list[i + 9].opcode != OpCodes.Ldnull ||
                    !list[i + 10].Calls(MethodInfos.Object__op_Inequality))
                    continue;

                list[i + 7] = new CodeInstruction(OpCodes.Call, CustomCheck_MethodInfo)
                {
                    labels = list[i + 5].labels
                };
                list[i + 6] = new CodeInstruction(OpCodes.Ldarg_1);
                list[i + 5] = list[i + 4];
                list[i + 4] = list[i + 3];
                list[i + 3] = new CodeInstruction(OpCodes.Ldarg_2);

                list.RemoveRange(i + 8, 3);
                return list;
            }

            ComfortableInventory.LogError("Inventory.SwitchSlots patch failed");
            return null;
        }
    }
}