using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Chase/哥布林追逐")]
public class Component_哥布林追逐 : EnemyBehaviorComponent
{
    [SerializeField] private float _movementSpeed;

    [SerializeField] private float distanceToAttack;
    [SerializeField] private float distanceToIdle;

    private float _timer;
    [SerializeField] private float _timeBetweenShots = 2f; // 几秒追不到就进入攻击

    public override void OnEnter()
    {
        _timer = 0f;
    }

    public override void OnUpdate()
    {
        _timer += Time.deltaTime;
        // 计算方向
        int moveDirection = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;

        bool canMove = (enemy.IsWallDetected() || !enemy.IsGroundDetected());
        // 如果前方是墙壁或者没有地面，则停止移动
        if (canMove && moveDirection == enemy.facingDir)
        {
            enemy.SetZeroVelocity();
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Chase", false);

        }
        else if (Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < 2f)
        {

            enemy.SetZeroVelocity();
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Chase", false);
        }
        else
        {
            enemy.SetVelocity(moveDirection * _movementSpeed, enemy.rb.velocity.y);
            enemy.anim.SetBool("Chase", true);
            enemy.anim.SetBool("Idle", false);
        }

        float xdistance = Mathf.Abs(playerTransform.position.x - enemy.transform.position.x);
        float ydistance = Mathf.Abs(playerTransform.position.y - enemy.transform.position.y);
        if ((xdistance < distanceToAttack || _timer > _timeBetweenShots) && enemy.currentAttackCooldown <= 0f)
        {
            Debug.Log("找的队");
            enemy.stateMachine.ChangeState(enemy.attackState);
            return;
        }
        if (xdistance > distanceToIdle || ydistance > 5f)
        {
            enemy.stateMachine.ChangeState(enemy.idleState);
            return;
        }
    }
}
