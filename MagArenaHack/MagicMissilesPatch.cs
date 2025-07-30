using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MagArenaHack;

[HarmonyPatch(typeof(MageBookController), "CastWard")]
public class CastWardPatch
{
    static void Postfix(MageBookController __instance, GameObject ownerobj, int level)
    {
        GameObject target = null;

        // Match original targeting logic
        Collider[] colliderArray = Physics.OverlapSphere(__instance.transform.position, 60f, (int)__instance.playerlayer);
        float bestScore = float.MaxValue;

        foreach (Collider collider in colliderArray)
        {
            Vector3 toTarget = collider.transform.position - __instance.transform.position;
            float distance = toTarget.magnitude;
            float angle = Vector3.Angle(Camera.main.transform.forward, toTarget.normalized);

            if (angle > 90f) continue;

            if (collider.TryGetComponent<PlayerMovement>(out var playerMov))
            {
                if (playerMov.playerTeam != ownerobj.GetComponent<PlayerMovement>().playerTeam)
                {
                    float score = distance + angle * 0.5f;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        target = playerMov.gameObject;
                    }
                }
            }
            else if (collider.TryGetComponent<GetPlayerGameobject>(out var getPlayer))
            {
                var pm = getPlayer.player.GetComponent<PlayerMovement>();
                if (pm != null && pm.playerTeam != ownerobj.GetComponent<PlayerMovement>().playerTeam)
                {
                    float score = distance + angle * 0.5f;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        target = getPlayer.player;
                    }
                }
            }
            else if (collider.TryGetComponent<GetShadowWizardController>(out var shadow))
            {
                float score = distance + angle * 0.5f;
                if (score < bestScore)
                {
                    bestScore = score;
                    target = shadow.ShadowWizardAI;
                }
            }
        }

        // Get private server method
        MethodInfo shootMethod = AccessTools.Method(typeof(MageBookController), "ShootMagicMissleServer");

// Create runner object for coroutine
        GameObject runner = new GameObject("SpellRepeater");
        GameObject.DontDestroyOnLoad(runner);
        var repeater = runner.AddComponent<SpellRepeater>();

// Start the coroutine shooting logic
        repeater.StartShootRoutine(__instance, ownerobj, target, level, shootMethod);
    }
}