using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
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



        //下落过程中 玩家脸上有墙 进入滑墙状态 
        if (player.IsWallBodyDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        //玩家到地上了 重置跳跃计数器 并回到静止状态
        if(player.IsGroundDetected())
        {
            player.SetJumpCounter(0);
            stateMachine.ChangeState(player.idleState);
        }

        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //天上跳的没那么快
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * .8f * xInput, rb.velocity.y);
        }
    }
}
