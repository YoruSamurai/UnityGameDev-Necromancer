using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviorComponent : ScriptableObject, IEnemyBehaviorComponent
{
    [SerializeField] private string componentName;
    [SerializeField , TextArea] private string componentDesc;

    protected Enemy enemy {  get; private set; }
    protected Transform playerTransform {  get; private set; }
    public virtual void Initialize(Enemy enemy, Transform playerTransform)
    {
        this.enemy = enemy;
        this.playerTransform = playerTransform;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public abstract void OnUpdate();

    public virtual void OnAnimationTrigger(AnimationTriggerType type)
    {

    }
}
