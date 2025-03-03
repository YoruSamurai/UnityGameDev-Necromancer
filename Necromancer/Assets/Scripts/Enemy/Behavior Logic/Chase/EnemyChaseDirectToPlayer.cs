using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Direct Chase", menuName = "Enemy Logic/Chase/Direct Chase")]
public class EnemyChaseDirectToPlayer : EnemyChaseSOBase
{

    [SerializeField] private float _movementSpeed;
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
        // 计算方向
        int moveDirection = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;

        bool canMove = (enemy.IsWallDetected() || !enemy.IsGroundDetected());
        // 如果前方是墙壁或者没有地面，则停止移动
        if (canMove && moveDirection == enemy.facingDir)
        {
            enemy.SetZeroVelocity();
            
        }
        else if(Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < .1f)
        {
            Debug.Log(21313);
            Debug.Log(playerTransform.position.x + " " + enemy.transform.position.x);
            enemy.SetZeroVelocity();
        }
        else
        {
            enemy.SetVelocity(moveDirection * _movementSpeed, enemy.rb.velocity.y);
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
