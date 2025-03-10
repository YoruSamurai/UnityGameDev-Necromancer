using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyStateMachine enemyStateMachine;
    protected MonsterStats monsterStats;

    public EnemyState(Enemy enemy, EnemyStateMachine enemyStateMachine, MonsterStats monsterStats)
    {
        this.enemy = enemy;
        this.enemyStateMachine = enemyStateMachine;
        this.monsterStats = monsterStats;
    }

    public virtual void EnterState()
    {

    }

    public virtual void ExitState()
    {

    }

    public virtual void UpdateState()
    {

    }

    public virtual void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {

    }
}
