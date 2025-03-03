using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Vector3 _targetPos;
    private Vector3 _direction;

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("Idle", true);
        _targetPos = GetRandomPointInCircle();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("Idle", false);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (enemy.isAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }

        _direction = (_targetPos - enemy.transform.position).normalized;
        enemy.MoveEnemy(_direction * enemy.RandomMovementSpeed);

        if((enemy.transform.position - _targetPos).sqrMagnitude < 0.01f)
        {
            _targetPos = GetRandomPointInCircle();
        }
    }

    private Vector3 GetRandomPointInCircle()
    {
        return enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * enemy.RandomMovementRange;
    }
}
