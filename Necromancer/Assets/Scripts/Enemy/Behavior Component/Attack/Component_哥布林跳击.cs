using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Attack/哥布林跳击")]
public class Component_哥布林跳击 : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots = 2f;  // 攻击冷却时间
    [SerializeField] private float damageMultiplier;  // 攻击伤害倍率

    [SerializeField] private float preAttackDelay = 0.3f;     // 攻击前摇延迟
    [SerializeField] private float jumpHeight = 5f;           // 向上的速度
    [SerializeField] private float attackDuration = 0.7f;     // 攻击动画持续时间

    private bool attackTriggered = false; // 用于确保在一次攻击周期内只执行一次跳跃逻辑
    private Vector2 jumpVelocity;        // 跳跃速度

    public override void OnEnter()
    {
        _timer = _timeBetweenShots - .01f;
        attackTriggered = false;
        enemy.currentDamageMultiplier = damageMultiplier;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // 如果不在攻击状态，保持静止并播放Idle动画
        if (!enemy.isAttacking)
        {
            enemy.SetVelocity(0, 0);
        }
        // 如果处于攻击状态且跳跃逻辑已触发，则设置速度
        if (enemy.isAttacking && attackTriggered)
        {
            enemy.SetVelocity(jumpVelocity.x, enemy.rb.velocity.y);
        }
    }

    public override void OnUpdate()
    {
        // 如果不在攻击状态，保持静止并播放Idle动画
        if (!enemy.isAttacking)
        {
            enemy.anim.SetBool("Idle", true);
        }

        _timer += Time.deltaTime;

        // 当累计时间达到攻击冷却时间时，触发攻击（设置动画参数）
        if (!enemy.isAttacking && _timer >= _timeBetweenShots)
        {
            enemy.isAttacking = true;
            enemy.anim.SetBool("Attack2", true);
            enemy.anim.SetBool("Idle", false);
            // 重置计时器，用于测量前摇延迟
            _timer = 0f;
        }

        // 如果处于攻击状态，并且前摇延迟结束，则计算跳跃速度（只执行一次）
        if (enemy.isAttacking && !attackTriggered && _timer >= preAttackDelay)
        {
            CalculateJumpVelocity();
            attackTriggered = true;
        }
    }

    private void CalculateJumpVelocity()
    {
        // 计算跳跃方向：
        // 令攻击在 attackDuration 内刚好横向到达玩家位置
        Vector2 enemyPos = enemy.transform.position;
        Vector2 playerPos = playerTransform.position;
        float jumpSpeedX = (playerPos.x - enemyPos.x) / attackDuration;
        // 适当乘以系数（例如1.2f）来调整效果
        jumpVelocity = new Vector2(jumpSpeedX * 1.2f, jumpHeight);
        enemy.SetVelocity(jumpVelocity.x, jumpVelocity.y);
        Debug.Log("跳击: 计算跳跃速度");
    }

    public override void OnAnimationTrigger(EnemyAnimationTriggerType triggerType)
    {
        if (triggerType == EnemyAnimationTriggerType.EnemyAttackEnd)
        {
            // 攻击动画结束，重置攻击状态和动画参数
            enemy.anim.SetBool("Attack2", false);
            enemy.isAttacking = false;
            Debug.Log("跳击: 攻击结束");
            // 设置攻击冷却
            enemy.currentAttackCooldown = enemy.attackCooldown;
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
        if (triggerType == EnemyAnimationTriggerType.EnemyHitDetermineStart)
        {
            Debug.Log("开始伤害判定咯");
        }
        if (triggerType == EnemyAnimationTriggerType.EnemyHitDetermineEnd)
        {
            Debug.Log("结束伤害判定咯");
        }
    }
}