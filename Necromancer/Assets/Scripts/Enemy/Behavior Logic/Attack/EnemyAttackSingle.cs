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
        enemy.anim.SetBool("Attack", true);
        enemy.isAttacking = true;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        enemy.anim.SetBool("Attack", false);
        enemy.isAttacking = true;
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
