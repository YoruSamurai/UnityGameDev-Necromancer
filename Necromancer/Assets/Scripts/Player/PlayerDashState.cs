using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //冲刺开始时 给玩家设置冲刺持续时间
        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.SetVelocity(0,rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        //设置冲刺速度 开冲
        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        //在冲刺时间结束后，回到静止状态
        if(stateTimer < 0f)
        {
            //stateMachine.ChangeState(player.moveState);
            stateMachine.ChangeState(player.idleState);
        }
    }
}
