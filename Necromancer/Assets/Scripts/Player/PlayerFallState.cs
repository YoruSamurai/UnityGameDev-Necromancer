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

        if(Input.GetKeyDown(KeyCode.Space) && player.jumpCounter < 2)
        {
            player.jumpCounter++;
            stateMachine.ChangeState(player.jumpState);
        }

        if (player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        if(player.IsGroundDetected())
        {
            player.SetJumpCounter(0);
            stateMachine.ChangeState(player.idleState);
        }

        if(xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * .8f * xInput, rb.velocity.y);
        }
    }
}
