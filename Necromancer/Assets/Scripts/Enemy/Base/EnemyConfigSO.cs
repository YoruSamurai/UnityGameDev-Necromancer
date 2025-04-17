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

    [Header("属性配置")]
    public EnemyProfileSO EnemyProfile;


    // 新增深拷贝方法
    public EnemyConfigSO DeepCopy()
    {
        var copy = CreateInstance<EnemyConfigSO>();
        copy.IdleBehavior = Instantiate(IdleBehavior);
        copy.ChaseBehavior = Instantiate(ChaseBehavior);
        copy.AttackBehavior = Instantiate(AttackBehavior);
        copy.EnemyProfile = Instantiate(EnemyProfile);
        return copy;
    }
}


public enum EnterCondition
{
    DistancePlus,//距离大于x
    DistanceMinus,//距离小于x
    FrontRaycast,//对前方释放射线
    RadiusDetect,//圆形范围感知
    BackGroundCheck, // 检测敌人后方是否有地面
    None,//无条件
    Flash,//不在同一个平台的话 我们就闪现
}



public enum EnemyStateType
{
    Idle,
    Chase,
    Attack
    // 以后还可以添加其他状态
}

[System.Serializable]
public struct AttackChoice
{
    public EnemyAttackSOBase attackBehavior; // 具体的攻击行为
    public EnterCondition condition; // 切换条件
    public float value; // 此攻击方式适用的最大距离
}
[System.Serializable]
public struct ChaseChoice
{
    public EnemyChaseSOBase attackBehavior; // 具体的攻击行为
    public EnterCondition condition; // 切换条件
    public float value; // 此攻击方式适用的最大距离
}