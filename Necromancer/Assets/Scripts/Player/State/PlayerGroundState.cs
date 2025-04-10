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
        
        // 优先判断 S+空格，跳过跳跃判断
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("我射");
            // 向下发射一条射线检测地面
            RaycastHit2D groundHit = Physics2D.Raycast(
                player.transform.position,
                Vector2.down,
                2f, // 射线长度，可以根据需要调整
                player.combinedGroundLayers
            );

            // 绘制射线方便调试
            Debug.DrawLine(player.transform.position, player.transform.position + Vector3.down * 2f, Color.yellow);

            // 如果检测到地面，则切换回Idle状态
            if (groundHit.collider != null && groundHit.collider.tag == "OneWayPlatform")
            {
                Debug.Log("优先触发下落逻辑");
                player.dropTimer = .05f;
                return;  // 阻止进入其他状态
            }
        }
        //判断下蹲
        if (Input.GetKey(KeyCode.S) && player.IsGroundDetected() && !(stateMachine.currentState is PlayerCrouchingState))
        {
            stateMachine.ChangeState(player.crouchingState);
        }

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
