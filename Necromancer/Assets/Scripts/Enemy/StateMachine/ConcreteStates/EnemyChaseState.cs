using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private Transform _playerTransform;
    private float _movementSpeed = 2f;

    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("Chase", true);

    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("Chase", false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        Vector2 moveDir = (_playerTransform.position - enemy.transform.position).normalized;
        enemy.MoveEnemy(moveDir * _movementSpeed);

        if(enemy.isWithinStrikingDistance)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
        if (!enemy.isAggroed)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }
    }
}
