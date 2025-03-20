using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 定向雷轨子弹 : BaseSkillProjectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*// 如果碰到敌人，输出命中信息，但不销毁
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log($"闪雷击中了敌人：{collision.name}");
            // 这里可以加伤害逻辑
            return;  // 穿透敌人继续飞行
        }

        // 碰到地形，销毁
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("闪雷撞到了地形！");
            Destroy(gameObject);
        }*/
    }
}
