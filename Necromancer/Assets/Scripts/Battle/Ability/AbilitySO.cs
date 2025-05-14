using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Battle/Ability")]
public class AbilitySO : ScriptableObject
{
    [Header("能力ID")]
    public int abilityID;

    [Header("能力名称")]
    public string abilityName;

    [Header("能力图标")]
    public Sprite abilitySprite;

    [Header("能力描述")]
    public string abilityDesc;
}
