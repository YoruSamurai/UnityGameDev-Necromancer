using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Projectile")]
public class ProjectileSO : ScriptableObject
{
    [Header("基础设置")]
    [SerializeField] public float damageMultiplier;
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float projectileMaxDistance;
    [SerializeField] public float projectileLifetime;
    [SerializeField] public float projectileGravity;
    [SerializeField] public float projectileAngle;
    [SerializeField] public float projectileRadius;
    [SerializeField] public Sprite projectileSprite;




    [Header("子弹初始位置偏移")]
    [SerializeField] public Vector2 projectileOffset;


    [Header("移动类型/打玩家还是敌人/世界空间还是局部空间")]
    [SerializeField] public ProjectileMovingType projectileMovingType;
    [SerializeField] public ProjectileTargetType projectileTargetType;
    [SerializeField] public ProjectileParentType projectileParentType;

    [Header("最大命中多少敌人被摧毁，-1为不在乎")]
    [SerializeField] public int maxHitNum;
    [Header("是否可穿过墙体 可以的话就是距离毁灭 不然就是射到墙上/贯穿X个敌人被毁灭")]
    [SerializeField] public ProjectileDestroyType projectileDestroyType;
    [Header("是否可以回收 回收的话就在Destroy那里处理")]
    [SerializeField] public ProjectileRecoverType projectileRecoverType;

    [Header("效果是在目标身上/子弹位置（炸弹）")]
    [SerializeField] public ProjectileEffectGenerateType projectileEffectGenerateType;

    [Header("单体还是aoe aoe范围")]
    [SerializeField] public ProjectileHitType projectileHitType;
    [SerializeField] public float aoeRange;

    [Header("命中地板也算命中 适合AOE")]
    [SerializeField] public bool groundIsHit;

    [Header("是否可以卡在墙里面？卡多久")]
    [SerializeField] public bool canStickInGround;
    [SerializeField] public float stickTime;

    [Header("追踪类型，最多索敌多少次 索敌范围")]
    [SerializeField] public ProjectileTargetFinderType projectileTargetFinderType;
    [SerializeField] public int targetFinderNum;
    [SerializeField] public float targetFinderRange;

    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public GameObject projectileTrailPrefab;
    [SerializeField] public GameObject projectileHitEffectPrefab;


}
/// <summary>
/// 子弹结束它的射击的类型
/// Normal 正常消亡，命中n次敌人结束他的一生
/// Penetrate 贯穿沿途的所有敌人 只有射到墙上了或者超过一定距离了才结束一生
/// Distance 无视所有一切 只和它的行进距离有关
/// </summary>
public enum ProjectileDestroyType
{
    Normal,
    Penetrate, 
    Distance
}

public enum ProjectileEffectGenerateType
{
    HitPoint,
    Target,
}

public enum ProjectileRecoverType
{
    None,
    Recover
}

public enum ProjectileHitType
{
    Single,
    Aoe
}

public enum ProjectileTargetFinderType
{
    None,
    Closest
}

public enum ProjectileParentType
{
    World,
    Local
}

public enum ProjectileMovingType
{
    Straight,
    A,//没想好是什么 一开始一个莫名其妙的方向 然后转向玩家
    Track,//索敌
}

public enum ProjectileTargetType
{
    Player,
    Enemy,
} 