using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Idle/哥布林闲置转换")]
public class TComponent_哥布林闲置转换 : EnemyBehaviorComponent
{
    [SerializeField] private float distance;

    public override void OnUpdate()
    {
        // 射线起点为敌人当前位置
        Vector2 origin = enemy.transform.position;
        // 射线方向为敌人正前方（根据 facingDir 判断左右方向）
        Vector2 direction = Vector2.right * enemy.facingDir;

        // 执行射线检测
        RaycastHit2D[] hit = Physics2D.RaycastAll(origin, direction, distance);

        // 可选：绘制调试射线
        Debug.DrawRay(origin, direction * distance, Color.red);

        // 如果射线检测到对象并且该对象标签为 "Player"，则切换状态
        foreach(var it in hit)
        {
            if (it.collider != null && it.collider.CompareTag("Player"))
            {
                enemy.stateMachine.ChangeState(enemy.attackState);
            }
        }
        
    }
}
