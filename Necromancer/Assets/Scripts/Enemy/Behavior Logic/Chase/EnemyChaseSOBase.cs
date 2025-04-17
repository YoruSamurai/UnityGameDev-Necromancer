using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "追逐", menuName = "Enemy Logic/Chase")]
public class EnemyChaseSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected MonsterStats monsterStats;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    protected Player player;
    [SerializeField] private List<EnemyBehaviorComponent> _components;
    // 用于存储每个敌人专用的组件克隆实例
    [SerializeField] private List<EnemyBehaviorComponent> _componentInstances;

    [SerializeField] public bool canNotInterrupt;

    public virtual void Initialize(GameObject gameObject, Enemy enemy, MonsterStats monsterStats)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;
        this.monsterStats = monsterStats;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // 克隆 _components 中的每个组件，生成独立实例
        _componentInstances = new List<EnemyBehaviorComponent>();
        foreach (var comp in _components)
        {
            // 使用 Instantiate 克隆出新的实例
            var clone = Instantiate(comp);
            clone.Initialize(enemy,player, playerTransform, monsterStats);
            _componentInstances.Add(clone);
        }
    }

    public virtual void DoEnterLogic()
    {
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

    public virtual void DoFixedUpdateLogic()
    {
        foreach (var comp in _componentInstances)
            comp.OnFixedUpdate();
    }

    public virtual void DoAnimationTriggerEventLogic(EnemyAnimationTriggerType triggerType)
    {
        foreach (var comp in _componentInstances)
            comp.OnAnimationTrigger(triggerType);
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
