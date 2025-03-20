using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 定向雷轨 : BaseSkillComponent
{
    [Header("魔法阵Prefab")]
    public GameObject magicCirclePrefab;

    public override void Execute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        OnSkillStart(player, skillData);

        if (magicCirclePrefab != null)
        {
            // 根据玩家朝向，稍微偏移生成魔法阵
            Vector3 spawnPos = player.transform.position + new Vector3(player.facingDir * 0.5f, 0, 0);
            // 使用玩家当前的旋转
            GameObject magicCircle = Instantiate(magicCirclePrefab, spawnPos, player.transform.rotation);
            // 获取挂在魔法阵上的脚本并初始化：传入技能数据和玩家朝向
            定向雷轨魔法阵 circleScript = magicCircle.GetComponent<定向雷轨魔法阵>();
            if (circleScript != null)
            {
                circleScript.Initialize(skillData, player.facingDir);
            }
            else
            {
                Debug.LogError("魔法阵Prefab上未找到 DirectionalThunderboltMagicCircle 脚本！");
            }
        }

        OnSkillEnd(player, skillData);
    }
}
