using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyState
{
    public EnemyStunState(Enemy enemy, EnemyStateMachine enemyStateMachine, MonsterStats monsterStats) : base(enemy, enemyStateMachine, monsterStats)
    {
    }

    public override void AnimationTriggerEvent(EnemyAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("Stun" , true);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("Stun", false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(monsterStats.currentStunTimer <= 0)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        // 如果正在被击退，则跳过正常移动逻辑
        if (enemy.isKnockBack)
        {
            return;
        }
        enemy.SetVelocity(0,enemy.rb.velocity.y);
    }
}
