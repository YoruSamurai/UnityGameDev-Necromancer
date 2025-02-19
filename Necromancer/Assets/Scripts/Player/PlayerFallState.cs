using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //按下空格 并且玩家还没跳过第二下 就进行二段跳 并设置跳跃计数器
        if(Input.GetKeyDown(KeyCode.Space) && player.jumpCounter < 2)
        {
            player.jumpCounter++;
            stateMachine.ChangeState(player.jumpState);
        }

        //下落过程中 玩家脸上有墙 进入滑墙状态 
        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        //玩家到地上了 重置跳跃计数器 并回到静止状态
        if(player.IsGroundDetected())
        {
            player.SetJumpCounter(0);
            stateMachine.ChangeState(player.idleState);
        }

        //天上跳的没那么快
        if(xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * .8f * xInput, rb.velocity.y);
        }
    }
}
