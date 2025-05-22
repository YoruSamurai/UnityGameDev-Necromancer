using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDestructionHandler : MonoBehaviour
{

    private BaseProjectileGenerator generator;

    public void Initialize(BaseProjectileGenerator _generator)
    {
        generator = _generator;
    }

    /// <summary>
    /// 毁灭吧
    /// </summary>
    public void DestructionHandle(GameObject projectilePrefab, GameObject projectileTrailPrefab)
    {
        Destroy(projectilePrefab);
        Destroy(projectileTrailPrefab);

        Debug.Log("战战战我杀杀杀");
        ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.Projectiles);
    }

}
