using System.Collections;
using System.Reflection;
using FishNet.Managing;
using FishNet.Object;
using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SharedCheats;

[HarmonyPatch(typeof(MageBookController), "ShootfireballOBs")]
public class ShootfireballOBsPatch
{
    static bool Prefix(
        MageBookController __instance,
        GameObject ownerobj,
        Vector3 fwdVector,
        int level,
        Vector3 spawnpos)
    {
        // Log for debugging
        Main.Log.LogInfo($"[Patch] ShootfireballOBs PREFIX called | owner: {(ownerobj != null ? ownerobj.name : "null")}, level: {level}");

        // Find the internal method that actually sends the network message to clients
        MethodInfo? originalRpcMethod = typeof(MageBookController).GetMethod(
            "RpcWriter___Observers_ShootfireballOBs_3976682022",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        if (originalRpcMethod == null)
        {
            Main.Log.LogError("[Patch] Could not find RpcWriter___Observers_ShootfireballOBs_3976682022!");
            // Let the original method run if we can't patch properly
            return true;
        }

        // Send 10 fireballs with spread to all clients
        for (int i = 0; i < 10; i++)
        {
            Vector3 spreadDir = ApplyRandomSpread(fwdVector, 30f);
            Main.Log.LogInfo($"[Patch] Sending fireball #{i + 1} with spread {spreadDir}");

            try
            {
                originalRpcMethod.Invoke(__instance, new object[] { ownerobj, spreadDir, level, spawnpos });
            }
            catch (Exception ex)
            {
                Main.Log.LogError($"[Patch] Exception sending fireball #{i + 1}: {ex}");
            }
        }

        // Skip the original single call to prevent double firing
        return false;
    }

    private static Vector3 ApplyRandomSpread(Vector3 forward, float maxAngleDegrees)
    {
        Vector3 randomAxis = UnityEngine.Random.onUnitSphere;
        float randomAngle = UnityEngine.Random.Range(-maxAngleDegrees, maxAngleDegrees);
        Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, randomAxis);
        return spreadRotation * forward;
    }

    /*static void Postfix(MageBookController __instance, GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
    {
        Main.Log.LogInfo("ShootFireballServerPatch Postfix called");
        __instance.StartCoroutine(FireballSpamCoroutine(__instance, ownerobj, fwdVector, level, spawnpos));
    }
    private static IEnumerator FireballSpamCoroutine(MageBookController instance, GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
    {
        var method = instance.GetType().GetMethod("RpcWriter___Observers_ShootfireballOBs_3976682022",
            BindingFlags.Instance | BindingFlags.NonPublic);

        if (method == null)
        {
            Main.Log.LogError("Could not find RpcWriter___Observers_ShootfireballOBs_3976682022!");
            yield break;
        }
        
        for (int i = 0; i < 9; i++) // fire 9 shots instantly with spread
        {
            Vector3 spreadDir = ApplyRandomSpread(fwdVector, 30f); // 15 degree max spread
            method.Invoke(instance, new object[] { ownerobj, spreadDir, level, spawnpos });
        }

        yield break; // technically optional
    }
    private static Vector3 ApplyRandomSpread(Vector3 forward, float maxAngleDegrees)
    {
        // Get a random rotation within a cone of maxAngleDegrees
        float angle = UnityEngine.Random.Range(0f, maxAngleDegrees);
        float azimuth = UnityEngine.Random.Range(0f, 360f);

        // Create a quaternion for the spread direction
        Quaternion spreadRotation = Quaternion.AngleAxis(angle, Random.onUnitSphere);
        return spreadRotation * forward;
    }*/

    /*private static IEnumerator FireballSpamCoroutine(MageBookController instance, GameObject ownerobj, Vector3 fwdVector, int level, Vector3 spawnpos)
    {
        // This is the *real* internal method name the game uses to spawn the fireball over network
        var method = instance.GetType().GetMethod("RpcWriter___Server_ShootFireballServer_3976682022",
            BindingFlags.Instance | BindingFlags.NonPublic);

        if (method == null)
        {
            Main.Log.LogError("Could not find RpcWriter___Server_ShootFireballServer_3976682022!");
            yield break;
        }
        //Get spellbook Owner
        MageBookController mageBookOfTheCaster = GetMageBookByOwner(ownerobj);
        
        for (int i = 0; i < 9; i++) // Already fired once in the original method, now add 9 more
        {
            yield return new WaitForSeconds(0.5f);
            method.Invoke(instance, new object[] { ownerobj, fwdVector, level, mageBookOfTheCaster.firePoint.position });
        }
    }
    
    private static MageBookController GetMageBookByOwner(GameObject ownerobj)
    {
        var netObj = ownerobj.GetComponent<NetworkObject>();
        if (netObj == null)
            return null;

        int ownerClientId = netObj.OwnerId;
        
        Main.Log.LogInfo("Found owner id = " + ownerClientId);
        foreach (var controller in GameObject.FindObjectsOfType<PlayerInventory>())
        {
            var netOwner = controller.GetComponent<NetworkObject>();
            if (netOwner != null && netOwner.OwnerId == ownerClientId)
            {
                return controller.MageBook.GetComponent<MageBookController>();
            }
                
        }

        return null;
    }*/

}