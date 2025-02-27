
#region 装备标签
using System;

public enum EquipmentTag
{
    //先制作轻重武器和法杖
    lightMelee,//轻型近战
    heavyMelee,//重型近战
    staff,//法杖
    bleed,//血
    poison,//毒
    burn,//燃烧
    frozen,//冻结
}
#endregion
#region 效果触发条件
public enum BattleTriggerCondition
{
    playerAttack,//玩家发动攻击
    playerHit,//玩家命中怪物
    enemyHit,//怪物命中玩家
    enemyDeath,//怪物死亡
    playerDefense,//怪物命中时 玩家正在防御
    playerBlock,//怪物命中时 玩家招架了
}
#endregion
#region 词条标签
public enum AffixTag
{
    burnAddDmg,//对燃烧人加40%伤害
    bleedAddDmg,//对流血人加40%伤害
    frozenAddDmg,//对冻结人加40%伤害
    poisonAddDmg,//中毒人加40%伤害
    addDmg20,//加20%伤害
    addDmg40,//加40%伤害
}
#endregion
#region 稀有度
public enum Rarity
{
    normal,//普通
    rare,///稀有
}
#endregion

#region 毒类
[Serializable]
public class PoisonEffect // 结构体改为类
{
    public int damage;
    public float timer;
    public float interval;
    public float totalDuration;
    public float elapsedTime;

    public PoisonEffect(int damage, float duration, float interval)
    {
        this.damage = damage;
        this.totalDuration = duration;
        this.timer = 0f;
        this.interval = interval;
        this.elapsedTime = 0f;
    }
}

#endregion

#region 灼烧类
[Serializable]
public class BurnEffect // 结构体改为类
{
    public int damage;
    public float timer;
    public float interval;
    public float totalDuration;
    public float elapsedTime;

    public BurnEffect(int damage, float duration, float interval)
    {
        this.damage = damage;
        this.totalDuration = duration;
        this.timer = 0f;
        this.interval = interval;
        this.elapsedTime = 0f;
    }
}

#endregion

#region 毒类
[Serializable]
public class BleedEffect // 结构体改为类
{
    public int damage;
    public float timer;
    public float interval;
    public float totalDuration;
    public float elapsedTime;

    public BleedEffect(int damage, float duration, float interval)
    {
        this.damage = damage;
        this.totalDuration = duration;
        this.timer = 0f;
        this.interval = interval;
        this.elapsedTime = 0f;
    }
}

#endregion

#region 近战攻击形状枚举
public enum MeleeAttackShapeEnum
{
    rectangle,
    circle
}
#endregion

#region 近战攻击中心点枚举
public enum MeleeAttackCenterEnum
{
    player,
    front,//玩家面前一个半径的位置
}
#endregion

#region 近战攻击结构体
[Serializable]
public struct meleeAttackStruct
{
    public float attackTime;//攻击时间 在这个时间内播放攻击动画 
    public float attackMag;
    public float attackStun;//这里预计是一个晕眩值 也就是耐性条
    public MeleeAttackShapeEnum attackShape;
    public MeleeAttackCenterEnum attackCenter;
    public float attackRadius;
    public float attackLength;
    public float attackWidth;
    public float comboBreakTime;
}
#endregion