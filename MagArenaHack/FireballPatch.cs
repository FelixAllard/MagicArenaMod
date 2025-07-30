using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace MagArenaHack;
[HarmonyPatch(typeof(MageBookController), nameof(MageBookController.Fireball))]
public class FireballPatch
{

    static void Postfix(MageBookController __instance, GameObject ownerobj, int level)
    {
        FastPlugin.Log.LogInfo("FireballPatch Postfix called");

        __instance.StartCoroutine(FireballSpamCoroutine(__instance, ownerobj, level));
    }

    private static IEnumerator FireballSpamCoroutine(MageBookController instance, GameObject owner, int level)
    {
        var type = instance.GetType();
        var method = type.GetMethod("ShootFireballServer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method == null)
        {
            FastPlugin.Log.LogError("Could not find ShootFireballServer method!");
            yield break;
        }

        for (int i = 0; i < 10; i++)
        {
            method.Invoke(instance, new object[] { owner, Camera.main.transform.forward, level, instance.firePoint.position });
            yield return new WaitForSeconds(0.5f);
        }
    }

    private static void CallShootFireballServer(object instance, GameObject owner, Vector3 forward, int level, Vector3 position)
    {
        var type = instance.GetType();
        var method = type.GetMethod("ShootFireballServer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(instance, new object[] { owner, forward, level, position });
        }
        else
        {
            FastPlugin.Log.LogError("Could not find ShootFireballServer method!");
        }
    }
}