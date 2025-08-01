using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SharedCheats
{
    [BepInPlugin("SharedCheats", "Fast Plugin", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource Log;
        private Harmony _harmony;
        void Awake()
        {
            Log = Logger;
            _harmony = new Harmony("SharedCheats");
            _harmony.PatchAll();
            Logger.LogInfo("OverridePatchPlugin loaded and patched.");
        }
    }
}