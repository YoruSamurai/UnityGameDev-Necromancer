using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/EquipmentList")]
public class EquipmentListSO : ScriptableObject
{
    [SerializeField] public List<BaseEquipment> equipmentList;
}
