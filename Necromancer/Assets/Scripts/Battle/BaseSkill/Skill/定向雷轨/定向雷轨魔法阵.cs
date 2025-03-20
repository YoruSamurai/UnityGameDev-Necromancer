using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 定向雷轨魔法阵 : MonoBehaviour
{
    [Header("闪雷参数")]
    public GameObject thunderBoltPrefab;  // 闪雷投射物Prefab
    public float fireRate = 1f;           // 每隔多少秒发射一道
    public float duration = 5f;           // 魔法阵持续时间

    // 投射物的参数，这里从SkillSO中获取
    private float projectileSpeed;
    private float projectileMaxDistance;
    private float projectileMaxTimer;
    private float projectileGravity;
    private float projectileAngle;
    private int facingDir;  // 根据玩家当时的朝向

    private float timer;

    /// <summary>
    /// 初始化魔法阵组件（由定向雷轨技能调用）
    /// </summary>
    public void Initialize(SkillSO skillData, int playerFacingDir)
    {
        // 从SkillSO中读取投射物相关参数
        projectileSpeed = skillData.projectileSpeed;
        projectileMaxDistance = skillData.projectileMaxDistance;
        projectileMaxTimer = skillData.projectileMaxTimer;
        projectileGravity = skillData.projectileGravity;
        projectileAngle = skillData.projectileAngle;
        facingDir = playerFacingDir;  // 保存玩家当时的面向

        // 开始协程发射闪雷
        StartCoroutine(FireThunderBolts());
    }

    private IEnumerator FireThunderBolts()
    {
        timer = 0f;
        while (timer < duration)
        {
            FireThunderBolt();
            yield return new WaitForSeconds(fireRate);
            timer += fireRate;
        }
        Destroy(gameObject);  // 持续时间结束后销毁魔法阵
    }

    private void FireThunderBolt()
    {
        if (thunderBoltPrefab != null)
        {
            // 闪雷从魔法阵的位置发射
            Vector2 spawnPos = transform.position;
            // 使用魔法阵当前的旋转，也可以自行调整旋转
            Quaternion spawnRotation = transform.rotation;

            GameObject bolt = Instantiate(thunderBoltPrefab, spawnPos, spawnRotation);
            // 获取 BaseSkillProjectile 组件，并传递参数
            BaseSkillProjectile projectile = bolt.GetComponent<BaseSkillProjectile>();
            if (projectile != null)
            {
                projectile.Initialize(
                    projectileSpeed,
                    projectileMaxDistance,
                    projectileMaxTimer,
                    projectileGravity,
                    projectileAngle,
                    facingDir > 0  // 如果 facingDir > 0 则为右侧
                );
            }
        }
    }
}