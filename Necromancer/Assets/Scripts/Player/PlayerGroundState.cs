using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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

        //如果玩家不在地上了 进入下坠状态 用于跳下平台的情况
        if (!player.IsGroundDetected())
        {
            //player.SetJumpCounter(1);
            stateMachine.ChangeState(player.fallState);
        }
            
        //按下空格键 并且玩家在地上的话 就进入跳跃状态 并设置跳跃计数器为1
        if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
        {
            player.SetJumpCounter(1);
            stateMachine.ChangeState(player.jumpState);
        }

    }
}
