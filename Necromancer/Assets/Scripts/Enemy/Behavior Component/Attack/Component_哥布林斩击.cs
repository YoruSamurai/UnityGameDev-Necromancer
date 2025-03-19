using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Attack/哥布林斩击")]
public class Component_哥布林斩击 : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots = 2f; // 攻击冷却时间
    [SerializeField] private float damageMultiplier;  // 攻击伤害倍率

    public override void OnEnter()
    {
        _timer = _timeBetweenShots - .01f;
        enemy.currentDamageMultiplier = damageMultiplier;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // 保证敌人静止
        enemy.SetVelocity(0, 0);
    }

    public override void OnUpdate()
    {

        if (!enemy.isAttacking)
        {
            enemy.anim.SetBool("Idle", true);
        }

        if (!enemy.isAttacking && _timer >= _timeBetweenShots)
        {
            _timer = 0f;
            Debug.Log("斩击: 触发攻击");
            enemy.anim.SetBool("Attack", true);
            enemy.anim.SetBool("Idle", false);
            enemy.isAttacking = true;
        }
        _timer += Time.deltaTime;
    }

    public override void OnAnimationTrigger(EnemyAnimationTriggerType triggerType)
    {
        if (triggerType == EnemyAnimationTriggerType.EnemyAttackEnd)
        {
            // 攻击动画结束，重置攻击状态和动画参数
            enemy.anim.SetBool("Attack", false);
            enemy.isAttacking = false;
            Debug.Log("斩击: 攻击结束");
            // 设置攻击冷却
            enemy.currentAttackCooldown = enemy.attackCooldown;
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
        if(triggerType == EnemyAnimationTriggerType.EnemyHitDetermineStart)
        {
            Debug.Log("开始伤害判定咯");
        }
        if(triggerType == EnemyAnimationTriggerType.EnemyHitDetermineEnd)
        {
            Debug.Log("结束伤害判定咯");
        }
    }
}
