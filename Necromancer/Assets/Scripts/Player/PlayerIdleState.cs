using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    //没什么用 写一下
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    //如果是当前状态 每一帧被调用
    public override void Update()
    {
        base.Update();

        
        //有X方向移动了 就进入移动状态
        if (xInput != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }

        //停止X移动 避免玩家跳完之后滑动
        player.SetVelocity(0, rb.velocity.y);
    }
}
