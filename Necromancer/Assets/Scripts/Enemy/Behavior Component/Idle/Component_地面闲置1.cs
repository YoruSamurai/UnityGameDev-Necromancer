using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Idle/地面闲置1")]
public class Component_地面闲置1 : EnemyBehaviorComponent
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
    