using System.Reflection;
using HarmonyLib;
using HMLLibrary;
using kohanis.ComfortableInventory.Patches;
using RaftModLoader;
using UnityEngine;

namespace kohanis.ComfortableInventory
{
    internal class ComfortableInventory : Mod
    {
        private const string HarmonyID = "dev.kohanis.Raft.ComfortableInventory";
        private static readonly Config ExtraSettingsAPI_Settings;

        private static JsonModInfo _modInfo;
        private static Harmony _harmony;

        public void Start()
        {
            _modInfo = modlistEntry.jsonmodinfo;
            _harmony = new Harmony(HarmonyID);

            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Tank__HandleAddFuel__Patch.Patch(_harmony);

            Log("Mod has been loaded!");
        }

        public void OnModUnload()
        {
            _harmony.UnpatchAll(HarmonyID);

            Log("Mod has been unloaded!");
        }

        public static void Log(string message)
        {
            Debug.Log($"[{_modInfo?.name}] : {message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"[{_modInfo?.name}] : {message}");
        }

        [ConsoleCommand("fixBackpack", "fixes backpack glitch when items are in inactive slots.")]
        public static void FixBackpack()
        {
            var inv = RAPI.GetLocalPlayer()?.Inventory;
            if (inv is null)
            {
                Log("this command can only be used with loaded player");
                return;
            }

            foreach (var slot in inv.allSlots)
                if (!slot.active && !slot.IsEmpty)
                    inv.DropItem(slot);
        }
    }
}