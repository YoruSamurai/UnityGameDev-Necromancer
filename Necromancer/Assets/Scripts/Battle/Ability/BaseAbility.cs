using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAbility : MonoBehaviour
{
    
    protected AbilityInvoker abilityInvoker;
    [SerializeField] private AbilitySO abilitySO;

    [Header("能力ID")]
    public int abilityID;

    [Header("能力名称")]
    public string abilityName;

    [Header("能力图标")]
    public Sprite abilitySprite;

    [Header("能力描述")]
    public string abilityDesc;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        abilityInvoker = GetComponentInParent<AbilityInvoker>();
        abilityInvoker.AddAbilityToList(this);
        abilityID = abilitySO.abilityID;
        abilityName = abilitySO.abilityName;
        abilityDesc = abilitySO.abilityDesc;

        Debug.Log("abilityInvoker" + abilityInvoker);
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {

    }
    /// <summary>
    /// 玩家攻击命中敌人
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void InvokeOnPlayerHit(object sender, EventArgs e)
    {
        
    }
    /// <summary>
    /// 玩家被敌人攻击命中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void InvokeOnPlayerHitted(object sender, EventArgs e)
    {

    }
    /// <summary>
    /// 玩家击杀敌人
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void InvokeOnKillMonster(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 玩家攻击暴击了
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void InvokeOnPlayerCrit(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 玩家招架成功
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void InvokeOnPlayerParry(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 玩家正在连段
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void InvokeOnPlayerCombo(object sender, EventArgs e)
    {

    }



}
