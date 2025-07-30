using System.Collections;
using System.Reflection;
using UnityEngine;

namespace MagArenaHack;

public class SpellRepeater : MonoBehaviour
{
    public void StartShootRoutine(MageBookController controller, GameObject ownerobj, GameObject target, int level, MethodInfo shootMethod)
    {
        StartCoroutine(ShootRoutine(controller, ownerobj, target, level, shootMethod));
    }

    private IEnumerator ShootRoutine(MageBookController controller, GameObject ownerobj, GameObject target, int level, MethodInfo shootMethod)
    {
        for (int wave = 0; wave < 3; wave++)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y); // keep it mostly upward

                shootMethod.Invoke(controller, new object[]
                {
                    ownerobj,
                    randomDirection,
                    target,
                    level
                });

                Debug.Log("[Fast Plugin] Shot Additional " + (wave * 10 + i + 1));
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
