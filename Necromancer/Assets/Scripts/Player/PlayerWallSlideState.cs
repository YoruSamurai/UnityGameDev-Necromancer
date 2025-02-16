using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        if (Input.GetKeyDown(KeyCode.Space) && player.jumpCounter < 2)
        {
            player.jumpCounter++;
            stateMachine.ChangeState(player.jumpState);
        }

        if (xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (yInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * 0.98f);




        if (player.IsGroundDetected() || !player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
