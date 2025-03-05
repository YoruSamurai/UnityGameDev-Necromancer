using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Chase/飞行追逐1")]
public class Component_飞行追逐1 : EnemyBehaviorComponent
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float stopThreshold = 5f; // 当距离小于此值时开始减速

    public override void OnUpdate()
    {
        // 计算目标位置：玩家位置加上 Y 轴偏移 1.3f 
        Vector2 targetPos = (Vector2)playerTransform.position + new Vector2(0, 1.3f);
        Vector2 currentPos = enemy.transform.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        // 计算速度缩放因子
        float speedFactor = 1f;
        if (distance < stopThreshold)
        {
            speedFactor = Mathf.Clamp01(distance / stopThreshold);
        }

        // 计算当前速度
        float currentSpeed = _movementSpeed * speedFactor;
        Vector2 direction = (targetPos - currentPos).normalized;
        enemy.SetVelocity(direction.x * currentSpeed, direction.y * currentSpeed);
    }
}
