using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Chase/条件转换/追逐范围感知")]
public class T追逐_范围转换 : EnemyBehaviorComponent
{
    [SerializeField] private float detectionRadius;  // 检测范围的半径
    [SerializeField] private EnterCondition conditionType; // 之前的条件类型
    [SerializeField] private EnemyStateType targetState;  // 条件满足时要切换的目标状态

    public override void OnUpdate()
    {
        // 使用 OverlapCircle 进行范围检测，假设玩家在 "Player" 层中
        Collider2D hit = Physics2D.OverlapCircle(enemy.transform.position, detectionRadius, LayerMask.GetMask("Player"));

        // 绘制调试圆的近似：这里绘制一条向上的射线作为辅助观察
        Debug.DrawRay(enemy.transform.position, Vector2.up * detectionRadius, Color.blue);
        if (conditionType == EnterCondition.DistanceMinus)
        {
            // 如果检测到的碰撞体存在且标签为 "Player"，则根据 targetState 切换状态
            if (hit != null && hit.CompareTag("Player"))
            {
                switch (targetState)
                {
                    case EnemyStateType.Attack:
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
        else if (conditionType == EnterCondition.DistancePlus)
        {
            // 如果检测到的碰撞体存在且标签为 "Player"，则根据 targetState 切换状态
            if (hit == null)
            {
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



    }
}
