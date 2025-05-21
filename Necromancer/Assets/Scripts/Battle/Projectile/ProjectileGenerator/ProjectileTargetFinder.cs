using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTargetFinder : MonoBehaviour
{

    private BaseProjectileGenerator generator;
    [SerializeField] public ProjectileTargetFinderType projectileTargetFinderType;
    [SerializeField] private int targetFinderNum;
    [SerializeField] private float targetFinderRange;

    public void Initialize(BaseProjectileGenerator _generator,ProjectileSO projectileSO)
    {
        generator = _generator;
        projectileTargetFinderType = projectileSO.projectileTargetFinderType;
        targetFinderNum = projectileSO.targetFinderNum;
        targetFinderRange = projectileSO.targetFinderRange;
    }

    public Transform TryGetNewTransform(Transform target)
    {
        if(projectileTargetFinderType == ProjectileTargetFinderType.None)
        {
            return null;
        }
        else if (projectileTargetFinderType == ProjectileTargetFinderType.Closest)
        {
            if(targetFinderNum <= 0)
            {
                return null;
            }
            targetFinderNum--;
            LayerMask enemyMask = 1 << LayerMask.NameToLayer("Enemy");
            Transform newTarget = FindClosestTarget(target,transform.position, targetFinderRange, enemyMask);
            if (newTarget != null) 
                return newTarget;
            generator.DestroyProjectile();
        }
        return null;
    }

    private Transform FindClosestTarget(Transform target ,Vector2 position, float radius, LayerMask mask)
    {
        Debug.Log(position + " " + radius + " " + mask);
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, mask);
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.transform == target) continue;
            float dist = Vector2.Distance(position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }
        Debug.Log(closest);
        return closest;
    }
}
