using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Chase/条件转换/距离攻击")]
public class T追逐_距离攻击 : EnemyBehaviorComponent
{
    [SerializeField] private float distanceToAttack; // X轴距离阈值
    [SerializeField] private EnterCondition conditionType; // 之前的条件类型
    [SerializeField] private EnemyStateType targetState;  // 条件满足时要切换的目标状态

    public override void OnUpdate()
    {
        float xdistance = Mathf.Abs(playerTransform.position.x - enemy.transform.position.x);
        if (xdistance < distanceToAttack && enemy.currentAttackCooldown <= 0f)
        {
            Debug.Log("对吗");
            enemy.stateMachine.ChangeState(enemy.attackState);   
        }
    }
}
