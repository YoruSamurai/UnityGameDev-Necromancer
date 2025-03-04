using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Chase/DirectToPlayer")]
public class Component_ChaseDirectToPlayer : EnemyBehaviorComponent
{
    [SerializeField] private float _movementSpeed;
    public override void OnUpdate()
    {
        // 计算方向
        int moveDirection = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;

        bool canMove = (enemy.IsWallDetected() || !enemy.IsGroundDetected());
        // 如果前方是墙壁或者没有地面，则停止移动
        if (canMove && moveDirection == enemy.facingDir)
        {
            enemy.SetZeroVelocity();

        }
        else if (Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < .1f)
        {

            enemy.SetZeroVelocity();
        }
        else
        {
            enemy.SetVelocity(moveDirection * _movementSpeed, enemy.rb.velocity.y);
        }
    }
}
