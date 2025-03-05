using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "追逐", menuName = "Enemy Logic/Chase")]
public class EnemyChaseSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    [SerializeField] private List<EnemyBehaviorComponent> _components;
    // 用于存储每个敌人专用的组件克隆实例
    [SerializeField] private List<EnemyBehaviorComponent> _componentInstances;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // 克隆 _components 中的每个组件，生成独立实例
        _componentInstances = new List<EnemyBehaviorComponent>();
        foreach (var comp in _components)
        {
            // 使用 Instantiate 克隆出新的实例
            var clone = Instantiate(comp);
            clone.Initialize(enemy, playerTransform);
            _componentInstances.Add(clone);
        }
    }

    public virtual void DoEnterLogic()
    {
        enemy.anim.SetBool("Chase", true);
        foreach (var comp in _componentInstances)
            comp.OnEnter();
    }

    public virtual void DoExitLogic()
    {
        foreach (var comp in _componentInstances)
            comp.OnExit();
        ResetValues();
    }

    public virtual void DoUpdateLogic()
    {
        foreach (var comp in _componentInstances)
            comp.OnUpdate();
    }

    public virtual void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {

    }

    public virtual void ResetValues()
    {
        foreach (AnimatorControllerParameter parameter in enemy.anim.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                enemy.anim.SetBool(parameter.name, false);
            }
        }
    }
}
