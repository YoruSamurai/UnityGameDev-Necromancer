using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{

    private GameObject currentTrailObj;
    private ParticleSystem trail;

    public bool isCornerDetected;
    public float lastVelocityY;
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //冲刺开始时 给玩家设置冲刺持续时间
        // Dash 开始时设置无敌
        player.SetInvincible(true);
        stateTimer = player.dashDuration;

        // 实例化一个新的 dashTrail 作为当前 trail
        currentTrailObj = GameObject.Instantiate(player.dashTrail, player.transform.position - new Vector3(0,1f,0), Quaternion.identity, player.transform);
        trail = currentTrailObj.GetComponent<ParticleSystem>();
        trail.Clear();

        lastVelocityY = rb.velocity.y;
        isCornerDetected = false;
    }

    public override void Exit()
    {
        base.Exit();
        // Dash 结束时设置无敌
        player.SetInvincible(false);
        float xVelocity = rb.velocity.x;
        if(!isCornerDetected)
        {
            player.SetVelocity(0, lastVelocityY, xVelocity);
        }
        else
        {
            player.SetVelocity(0,0, xVelocity);
        }

        player.rb.gravityScale = 3.5f;

        // 开始一个协程，在一段时间后关闭TrailRenderer
        player.StopTrail(currentTrailObj, .3f);
    }

    public override void Update()
    {
        base.Update();
        

        //在冲刺时间结束后，回到静止状态
        if(stateTimer < 0f)
        {
            //stateMachine.ChangeState(player.moveState);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //Debug.Log("TY速度" + rb.velocity.y);
        
        //设置冲刺速度 开冲
        if (lastVelocityY < 0f && rb.velocity.y - lastVelocityY > 10f)
        {
            player.SetVelocity(player.dashSpeed * player.dashDir, lastVelocityY);
            isCornerDetected=true;

        }
        else
        {
            player.SetVelocity(player.dashSpeed * player.dashDir, rb.velocity.y);
            lastVelocityY = rb.velocity.y;
        }

        // 检测墙角情况
        if (player.IsWallFootDetected() && !player.IsWallBodyDetected())
        {
            // 如果满足条件，给一个向上的速度
            player.SetVelocity(player.dashSpeed * player.dashDir * .5f, 10f);
            player.rb.gravityScale = 10f;
        }
        
    }
}
