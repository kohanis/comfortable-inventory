namespace kohanis.ComfortableInventory
{
    public class Config
    {
        internal static bool PreferInventory = true;
        internal static bool PreferHotbarEquipment = true;
        internal static bool PreferHotbarTools = true;
        internal static bool PreferHotbarCookedFood = true;
        internal static bool PreferHotbarFreshWater = true;
        internal static bool PreferHotbarFoodContainers = true;
        internal static bool PreferHotbarHealing = true;
        internal static bool PreferHotbarBuildable = true;


        public static void ExtraSettingsAPI_Load()
        {
            Update();
        }

        public static void ExtraSettingsAPI_SettingsClose()
        {
            Update();
        }

        private static void Update()
        {
            PreferInventory = ExtraSettingsAPI_GetCheckboxState("preferInventory");
            PreferHotbarEquipment = ExtraSettingsAPI_GetCheckboxState("preferHotbarEquipment");
            PreferHotbarTools = ExtraSettingsAPI_GetCheckboxState("preferHotbarTools");
            PreferHotbarCookedFood = ExtraSettingsAPI_GetCheckboxState("preferHotbarCookedFood");
            PreferHotbarFreshWater = ExtraSettingsAPI_GetCheckboxState("PreferHotbarFreshWater");
            PreferHotbarFoodContainers = ExtraSettingsAPI_GetCheckboxState("preferHotbarFoodContainers");
            PreferHotbarHealing = ExtraSettingsAPI_GetCheckboxState("preferHotbarHealing");
            PreferHotbarBuildable = ExtraSettingsAPI_GetCheckboxState("preferHotbarBuildable");
        }

        public static bool ExtraSettingsAPI_GetCheckboxState(string settingName) =>
            true;
    }
}