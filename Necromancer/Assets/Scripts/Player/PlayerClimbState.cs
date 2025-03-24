using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerState
{
    public float climbSpeed = 5f;
    private bool isClimbing = false;
    private float currentYInput;

    public PlayerClimbState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
        : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);

        if (triggerType == PlayerAnimationTriggerType.PlayerIsClimb)
        {
            if(yInput == 0)
            {
                // 如果有输入，开始攀爬
                isClimbing = false;
                player.anim.speed = 0f; // 恢复动画速度
                currentYInput = 0;
            }
            
        }
    }

    public override void Enter()
    {
        base.Enter();
        player.rb.gravityScale = 0f; // 禁用重力
        player.anim.SetBool("Climb", true); // 确保攀爬动画初始为 true
        player.anim.speed = 1f; // 设置动画速度为正常
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("离开攀爬");
        player.rb.gravityScale = 3.5f; // 恢复重力
        player.anim.SetBool("Climb", false); // 退出时关闭攀爬动画
        player.anim.speed = 1f; // 恢复动画速度
    }

    public override void Update()
    {
        base.Update();

        // 检测到一键平台并且向上输入
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

        // 离开攀爬的条件
        if (!player.isOnLadder)
        {
            stateMachine.ChangeState(player.idleState);
        }

        // 按下跳跃键
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.jumpState);
        }

        // 检测输入并控制攀爬
        if (yInput != 0)
        {
            // 如果有输入，开始攀爬
            currentYInput = yInput;
            isClimbing = true;
            player.anim.speed = 1f; // 恢复动画速度
        }
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isClimbing)
        {
            player.SetVelocity(0, currentYInput * climbSpeed); // 继续攀爬
        }
        else
        {
            player.SetVelocity(0, 0); // 停止移动
        }
    }
}










/*using System.Collections;
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

}*/