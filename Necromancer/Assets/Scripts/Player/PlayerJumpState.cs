using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();
    }
    //向上跳是jump 向下坠是fall
    public override void Update()
    {
        base.Update();

        //按下空格 可以二段跳 就二段跳
        if (Input.GetKeyDown(KeyCode.Space) && player.jumpCounter < 2)
        {
            player.jumpCounter++;
            stateMachine.ChangeState(player.jumpState);
        }
        //开始下坠了 进入下坠状态
        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
        }
        //天上动的比较慢
        if (xInput != 0)
        {
            player.SetVelocity(player.moveSpeed * .8f * xInput, rb.velocity.y);
        }
    }
}
