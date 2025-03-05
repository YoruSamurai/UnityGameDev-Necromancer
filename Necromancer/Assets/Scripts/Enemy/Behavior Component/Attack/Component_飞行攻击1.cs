using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Attack/飞行攻击1")]
public class Component_飞行攻击1 : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots = 3f; // 攻击冷却时间

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

        if (!enemy.isAttacking && _timer >= _timeBetweenShots)
        {
            _timer = 0f;
            Debug.Log("Component_AttackSingle: 触发攻击");
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
            enemy.anim.SetBool("Attack", false);
            enemy.isAttacking = false;
            Debug.Log("Component_AttackSingle: 攻击结束");
            // 如果当前的攻击行为在复合攻击状态下，则通知复合攻击模块重置
            EnemyCompositeAttackSO composite = enemy.enemyAttackBaseInstance as EnemyCompositeAttackSO;
            if (composite != null)
            {
                Debug.Log("进入重置攻击");
                composite.ResetAttack();
            }
        }
    }
}
