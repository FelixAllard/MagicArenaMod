using BepInEx;
using HarmonyLib;
namespace MagArenaHack;

[BepInPlugin("FirstPluginMageArena", "Fast Plugin", "1.0.0")]
public class FastPlugin : BaseUnityPlugin
{
    internal static BepInEx.Logging.ManualLogSource Log;
    private Harmony _harmony;
    void Awake()
    {
        Log = Logger;
        _harmony = new Harmony("FirstPluginMageArena");
        _harmony.PatchAll();
        Logger.LogInfo("OverridePatchPlugin loaded and patched.");
    }
    
}