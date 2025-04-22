using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{

    private GameObject currentTrailObj;
    private ParticleSystem trail;
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
    }

    public override void Exit()
    {
        base.Exit();
        // Dash 结束时设置无敌
        player.SetInvincible(false);
        player.SetVelocity(0,rb.velocity.y);


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
        //设置冲刺速度 开冲
        player.SetVelocity(player.dashSpeed * player.dashDir, rb.velocity.y);
    }
}
