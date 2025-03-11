using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Battle/ShieldEquipment")]
public class ShieldEquipmentSO : EquipmentSO
{
    [Header("盾牌武器")]
    public int fullComboAttackTimes;
    public List<ShieldAttackStruct> shieldAttacks;
    public float holdThreshold; // 长按阈值

    [Header("防御动画配置")]
    public AnimationClip[] defenseAnimations;            // 连击动画片段数组
}
