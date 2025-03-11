using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{

    private Transform _playerTransform;
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine,MonsterStats monsterStats) : base(enemy, enemyStateMachine, monsterStats)
    {
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.enemyAttackBaseInstance.DoAnimationTriggerEventLogic(triggerType); 
    }

    public override void EnterState()
    {
        base.EnterState();
        
        enemy.enemyAttackBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.isAttacking = false;
        enemy.enemyAttackBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.enemyAttackBaseInstance.DoUpdateLogic();

    }
}
