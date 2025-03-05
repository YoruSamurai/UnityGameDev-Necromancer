using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Idle/飞行闲置1")]
public class Component_飞行闲置1 : EnemyBehaviorComponent
{
    [SerializeField] private float cycleTime; // 每 5 秒切换方向
    [SerializeField] private float speed;      // 水平飞行速度

    private float _timer;

    public override void OnEnter()
    {
        _timer = 0f;
    }

    public override void OnUpdate()
    {
        // 累加时间
        _timer += Time.deltaTime;

        // 如果检测到墙壁，立即翻转方向并重置计时器
        if (enemy.IsWallDetected())
        {
            enemy.Flip();
            _timer = 0f;
        }

        // 当达到周期时，自动翻转方向并重置计时器
        if (_timer >= cycleTime)
        {
            enemy.Flip();
            _timer = 0f;
        }

        // 设置水平飞行速度，不改变垂直速度
        enemy.SetVelocity(enemy.facingDir * speed, enemy.rb.velocity.y);
    }

    public override void OnExit()
    {
        _timer = 0f;
    }
}
