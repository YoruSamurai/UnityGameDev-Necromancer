using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerClimbState : PlayerState
{
    public float climbSpeed = 8f;
    private bool isClimbing = false;
    private float currentYInput;

    private Transform currentLadder; // 当前攀爬的梯子
    private float ladderCenterX;     // 梯子的水平中心坐标
    private float horizontalOffset = 0.2f; // 水平偏移量

    private float timer;

    // 创建一个包含多个Layer的LayerMask
    public int combinedGroundLayers = LayerMask.GetMask("Ground", "OneWayPlatform");

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
        player.climbTimer = .1f;
        player.rb.gravityScale = 0f; // 禁用重力
        player.anim.SetBool("Climb", true); // 确保攀爬动画初始为 true
        player.anim.speed = 1f; // 设置动画速度为正常
        timer = 0f;
        // 禁用周围平台
        var platforms = Physics2D.OverlapCircleAll(player.transform.position, 3f, player.combinedGroundLayers);
        foreach (var platform in platforms)
        {
            if (platform.CompareTag("OneWayPlatform"))
            {
                player.AddIgnoredPlatform(platform);
            }
        }

        if (player.currentLadder.IsLadderExist())
        {
            ladderCenterX = (player.currentLadder.ladderBottomX + player.currentLadder.ladderTopX) / 2;
            AdjustPositionToLadder();
        }
        
    }

    void AdjustPositionToLadder()
    {
        // 根据面向方向计算目标X坐标
        float targetX = ladderCenterX - (player.facingDir * horizontalOffset);

        // 平滑移动位置
        player.transform.DOMoveX(targetX, 0.01f)
            .SetEase(Ease.OutQuad);
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("离开攀爬");
        Debug.Log("我们的时光" + timer);
        player.rb.gravityScale = 3.5f; // 恢复重力
        player.anim.SetBool("Climb", false); // 退出时关闭攀爬动画
        isClimbing = false;
        player.climbCooldownTimer = .2f;
        player.isClimbing = false;
        player.anim.speed = 1f; // 恢复动画速度
        //player.ClearIgnoredPlatforms();
        if (!(stateMachine.nextState is PlayerDownDashState))
        {
            player.ClearIgnoredPlatforms();
        }
    }

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;

        // 检测到一键平台并且向上输入
        RaycastHit2D hit = Physics2D.Raycast(
            player.transform.position,
            Vector2.up,
            1f,
            combinedGroundLayers
        );
        // 绘制射线
        Debug.DrawLine(player.transform.position, player.transform.position + Vector3.up * 1f, Color.yellow);
        if (hit.collider != null && hit.collider.CompareTag("OneWayPlatform") && currentYInput > 0)
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
        if (Input.GetKeyDown(KeyCode.Space) && player.climbTimer <= 0)
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

        // 向下发射一条射线检测地面
        RaycastHit2D groundHit = Physics2D.Raycast(
            player.groundCheck.transform.position - new Vector3(0, 1.5f),
            Vector2.down,
            0.5f, // 射线长度，可以根据需要调整
            combinedGroundLayers
        );

        // 绘制射线方便调试
        Debug.DrawLine(player.groundCheck.transform.position - new Vector3(0, 1.5f), player.groundCheck.transform.position - new Vector3(0, 1.5f) + Vector3.down * 0.5f, Color.yellow);

        // 如果检测到地面，则切换回Idle状态
        if (groundHit.collider != null && currentYInput < 0)
        {
            Debug.Log("检测到地面，退出攀爬状态");
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (xInput != 0)
        {
            // 确定新方向
            int newFacingDir = (int)Mathf.Sign(xInput);

            // 如果方向改变
            if (newFacingDir != player.facingDir)
            {
                player.Flip(); // 翻转角色Sprite

                // 计算新位置
                float targetX = ladderCenterX - (newFacingDir * horizontalOffset);

                // 平滑移动
                player.transform.DOMoveX(targetX, 0.01f)
                    .SetEase(Ease.OutQuad);
            }
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



