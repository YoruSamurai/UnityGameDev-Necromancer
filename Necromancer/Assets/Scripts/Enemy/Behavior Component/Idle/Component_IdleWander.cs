using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Idle/IdleWander")]
public class Component_IdleWander : EnemyBehaviorComponent
{
    [SerializeField] private float MovementSpeed;
    public override void OnUpdate()
    {
        // 检测是否碰到墙或者即将掉落，如果是，则翻转
        if (!enemy.IsGroundDetected() || enemy.IsWallDetected())
        {
            enemy.Flip();
        }
        enemy.SetVelocity(enemy.facingDir * MovementSpeed, enemy.rb.velocity.y);
    }
}
    