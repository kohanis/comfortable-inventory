namespace kohanis.ComfortableInventory
{
    internal static class Extensions
    {
        internal static bool IsTool(this Item_Base self)
        {
            var craftingCategory = self.settings_recipe.CraftingCategory;
            return craftingCategory == CraftingCategory.Tools || craftingCategory == CraftingCategory.Weapons ||
                   (craftingCategory != CraftingCategory.Resources && self.MaxUses >= 10 &&
                    self.settings_equipment.EquipType == EquipSlotType.None &&
                    self.settings_consumeable.FoodForm == FoodForm.None);
        }

        internal static bool IsCookedFood(this Item_Base self)
        {
            return self.settings_consumeable.FoodForm != FoodForm.None && self.settings_Inventory.StackSize <= 5;
        }
    }
}