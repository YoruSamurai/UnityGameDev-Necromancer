using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Random Wander", menuName = "Enemy Logic/Idle/Random Wander")]
public class EnemyIdleRandomWander : EnemyIdleSOBase
{

    


    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        enemy.anim.SetBool("Move", true);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        enemy.anim.SetBool("Move", false);
    }

    public override void DoUpdateLogic()
    {
        base.DoUpdateLogic();
        if ((playerTransform.position - transform.position).magnitude < 12f)
        {
            enemy.stateMachine.ChangeState(enemy.chaseState);
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
