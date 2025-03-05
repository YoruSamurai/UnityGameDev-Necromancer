using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Enemy/Config")]
public class EnemyConfigSO : ScriptableObject
{
    [Header("状态配置")]
    public EnemyIdleSOBase IdleBehavior;
    public EnemyChaseSOBase ChaseBehavior;
    public EnemyAttackSOBase AttackBehavior;

    // 新增深拷贝方法
    public EnemyConfigSO DeepCopy()
    {
        var copy = CreateInstance<EnemyConfigSO>();
        copy.IdleBehavior = Instantiate(IdleBehavior);
        copy.ChaseBehavior = Instantiate(ChaseBehavior);
        copy.AttackBehavior = Instantiate(AttackBehavior);
        return copy;
    }
}


public enum EnterCondition
{
    DistancePlus,//距离大于x
    DistanceMinus,//距离小于x
}

[System.Serializable]
public struct AttackChoice
{
    public EnemyAttackSOBase attackBehavior; // 具体的攻击行为
    public EnterCondition condition; // 此攻击方式适用的最小距离
    public float distance; // 此攻击方式适用的最大距离
}