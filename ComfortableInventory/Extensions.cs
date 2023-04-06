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

        internal static bool IsEquipment(this Item_Base self) =>
            self.settings_equipment.EquipType != EquipSlotType.None;

        internal static bool IsCookedFood(this Item_Base self)
        {
            // There's no better way
            var uniqueName = self.UniqueName;
            return self.settings_consumeable.FoodType == FoodType.Food &&
                   (uniqueName.StartsWith("Cooked_") || uniqueName.StartsWith("Claybowl_") ||
                    uniqueName.StartsWith("ClayPlate_") || uniqueName.StartsWith("DrinkingGlass_"));
        }

        internal static bool IsFreshWater(this Item_Base self) =>
            self.settings_consumeable.FoodType == FoodType.Water;

        internal static bool IsFoodContainers(this Item_Base self)
        {
            var consumable = self.settings_consumeable;
            return consumable.FoodType == FoodType.None && consumable.FoodForm != FoodForm.None;
        }
    }
}