using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Idle/飞行闲置转换1")]
public class TComponent_飞行闲置转换1 : EnemyBehaviorComponent
{
    [SerializeField] private float detectionRange;

    public override void OnUpdate()
    {
        float distance = Vector2.Distance(enemy.transform.position, playerTransform.position);
        if (distance <= detectionRange)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }

    public override void OnEnter() { }
    public override void OnExit() { }
}
