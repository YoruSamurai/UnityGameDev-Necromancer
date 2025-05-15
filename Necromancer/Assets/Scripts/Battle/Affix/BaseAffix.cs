using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAffix : MonoBehaviour
{
    [SerializeField] public AffixSO affixSO;

    [Header("SO内容的实例化")]
    [SerializeField] public AffixTag affixTag;
    [SerializeField] public List<EquipmentTag> needEquipmentTags;
    [SerializeField] public List<EquipmentTag> contrastEquipmentTags;
    [SerializeField] public List<AffixTag> contrastAffixTags;
    [SerializeField] public Rarity affixRarity;
    [SerializeField] public string affixDesc;

    [SerializeField] protected BaseEquipment baseEquipment;

    protected virtual void Awake()
    {

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    public virtual void Initialize(BaseEquipment _baseEquipment)
    {
        affixTag = affixSO.affixTag;
        needEquipmentTags = affixSO.needEquipmentTags;
        contrastEquipmentTags = affixSO.contrastEquipmentTags;
        affixRarity = affixSO.affixRarity;
        affixDesc = affixSO.affixDesc;
        baseEquipment = _baseEquipment;
        Debug.Log("装载了" + this.affixDesc);

    }

    public virtual void OnEquip()
    {

    }

    public virtual void OnUnequip()
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
