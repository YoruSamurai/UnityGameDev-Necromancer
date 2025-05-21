using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Battle/StaffEquipment")]
public class StaffEquipmentSO : EquipmentSO
{

    [Header("远程武器设置")]
    public int fullComboAttackTimes;//连击次数
    [Header("投射预制体SO")]
    public ProjectileSO projectileSO;   // 弹丸预制体
    public List<StaffAttackStruct> staffAttacks;

}
