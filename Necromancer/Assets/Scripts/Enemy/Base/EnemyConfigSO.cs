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
