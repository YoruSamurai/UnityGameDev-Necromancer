using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//对一个技能的子弹进行配置 
[CreateAssetMenu(menuName = "Enemy/Projectile")]
public class EnemyProjectileSO : ScriptableObject
{
    [SerializeField] public float damageMultiplier;  // 攻击伤害倍率
    [SerializeField] public float projectileSpeed;
    [SerializeField] public float projectileMaxDistance;
    [SerializeField] public float projectileMaxTimer;
    [SerializeField] public float projectileGravity;
    [SerializeField] public float projectileAngle;

    [SerializeField] public GameObject projectile;

    [SerializeField] public Vector2 startPosition;
    [SerializeField] public int projectileNum;
    [SerializeField] public float projectileInterval;
    [SerializeField] public ProjectileMovingType movingType;
}
