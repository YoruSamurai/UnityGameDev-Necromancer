using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle Random Wander", menuName = "Enemy Logic/Idle/Random Wander")]
public class EnemyIdleRandomWander : EnemyIdleSOBase
{

    [SerializeField] private float MovementSpeed;


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
        // 检测是否碰到墙或者即将掉落，如果是，则翻转
        if (!enemy.IsGroundDetected() || enemy.IsWallDetected())
        {
            enemy.Flip();
        }
        enemy.SetVelocity(enemy.facingDir * MovementSpeed, enemy.rb.velocity.y);
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
