using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Attack/哥布林跳击")]
public class Component_哥布林跳击 : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots = 2f; // 攻击冷却时间
    [SerializeField] private float jumpHeight = 5f; // 向上的速度
    [SerializeField] private float attackDuration = 0.5f; // 攻击动画持续时间

    public override void OnEnter()
    {
        _timer = _timeBetweenShots - .01f;
    }

    public override void OnUpdate()
    {
        

        if (!enemy.isAttacking)
        {
            // 保证敌人静止
            enemy.SetVelocity(0, 0);
            enemy.anim.SetBool("Idle", true);
        }

        if (!enemy.isAttacking && _timer >= _timeBetweenShots)
        {
            _timer = 0f;
            enemy.isAttacking = true;

            // 计算跳跃方向
            Vector2 playerPos = playerTransform.position;
            Vector2 enemyPos = enemy.transform.position;
            float jumpSpeedX = (playerPos.x - enemyPos.x) / attackDuration;

            // 触发跳跃攻击
            Debug.Log("跳击: 触发攻击");
            enemy.anim.SetBool("Attack", true);
            enemy.anim.SetBool("Idle", false);

            // 设置速度，使其向玩家跳跃
            enemy.SetVelocity(jumpSpeedX * 1.2f, jumpHeight);
        }

        _timer += Time.deltaTime;
    }

    public override void OnAnimationTrigger(AnimationTriggerType triggerType)
    {
        if (triggerType == AnimationTriggerType.EnemyAttackEnd)
        {
            // 攻击动画结束，重置攻击状态和动画参数
            enemy.anim.SetBool("Attack", false);
            enemy.isAttacking = false;
            Debug.Log("跳击: 攻击结束");
            // 设置攻击冷却
            enemy.currentAttackCooldown = enemy.attackCooldown;
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
