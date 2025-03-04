using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    [SerializeField] private List<EnemyBehaviorComponent> _components;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (var comp in _components)
            comp.Initialize(enemy, playerTransform);
    }

    public virtual void DoEnterLogic()
    {
        foreach (var comp in _components)
            comp.OnEnter();
    }

    public virtual void DoExitLogic()
    {
        foreach (var comp in _components)
            comp.OnExit();
        ResetValues();
    }

    public virtual void DoUpdateLogic()
    {
        foreach (var comp in _components)
            comp.OnUpdate();
    }

    public virtual void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {

    }

    public virtual void ResetValues()
    {

    }
}
