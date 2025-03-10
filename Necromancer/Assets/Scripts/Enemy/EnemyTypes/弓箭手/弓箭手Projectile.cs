using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class 弓箭手Projectile : EnemyBaseProjectile
{
    public override void Update()
    {
        base.Update();
        if (hasHit) return;

        // 确保子弹始终有水平速度
        rb.velocity = new Vector2((isFacingRight ? 1f : -1f) * projectileSpeed, rb.velocity.y);

        // 检查是否达到最大飞行距离
        if (Vector2.Distance(startPosition, transform.position) >= projectileMaxDistance)
        {
            Destroy(gameObject);
        }
    }
}
