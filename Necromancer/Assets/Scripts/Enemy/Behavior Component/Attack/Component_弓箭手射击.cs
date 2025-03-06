using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Attack/弓箭手射击")]
public class Component_弓箭手射击 : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots = 2f; // 攻击冷却时间

    public override void OnEnter()
    {
        _timer = _timeBetweenShots - .01f;
    }

    public override void OnUpdate()
    {
        // 保证敌人静止
        enemy.SetVelocity(0, 0);

        if (!enemy.isAttacking)
        {
            enemy.anim.SetBool("Idle", true);
        }

        // 当累计时间达到攻击冷却时间时，检查面向，然后触发攻击
        if (!enemy.isAttacking && _timer >= _timeBetweenShots)
        {
            // 调整面向：如果玩家在右侧但敌人面向左侧，或玩家在左侧但敌人面向右侧，则翻转
            float diff = playerTransform.position.x - enemy.transform.position.x;
            if (diff > 0 && enemy.facingDir < 0)
            {
                enemy.Flip();
            }
            else if (diff < 0 && enemy.facingDir > 0)
            {
                enemy.Flip();
            }

            _timer = 0f;
            Debug.Log("弓箭手射击: 触发攻击");
            enemy.anim.SetBool("Attack", true);
            enemy.anim.SetBool("Idle", false);
            enemy.isAttacking = true;
        }
        _timer += Time.deltaTime;
    }

    public override void OnAnimationTrigger(AnimationTriggerType triggerType)
    {
        if (triggerType == AnimationTriggerType.EnemyAttackEnd)
        {
            // 攻击动画结束，重置攻击状态和动画参数
            enemy.anim.SetBool("Attack", false);
            enemy.isAttacking = false;
            Debug.Log("射击: 攻击结束");
            // 设置攻击冷却
            enemy.currentAttackCooldown = enemy.attackCooldown;
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
