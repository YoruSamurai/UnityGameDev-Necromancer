using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/MeleeEquipment")]
public class MeleeEquipmentSO : EquipmentSO
{
    [Header("近战武器")]
    public int fullComboAttackTimes;
    public List<MeleeAttackStruct> meleeAttacks;
}
