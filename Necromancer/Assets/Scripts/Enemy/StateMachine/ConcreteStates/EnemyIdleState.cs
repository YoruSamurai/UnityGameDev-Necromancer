using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine, MonsterStats monsterStats) : base(enemy, enemyStateMachine, monsterStats)
    {
    }

    public override void AnimationTriggerEvent(EnemyAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.enemyIdleBaseInstance.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.enemyIdleBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        
        enemy.enemyIdleBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.enemyIdleBaseInstance.DoUpdateLogic();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        enemy.enemyIdleBaseInstance.DoFixedUpdateLogic();
    }
}
