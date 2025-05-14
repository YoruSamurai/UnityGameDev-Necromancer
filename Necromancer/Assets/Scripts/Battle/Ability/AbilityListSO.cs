using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Battle/AbilityList")]
public class AbilityListSO : ScriptableObject
{
    [SerializeField] public List<BaseAbility> abilityList;
}
