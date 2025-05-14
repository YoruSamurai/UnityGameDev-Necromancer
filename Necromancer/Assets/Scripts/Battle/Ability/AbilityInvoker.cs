using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInvoker : MonoBehaviour
{


    [SerializeField] private List<BaseAbility> baseAbilityList;

    [SerializeField] private float damageMag;
    [SerializeField] private float reduceMag;

    public void AddAbilityToList(BaseAbility baseAbility)
    {
        baseAbilityList.Add(baseAbility);   
    }
    /// <summary>
    /// 玩家命中敌人的时候触发事件
    /// </summary>
    /// <param name="_baseEquipment"></param>
    /// <param name="_monsterStats"></param>
    /// <returns></returns>
    public float InvokeOnPlayerHit(BaseEquipment _baseEquipment,MonsterStats _monsterStats)
    {
        damageMag = 1f;
        this.TriggerEvent(EventName.OnPlayerHit,new OnPlayerHitEventArgs
        {
            baseEquipment = _baseEquipment,
            monsterStats = _monsterStats
        });
        float x = damageMag;
        damageMag = 1f;
        return x;
    }
    /// <summary>
    /// 玩家被命中的时候触发事件
    /// </summary>
    /// <param name="_baseEquipment"></param>
    /// <param name="_monsterStats"></param>
    /// <returns></returns>
    public float InvokeOnPlayerHitted(MonsterStats _monsterStats)
    {
        reduceMag = 1f;
        this.TriggerEvent(EventName.OnPlayerHitted, new OnPlayerHittedEventArgs
        {
            monsterStats = _monsterStats
        });
        float x = reduceMag;
        reduceMag = 1f;
        return x;
    }

    /// <summary>
    /// 怪物死亡的时候触发事件
    /// </summary>
    /// <param name="_monsterStats"></param>
    public void InvokeOnKillMonster(MonsterStats _monsterStats)
    {
        this.TriggerEvent(EventName.OnEnemyDead, new OnEnemyDeadEventArgs
        {
            monsterStats = _monsterStats
        });
    }

    /// <summary>
    /// 玩家暴击的时候触发事件
    /// </summary>
    /// <param name="_monsterStats"></param>
    public void InvokeOnPlayerCrit(BaseEquipment _baseEquipment, MonsterStats _monsterStats)
    {
        this.TriggerEvent(EventName.OnPlayerCrit, new OnPlayerCritEventArgs
        {
            baseEquipment = _baseEquipment,
            monsterStats = _monsterStats
        });
    }

    /// <summary>
    /// 玩家暴击的时候触发事件
    /// </summary>
    /// <param name="_monsterStats"></param>
    public void InvokeOnPlayerParry(BaseEquipment _baseEquipment, MonsterStats _monsterStats)
    {
        this.TriggerEvent(EventName.OnPlayerParry, new OnPlayerParryEventArgs
        {
            baseEquipment = _baseEquipment,
            monsterStats = _monsterStats
        });
    }

    /// <summary>
    /// 玩家暴击的时候触发事件
    /// </summary>
    /// <param name="_monsterStats"></param>
    public void InvokeOnPlayerCombo(BaseEquipment _baseEquipment, MonsterStats _monsterStats)
    {
        this.TriggerEvent(EventName.OnPlayerCombo, new OnPlayerComboEventArgs
        {
            baseEquipment = _baseEquipment,
            monsterStats = _monsterStats
        });
    }

    public void MultiplyDamageMag(float mag)
    {
        damageMag *= mag;
    }

    public void MultiplyReduceMag(float mag)
    {
        reduceMag *= mag;
    }

}
