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

    [Header("装备类型")]
    public EquipmentType equipmentType;

    [Header("装备标签")]
    public List<EquipmentTag> equipmentTags;

    [Header("装备基础暴击几率")]
    public int baseCritChance;

    [Header("装备暴击伤害倍率")]
    public float critMag;

    [Header("装备描述")]
    [SerializeField, TextArea] public string equipmentDesc;


    [Header("动画配置")]
    public AnimatorOverrideController attackAnimator;  // 武器专属动画覆盖控制器
    public AnimationClip[] comboAnimations;            // 连击动画片段数组

    [Header("刀光特效动画配置")]
    public AnimationClip[] slashAnimations; // 刀光动画数组
    public Vector2[] slashOffsets;          // 每个刀光动画对应的生成位置偏移

}
