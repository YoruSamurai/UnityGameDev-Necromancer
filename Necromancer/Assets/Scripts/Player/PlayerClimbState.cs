using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerClimbState : PlayerState
{
    public float climbSpeed = 5f;

    public PlayerClimbState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 进入攀爬状态：禁用重力，并切换攀爬动画
        player.rb.gravityScale = 0f;
        player.anim.SetBool("Climb", true);
    }
    public override void Exit()
    {
        base.Exit();
        // 恢复重力，并关闭攀爬动画
        Debug.Log("离开攀爬");
        player.rb.gravityScale = 3.5f;
        player.anim.SetBool("Climb", false);
    }



    public override void Update()
    {
        base.Update();
        RaycastHit2D hit = Physics2D.Raycast(
            player.transform.position,
            Vector2.up,
            1f,
            LayerMask.GetMask("Ground")
        );

        if (hit.collider != null && hit.collider.CompareTag("OneWayPlatform") && yInput > 0)
        {
            Debug.Log("检测到一键平台：" + hit.collider.name);
            Debug.Log("通过攀爬上墙");
            player.stateMachine.ChangeState(player.oneWayState);
        }
        // 离开攀爬的条件：如果玩家不再处于梯子区域，或者按下跳跃键
        if (!player.isOnLadder)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.jumpState);
        }
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.SetVelocity(0, yInput * climbSpeed);
    }

}