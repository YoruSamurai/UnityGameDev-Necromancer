using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : EnemyState
{
    public EnemyDieState(Enemy enemy, EnemyStateMachine enemyStateMachine, MonsterStats monsterStats) : base(enemy, enemyStateMachine, monsterStats)
    {
    }

    public override void AnimationTriggerEvent(EnemyAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if(triggerType == EnemyAnimationTriggerType.EnemyDied)
        {
            //销毁敌人
            // 销毁敌人对象
            enemy.Die();

        }
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("Die", true);

    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("Die", false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.SetVelocity(0, enemy.rb.velocity.y);
        
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}
