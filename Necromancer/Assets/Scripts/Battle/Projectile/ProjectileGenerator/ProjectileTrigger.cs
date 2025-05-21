using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileTrigger : MonoBehaviour
{

    private BaseProjectileGenerator generator;

    private Vector2 lastPosition;

    private Vector2 lastGroundHitPoint;

    [SerializeField] private ProjectileHitType projectileHitType;
    [SerializeField] private float aoeRange;
    [SerializeField] public bool groundIsHit;

    [SerializeField] private Transform lastHittedEnemy;

    private CircleCollider2D circleCollider;

    public void Initialize(BaseProjectileGenerator _generator, ProjectileSO _projectileSO)
    {
        generator = _generator;
        projectileHitType = _projectileSO.projectileHitType;
        aoeRange = _projectileSO.aoeRange;
        groundIsHit = _projectileSO.groundIsHit;

        lastPosition = transform.position;

        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (generator.GetIsStick())
            return;
        Vector2 currentPosition = transform.position;
        Vector2 direction = currentPosition - lastPosition;
        float distance = direction.magnitude;

        if (distance > 0.01f)
        {
            LayerMask combinedMask = LayerMask.GetMask("Enemy", "Ground", "Player");
            var hits = OverlapCapsuleArea(lastPosition, currentPosition, circleCollider.radius, combinedMask);
            if (hits.Length > 0)
            {
                HandleTrigger(hits);
            }
        }

        lastPosition = currentPosition;
    }

    /// <summary>
    /// 通过返回值知道发生了什么
    /// 跳出 什么都不做 0
    /// 单体伤害 跳出 1
    /// AOE 跳出 2
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private int FuckEnemy(Transform target)
    {
        List<Transform> targets = new List<Transform>();
        if (projectileHitType == ProjectileHitType.Single)
        {
            if (!generator.CanStillHit()) return 0; // 命中数满了，直接跳出

            if (generator.IsTargetInHittedList(target) 
                && generator.GetProjectileTargetFinderType() == ProjectileTargetFinderType.None) return 0;

            if(target == lastHittedEnemy) return 0;

            targets.Add(target);
            lastHittedEnemy = target;
            //造成伤害 
            generator.OnHit(targets);
            generator.InitializeEffect(targets, transform.position);
            return 1;
        }
        else if (projectileHitType == ProjectileHitType.Aoe)
        {
            if (!generator.CanStillHit()) return 0; // 命中数满了，直接跳出

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aoeRange, LayerMask.GetMask("Enemy"));
            foreach (var col in colliders)
            {
                targets.Add(col.transform);
            }
            //造成伤害 
            Debug.Log(targets.Count + "生成");
            generator.OnHit(targets);
            generator.InitializeEffect(targets, transform.position);
            generator.DestroyProjectile();
            return 2;
        }
        return 0;
    }

    private void HandleTrigger(Collider2D[] hits)
    {
        for(int i = 0; i < hits.Length; i++)
        {
            Collider2D other = hits[i];
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (generator.GetProjectileTargetType() == ProjectileTargetType.Enemy)
                {
                    int index = FuckEnemy(other.transform);
                    if (index == 2)
                        break;
                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (generator.GetProjectileTargetType() == ProjectileTargetType.Player)
                {


                }
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (!groundIsHit)
                {
                    //如果是普通或者贯穿子弹 这时候已经结束了
                    if (generator.projectileDestroyType != ProjectileDestroyType.Distance)
                    {
                        //我们让它Stick3s再毁灭吧
                        generator.Stick(transform, other.transform,lastGroundHitPoint);
                        return;
                    }
                    else if (generator.projectileDestroyType == ProjectileDestroyType.Distance)
                    {
                        return;
                    }
                }
                else
                {
                    if (generator.GetProjectileTargetType() == ProjectileTargetType.Enemy)
                    {
                        int index = FuckEnemy(other.transform);
                        if (index == 2)
                            break;
                    }
                }
            }
            else
            {

            }
        }
    }

    
    private Collider2D[] OverlapCapsuleArea(Vector2 start, Vector2 end, float radius, LayerMask combinedMask)
    {
        List<Collider2D> results = new List<Collider2D>();

        // 计算方向和中心点
        Vector2 direction = end - start;
        float distance = direction.magnitude;
        if (distance < 0.001f) distance = 0.001f;
        Vector2 center = (start + end) / 2f;
        Vector2 boxSize = new Vector2(distance, radius * 2);

        // 添加两端的圆形碰撞
        results.AddRange(Physics2D.OverlapCircleAll(end, radius, combinedMask));

        // 添加中间的矩形碰撞
        Collider2D[] boxHits = Physics2D.OverlapBoxAll(center, boxSize, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, combinedMask);
        results.AddRange(boxHits);

        // 去重
        HashSet<Collider2D> uniqueSet = new HashSet<Collider2D>(results);
        List<Collider2D> nonGroundList = new List<Collider2D>();
        List<Collider2D> groundList = new List<Collider2D>();

        foreach (var col in uniqueSet)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                groundList.Add(col);
            }
            else
            {
                nonGroundList.Add(col);
            }
        }

        // 排序：只排序非 Ground 的部分
        nonGroundList.Sort((a, b) =>
        {
            return a.transform.position.x.CompareTo(b.transform.position.x);
        });

        lastGroundHitPoint = new Vector2(0,0);
        foreach (var groundCol in groundList)
        {
            Vector2 rayOrigin = start;
            Vector2 rayDirection = (end - start).normalized;
            float rayDistance = Vector2.Distance(start, end);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance * 5, 1 << groundCol.gameObject.layer);

            if (hit.collider != null && hit.collider == groundCol)
            {
                Vector2 hitPoint = hit.point;

                // 插入排序：根据 hitPoint.x 插入到 nonGroundList 中
                bool inserted = false;
                for (int i = 0; i < nonGroundList.Count; i++)
                {
                    if (hitPoint.x < nonGroundList[i].transform.position.x)
                    {
                        nonGroundList.Insert(i, groundCol);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                    nonGroundList.Add(groundCol);
                lastGroundHitPoint = hitPoint;

                Debug.Log($"Ground {groundCol.name} 命中点（Raycast）: {hitPoint}");
            }
            else
            {
                Debug.LogWarning($"Ground {groundCol.name} 未被 Raycast 命中，可能方向不对或距离不够");
                nonGroundList.Add(groundCol);
            }
        }


        Debug.Log("我倒要看看");
        // 输出每个命中的碰撞体名称和命中点
        foreach (Collider2D collider in nonGroundList)
        {
            Vector2 hitPoint = collider.ClosestPoint(start);
            Debug.Log($"{collider.name} 命中点: {hitPoint}");
        }

        return nonGroundList.ToArray();
    }


    

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;

        Vector2 currentPosition = transform.position;
        Vector2 direction = currentPosition - lastPosition;
        float distance = direction.magnitude;
        if (distance > 0.01f)
        {
            Vector2 center = (lastPosition + currentPosition) / 2f;
            Vector2 boxSize = new Vector2(distance, circleCollider.radius * 2);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 中间矩形
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // 两端圆形
            Gizmos.matrix = Matrix4x4.identity;
            //Gizmos.DrawWireSphere(lastPosition, circleCollider.radius);
            Gizmos.DrawWireSphere(currentPosition, circleCollider.radius);
        }
    }

    
}