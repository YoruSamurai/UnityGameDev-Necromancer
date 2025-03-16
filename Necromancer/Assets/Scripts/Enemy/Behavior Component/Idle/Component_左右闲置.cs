using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Idle/左右闲置")]
public class Component_左右闲置 : EnemyBehaviorComponent
{
    [SerializeField] private float MovementSpeed;

    // 延迟状态相关变量
    private bool isDelaying = false;
    private float delayTimer = 0f;
    [SerializeField] private float delayDuration; // 延迟半秒

    public override void OnUpdate()
    {
        // 如果处于延迟状态，则更新计时器，保持静止，并设置动画参数
        if (isDelaying)
        {
            delayTimer += Time.deltaTime;
            enemy.SetVelocity(0, enemy.rb.velocity.y);
            if (delayTimer >= delayDuration)
            {
                // 延迟结束后执行翻转，并恢复运动
                //Debug.Log("我转" + delayDuration);
                enemy.Flip();
                enemy.anim.SetBool("Idle", false);
                enemy.anim.SetBool("Move", true);
                isDelaying = false;
                delayTimer = 0f;
            }
            return;
        }

        // 检测如果碰到墙或者即将掉落，进入延迟状态
        if (!enemy.IsGroundDetected() || enemy.IsWallDetected())
        {
            isDelaying = true;
            delayTimer = 0f;
            enemy.SetVelocity(0,enemy.rb.velocity.y);
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Move", false);
            return;
        }

        // 正常情况下按照左右移动
        enemy.SetVelocity(enemy.facingDir * MovementSpeed, enemy.rb.velocity.y);
    }
}
