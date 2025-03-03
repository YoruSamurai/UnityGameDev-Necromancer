using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.enemyIdleBaseInstance.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("Idle", true);
        enemy.enemyIdleBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("Idle", false);
        enemy.enemyIdleBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.enemyIdleBaseInstance.DoUpdateLogic();



    }
}
