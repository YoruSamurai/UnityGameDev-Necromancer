using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Equipment")]
public class EquipmentSO : ScriptableObject
{
    [Header("装备名称")]
    public string equipmentName;

    [Header("装备基础伤害")]
    public int baseDmg;

    [Header("装备图片")]
    public Sprite equipmentSprite;

    [Header("装备标签")]
    public List<EquipmentTag> equipmentTags;

    [Header("装备基础暴击几率")]
    public int baseCritChance;

    [Header("装备暴击伤害倍率")]
    public float critMag;

    [Header("装备描述")]
    [SerializeField, TextArea] public string equipmentDesc;

}
