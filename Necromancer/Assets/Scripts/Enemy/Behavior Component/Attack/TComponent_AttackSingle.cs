using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Enemy/Components/Attack/TransitionAttackSingle")]
public class TComponent_AttackSingle : EnemyBehaviorComponent
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
