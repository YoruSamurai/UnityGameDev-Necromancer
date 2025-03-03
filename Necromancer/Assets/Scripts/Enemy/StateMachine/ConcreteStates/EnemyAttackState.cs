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
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.anim.SetBool("Attack", true);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.anim.SetBool("Attack", false);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        enemy.MoveEnemy(Vector2.zero);
        if(_timer > _timeBetweenShots)
        {
            _timer = 0f;
            Debug.Log("我射");
        }
        _timer += Time.deltaTime;

        if(enemy.isWithinStrikingDistance == false)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
