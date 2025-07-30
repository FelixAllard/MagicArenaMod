using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace MagArenaHack;

[HarmonyPatch(typeof(MageBookController), nameof(MageBookController.Frostbolt))]
public class FrostboltPatch
{
    static void Postfix(MageBookController __instance, GameObject ownerobj, int level)
    {
        FastPlugin.Log.LogInfo("FrostboltPatch Postfix called");
        __instance.StartCoroutine(SpamFrostbolts(__instance, ownerobj, level, 10, 0.1f));
    }

    private static IEnumerator SpamFrostbolts(MageBookController instance, GameObject owner, int level, int count, float delay)
    {
        var method = instance.GetType().GetMethod("ShootFrostboltServer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method == null)
        {
            FastPlugin.Log.LogError("Could not find ShootFrostboltServer method!");
            yield break;
        }

        
        Vector3 forward = Camera.main.transform.forward;
        for (int i = 0; i < count; i++){
            forward = Camera.main.transform.forward;
        
            method.Invoke(instance, new object[] { owner, forward, level });
            yield return new WaitForSeconds(delay);
        }
    }
}