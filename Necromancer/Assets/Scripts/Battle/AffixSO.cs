using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Battle/Affix")]
public class AffixSO : ScriptableObject
{
    [Header("词缀标签")]
    public AffixTag affixTag;

    [Header("触发条件")]
    public BattleTriggerCondition triggerCondition;

    [Header("需求装备标签")]
    public List<EquipmentTag> needEquipmentTags;

    [Header("冲突的装备标签")]
    public List<EquipmentTag> contrastEquipmentTags;

    [Header("冲突的词缀标签")]
    public List<AffixTag> contrastAffixTags;

    [Header("词缀稀有度")]
    public Rarity affixRarity;

    [Header("词缀描述")]
    public string affixDesc;
}
