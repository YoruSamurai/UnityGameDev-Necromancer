using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//在这个脚本中，我们配置敌人的基础属性

[CreateAssetMenu(menuName = "Enemy/EnemyProfile")]
public class EnemyProfileSO : ScriptableObject
{

    [Header("生命值")]
    public int maxHealth;
    [Header("基础伤害值")]
    public int baseDamage;
    [Header("眩晕抵抗值")]
    public int stunResistance;
    [Header("冻结抵抗值")]
    public int freezeResistance;
    [Header("眩晕时长")]
    public float stunDuration;
    [Header("冻结时长")]
    public float freezeDuration;
    [Header("怪物类型")]
    public EnemyType enemyType;


}

public enum EnemyType
{
    Melee,//近战怪物
    Archer,//远程怪物
    Air,//飞行怪物
}
