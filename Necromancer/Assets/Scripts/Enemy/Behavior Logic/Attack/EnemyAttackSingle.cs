using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single", menuName = "Enemy Logic/Attack/Attack-Single")]
public class EnemyAttackSingle : EnemyAttackSOBase
{


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
