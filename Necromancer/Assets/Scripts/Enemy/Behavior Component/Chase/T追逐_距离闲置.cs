using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/Components/Chase/条件转换/距离闲置")]
public class T追逐_距离闲置 : EnemyBehaviorComponent
{
    [SerializeField] private float xDistanceToIdle; // X轴距离闲置阈值
    [SerializeField] private float yUpDistanceToIdle = 5f; // Y轴距离闲置阈值 PlayerY > enemyY
    [SerializeField] private float yDownDistanceToIdle = 2f; // Y轴距离闲置阈值 PlayerY < enemyY
    [SerializeField] private EnterCondition conditionType; // 之前的条件类型
    [SerializeField] private EnemyStateType targetState;  // 条件满足时要切换的目标状态

    public override void OnUpdate()
    {
        float xdistance = Mathf.Abs(playerTransform.position.x - enemy.transform.position.x);
        float ydistance = playerTransform.position.y - enemy.transform.position.y;
        if (xdistance > xDistanceToIdle || ydistance > yUpDistanceToIdle || ydistance < -yDownDistanceToIdle)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
        }

        //
    }
}
