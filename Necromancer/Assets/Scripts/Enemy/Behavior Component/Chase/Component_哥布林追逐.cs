using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Chase/哥布林追逐")]
public class Component_哥布林追逐 : EnemyBehaviorComponent
{
    [SerializeField] private float _movementSpeed;


    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        // 计算方向
        int moveDirection = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;

        bool canMove = (enemy.IsWallDetected() || !enemy.IsGroundDetected());
        // 如果前方是墙壁或者没有地面，则停止移动
        if (canMove && moveDirection == enemy.facingDir)
        {
            enemy.SetZeroVelocity();
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Chase", false);

        }
        else if (Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < 2f)
        {
            if(moveDirection != enemy.facingDir) 
                enemy.Flip();
            enemy.SetZeroVelocity();
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Chase", false);
        }
        else
        {
            enemy.SetVelocity(moveDirection * _movementSpeed, enemy.rb.velocity.y);
            enemy.anim.SetBool("Chase", true);
            enemy.anim.SetBool("Idle", false);
        }

        
    }
}
