using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkillComponent : BaseSkillComponent
{

    public override void Execute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        OnSkillStart(player, skillData);

        if (skillData.projectilePrefab != null)
        {
            // 生成投射物
            GameObject projectileObj = Instantiate(skillData.projectilePrefab, player.transform.position, skillData.projectilePrefab.transform.rotation);// 保留原本旋转
            BaseSkillProjectile projectile = projectileObj.GetComponent<BaseSkillProjectile>();

            // 如果有BaseSkillProjectile组件，就初始化
            if (projectile != null)
            {
                projectile.Initialize(
                    skillData.projectileSpeed,
                    skillData.projectileMaxDistance,
                    skillData.projectileMaxTimer,
                    skillData.projectileGravity,
                    skillData.projectileAngle,
                    player.facingDir > 0
                );

                Debug.Log($"{skillData.skillName} 发射了投射物！");
            }
            else
            {
                Debug.LogError("生成的投射物缺少 BaseSkillProjectile 组件！");
            }
        }
        OnSkillEnd(player, skillData);
    }

}
