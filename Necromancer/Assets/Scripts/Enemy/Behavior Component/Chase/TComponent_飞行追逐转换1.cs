using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Chase/飞行追逐转换1")]
public class TComponent_飞行追逐转换1 : EnemyBehaviorComponent
{
    [SerializeField] private float distanceToAttack;
    [SerializeField] private float distanceToIdle;
    public override void OnUpdate()
    {
        float xdistance = Mathf.Abs(playerTransform.position.x - enemy.transform.position.x);
        float ydistance = Mathf.Abs(playerTransform.position.y - enemy.transform.position.y);
        if (xdistance < distanceToAttack)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
            return;
        }
        if (xdistance > distanceToIdle)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
            return;
        }
    }
}
