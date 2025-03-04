using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Attack/AttackSingle")]
public class Component_AttackSingle : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots; // 设置为 3 秒冷却

    public override void OnEnter()
    {
        _timer = _timeBetweenShots;
    }

    public override void OnUpdate()
    {
        // 保证敌人静止
        enemy.SetVelocity(0, 0);

        // 如果不在攻击中，则保持 Idle 状态，并累加计时器
        if (!enemy.isAttacking)
        {
            enemy.anim.SetBool("Idle", true);
            
        }

        // 当冷却时间结束且不在攻击中时，触发攻击
        if (!enemy.isAttacking && _timer >= _timeBetweenShots)
        {
            _timer = 0f; // 重置计时器
            Debug.Log("触发攻击");
            enemy.anim.SetBool("Attack", true);
            enemy.anim.SetBool("Idle", false);
            enemy.isAttacking = true;
        }
        _timer += Time.deltaTime;
    }

    /// <summary>
    /// 当 AnimationTriggerEvent 调用时，检测是否为攻击结束的触发
    /// </summary>
    public override void OnAnimationTrigger(AnimationTriggerType triggerType)
    {
        if (triggerType == AnimationTriggerType.EnemyAttackEnd)
        {
            // 攻击动画结束，重置攻击状态和动画参数
            enemy.anim.SetBool("Attack", false);
            enemy.isAttacking = false;
            Debug.Log("攻击结束");
        }
    }
}