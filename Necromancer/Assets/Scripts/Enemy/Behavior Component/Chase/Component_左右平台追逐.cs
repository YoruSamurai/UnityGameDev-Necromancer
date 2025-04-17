using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Chase/左右平台追逐")]
public class Component_左右平台追逐 : EnemyBehaviorComponent
{
    [SerializeField] private float _movementSpeed;


    private int _moveDirection;
    private bool _canMove;

    public override void OnEnter()
    {
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // 如果正在被击退，则跳过正常移动逻辑1
        if (enemy.isKnockBack)
        {
            return;
        }

        // 计算方向
        _moveDirection = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;

        _canMove = (enemy.IsWallDetected() || !enemy.IsGroundDetected());

        // 如果前方是墙壁或者没有地面，则停止移动
        if (_canMove && _moveDirection == enemy.facingDir)
        {
            enemy.SetVelocity(0, enemy.rb.velocity.y);
        }
        else if (Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < 2f)
        {
            if (_moveDirection != enemy.facingDir)
                enemy.Flip();
            enemy.SetVelocity(0, enemy.rb.velocity.y);
        }
        else
        {
            enemy.SetVelocity(_moveDirection * _movementSpeed, enemy.rb.velocity.y);
        }
    }

    public override void OnUpdate()
    {
        // 更新动画状态
        if (_canMove && _moveDirection == enemy.facingDir)
        {
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Chase", false);
        }
        else if (Mathf.Abs(playerTransform.position.x - enemy.transform.position.x) < 2f)
        {
            enemy.anim.SetBool("Idle", true);
            enemy.anim.SetBool("Chase", false);
        }
        else
        {
            enemy.anim.SetBool("Chase", true);
            enemy.anim.SetBool("Idle", false);
        }
    }
}
