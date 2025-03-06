using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Chase/弓箭手翻滚")]
public class Component_弓箭手翻滚 : EnemyBehaviorComponent
{
    [SerializeField] private float rollSpeed = 5f; // 翻滚时的移动速度


    // 标记是否已经开始翻滚
    private bool rollStarted = false;

    public override void OnEnter()
    {
        // 计算翻滚方向：与玩家所在方向相反
        int rollDirection = playerTransform.position.x > enemy.transform.position.x ? -1 : 1;

        // 如果当前朝向不正确，则翻转（注意 Flip 会更新 facingDir）
        if (rollDirection != enemy.facingDir)
        {
            enemy.Flip();
        }

        enemy.anim.SetBool("Idle", false);
        enemy.anim.SetBool("Chase", true);

        // 开始翻滚：设置翻滚速度，Y 方向保持不变（或者你可以调整为符合实际需求）
        enemy.SetVelocity(rollDirection * rollSpeed, enemy.rb.velocity.y);
        rollStarted = true;
    }

    public override void OnUpdate()
    {
        // 在翻滚期间，可以确保敌人以固定速度保持翻滚
        if (rollStarted)
        {
            // 可选：不断重设速度，确保翻滚不会被外界因素打断
            int rollDirection = enemy.facingDir;
            enemy.SetVelocity(rollDirection * rollSpeed, enemy.rb.velocity.y);
        }
    }

    public override void OnAnimationTrigger(AnimationTriggerType triggerType)
    {
        // 假设在翻滚动画的最后一帧，我们通过动画事件发送了一个 EnemyRollEnd 触发器
        if (triggerType == AnimationTriggerType.EnemyRollEnd)
        {
            // 翻滚结束后，重置翻滚动画参数
            enemy.anim.SetBool("Chase", false);
            rollStarted = false;
            // 切换到攻击状态
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }
}
