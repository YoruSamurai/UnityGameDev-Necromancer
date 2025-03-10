using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine, MonsterStats monsterStats) : base(enemy, enemyStateMachine, monsterStats)
    {
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.enemyChaseBaseInstance.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        
        enemy.enemyChaseBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        
        enemy.enemyChaseBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.enemyChaseBaseInstance.DoUpdateLogic();


    }
}
