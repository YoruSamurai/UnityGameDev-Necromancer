using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Idle/闲置射线感知转换")]
public class T闲置_射线感知 : EnemyBehaviorComponent
{
    [SerializeField] private float distance;
    [SerializeField] private EnterCondition conditionType; // 之前的条件类型
    [SerializeField] private EnemyStateType targetState;     // 新增字段，指定满足条件时要进入哪个状态

    private float timer;
    [SerializeField] private float noticeTimer;
    private bool isNoticed;

    public override void OnEnter()
    {
        base.OnEnter();
        timer = 0f;
        isNoticed = false;
    }

    public override void OnUpdate()
    {
        if(isNoticed)
        {
            timer += Time.deltaTime;
            if(timer >= noticeTimer )
            {
                // 根据 targetState 切换状态
                switch (targetState)
                {
                    case EnemyStateType.Attack:
                        if (enemy.currentAttackCooldown <= 0f)
                            enemy.stateMachine.ChangeState(enemy.attackState);
                        break;
                    case EnemyStateType.Chase:
                        enemy.stateMachine.ChangeState(enemy.chaseState);
                        break;
                    case EnemyStateType.Idle:
                        enemy.stateMachine.ChangeState(enemy.idleState);
                        break;
                }
            }
        }
        if (conditionType == EnterCondition.FrontRaycast && isNoticed == false)
        {
            // 射线起点为敌人当前位置
            Vector2 origin = enemy.transform.position;
            // 射线方向为敌人正前方（根据 facingDir 判断左右方向）
            Vector2 direction = Vector2.right * enemy.facingDir;
            // 执行射线检测
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);
            // 绘制调试射线
            Debug.DrawRay(origin, direction * distance, Color.red);

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    isNoticed = true;
                    enemy.canMove = false;
                    break;
                }
            }
        }
        // 以后可以添加其他条件类型的判断
    }
}
