using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Attack/飞行攻击转换1")]
public class TComponent_飞行攻击转换1 : EnemyBehaviorComponent
{
    [SerializeField] private float distanceToAttack;

    public override void OnUpdate()
    {
        if ((playerTransform.position - enemy.transform.position).magnitude > distanceToAttack && !enemy.isAttacking)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
