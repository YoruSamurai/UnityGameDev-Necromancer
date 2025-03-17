using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviorComponent : ScriptableObject, IEnemyBehaviorComponent
{
    [SerializeField] private string componentName;
    [SerializeField , TextArea] private string componentDesc;

    protected Enemy enemy {  get; private set; }
    protected MonsterStats monsterStats { get; private set; }
    protected Transform playerTransform {  get; private set; }
    public virtual void Initialize(Enemy enemy, Transform playerTransform,MonsterStats monsterStats)
    {
        this.enemy = enemy;
        this.playerTransform = playerTransform;
        this.monsterStats = monsterStats;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public abstract void OnUpdate();

    public virtual void OnAnimationTrigger(EnemyAnimationTriggerType type)
    {

    }
}
