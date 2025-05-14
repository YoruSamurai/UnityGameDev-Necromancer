using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Battle/AffixList")]
public class AffixListSO : ScriptableObject
{
    [SerializeField] public List<BaseAffix> affixList;
}
