using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerDownDashState : PlayerState
{
    private float dashSpeed = 40f;//下冲速度
    private float shadowInterval = 0.03f;  // 残影生成间隔
    private bool hasLanded; // 标记是否已经落地
    //在这个里面我们给他0.1s的时间，时间结束后复原所有单向平台，避免玩家下冲的时候 如果下面是单向平台 就会冲下去 我们希望避免这个情况发生
    private float clearOneWayTimer;
    private bool hasCleared;

    public PlayerDownDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if (triggerType == PlayerAnimationTriggerType.PlayerAnimationEndTrigger)
        {
            stateMachine.ChangeState(player.idleState);
        }
        
    }

    public override void Enter()
    {
        base.Enter();
        // 暂停动画 后续根据真正的动作 可能不是暂停 而是在一段里面重播 比如在中间一个时间进行事件判断，如果此时不在地面我们就重播 
        //如果在地面了我们就进到事件的位置 播完后退出
        player.anim.speed = 0;
        //生成残影
        player.CreateAfterImage(shadowInterval);
        hasLanded = false;
        clearOneWayTimer = .1f;
        hasCleared = false;
    }

    public override void Exit()
    {
        base.Exit();

        // 退出状态恢复动画播放
        player.anim.speed = 1;
        player.ClearIgnoredPlatforms();
    }


    public override void Update()
    {
        base.Update();

        clearOneWayTimer -= Time.deltaTime;
        if(clearOneWayTimer < 0f && !hasCleared)
        {
            hasCleared = true;
            player.ClearIgnoredPlatforms();
        }
        //成功落地！
        if (!hasLanded)
        {
            // 检测是否触地
            if (player.IsGroundDetected())
            {
                hasLanded = true;
                player.anim.speed = 1; // 恢复动画播放
            }
        }
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // 没落地前持续向下冲
        if (!hasLanded)
        {
            player.SetVelocity(0, -dashSpeed);
        }
        else
        {
            player.SetZeroVelocity(); // 停止下冲
        }
    }
 
}
