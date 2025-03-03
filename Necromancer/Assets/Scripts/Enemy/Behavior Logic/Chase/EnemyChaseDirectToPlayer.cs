using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Direct Chase", menuName = "Enemy Logic/Chase/Direct Chase")]
public class EnemyChaseDirectToPlayer : EnemyChaseSOBase
{

    [SerializeField] private float _movementSpeed = 2f;
    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoUpdateLogic()
    {
        base.DoUpdateLogic();

        Vector2 moveDir = (playerTransform.position - enemy.transform.position).normalized;
        enemy.MoveEnemy(moveDir * _movementSpeed);
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
