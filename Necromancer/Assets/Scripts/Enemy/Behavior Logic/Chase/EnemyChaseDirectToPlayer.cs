using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Direct Chase", menuName = "Enemy Logic/Chase/Direct Chase")]
public class EnemyChaseDirectToPlayer : EnemyChaseSOBase
{

    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        enemy.anim.SetBool("Chase", true);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        enemy.anim.SetBool("Chase", false);
    }

    public override void DoUpdateLogic()
    {
        base.DoUpdateLogic();
        float distance = (playerTransform.position - enemy.transform.position).magnitude;
        if (distance < 2f)
        {
            enemy.stateMachine.ChangeState(enemy.attackState);
            return;
        }
        if (distance > 12f)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
            return;
        }
        
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
