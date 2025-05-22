using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyProjectileGenerator : MonoBehaviour
{
    private Enemy enemy;
    [SerializeField] protected int baseDamage;  // 攻击伤害
    
    [SerializeField] private EnemyProjectileSO projectileSO;

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

    public void Initialize(Enemy _enemy, int _baseDamage, EnemyProjectileSO _projectileSO)
    {
        enemy = _enemy;
        baseDamage = _baseDamage;
        projectileSO = _projectileSO;
        projectile = projectileSO.projectile;
        projectileNum = projectileSO.projectileNum;

        projectileInterval = projectileSO.projectileInterval;

        StartCoroutine("FireProjectiles");
    }

    public IEnumerator FireProjectiles()
    {
        int projectileNum = projectileSO.projectileNum;
        for(int i = 0; i < projectileNum; i++)
        {
            GameObject obj = Instantiate(
            projectile,
            enemy.shootPosition.position + new Vector3(startPosition.x,startPosition.y,0),
            Quaternion.identity
            );
            EnemyBaseProjectile projectileComponent = obj.GetComponent<EnemyBaseProjectile>();
            projectileComponent.Initialize(enemy, baseDamage, i + 1, projectileSO);
            yield return new WaitForSeconds(projectileInterval);
        }
        Destroy(gameObject);
    }

}
