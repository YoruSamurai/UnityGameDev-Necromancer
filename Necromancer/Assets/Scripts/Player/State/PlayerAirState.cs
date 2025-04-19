using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
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
        if (Input.GetKeyDown(KeyCode.Space) && player.jumpCounter < 2)
        {
            player.jumpCounter++;
            stateMachine.ChangeState(player.jumpState);
        }



        if (player.IsWallBodyDetected() && !player.IsWallHeadDetected())
        {
            Vector2 origin = player.wallCheckHead.position;
            Vector2 direction = Vector2.right * player.facingDir;
            float distance = player.wallCheckDistance;

            // 绘制射线（红色，持续1秒）
            Debug.DrawRay(origin, direction * distance, Color.red, 5f);

            /*RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, player.whatIsGround);

            if (hit.collider != null)
            {
                Debug.Log("命中了物体: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("没有命中任何物体");
            }*/
            Debug.Log("该进入上墙状态");
            stateMachine.ChangeState(player.ledgeUpState);
        }
    }
}
