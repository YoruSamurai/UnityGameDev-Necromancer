using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{

    private Transform _playerTransform;
    private float _timer;
    private float _timeBetweenShots = 2f;
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
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
        
        enemy.enemyAttackBaseInstance.DoExitLogic();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        enemy.enemyAttackBaseInstance.DoUpdateLogic();

    }
}
