using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Enemy/Components/Idle/地面闲置转换1")]
public class TComponent_地面闲置转换1 : EnemyBehaviorComponent
{
    [SerializeField] private float distance;
    public override void OnUpdate()
    {
        if (Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < distance  && Mathf.Abs(playerTransform.position.y - enemy.transform.position.y) < 2f )
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
