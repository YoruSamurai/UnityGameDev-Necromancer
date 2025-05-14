using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAffix : MonoBehaviour, IBattleTrigger
{
    [SerializeField] public AffixSO affixSO;

    [Header("SO内容的实例化")]
    [SerializeField] public AffixTag affixTag;
    [SerializeField] public BattleTriggerCondition triggerCondition;
    [SerializeField] public List<EquipmentTag> needEquipmentTags;
    [SerializeField] public List<EquipmentTag> contrastEquipmentTags;
    [SerializeField] public List<AffixTag> contrastAffixTags;
    [SerializeField] public Rarity affixRarity;
    [SerializeField] public string affixDesc;

    // 用于调整伤害倍率的委托
    public Action<float> OnModifyDamageMultiplier;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupAffixBase();

    }



    // Update is called once per frame
    protected virtual void Update()
    {

    }

    private void SetupAffixBase()
    {
        affixTag = affixSO.affixTag;
        triggerCondition = affixSO.triggerCondition;
        needEquipmentTags = affixSO.needEquipmentTags;
        contrastEquipmentTags = affixSO.contrastEquipmentTags;
        affixRarity = affixSO.affixRarity;
        affixDesc = affixSO.affixDesc;
        Debug.Log("装载了" + this.affixDesc);
    }



    public virtual void InvokeAffixPre(PlayerStats playerStats, MonsterStats monsterStats)
    {
        //throw new System.NotImplementedException();
    }

    public virtual void InvokeAffixPost(PlayerStats playerStats, MonsterStats monsterStats, float cMag)
    {
        //throw new System.NotImplementedException();
    }
}
