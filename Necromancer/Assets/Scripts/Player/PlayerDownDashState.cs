using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerDownDashState : PlayerState
{
    private float dashSpeed = 40f;
    private float shadowInterval = 0.1f;  // 残影生成间隔
    private float shadowTimer;
    private bool hasLanded; // 标记是否已经落地

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

        // 暂停动画
        player.anim.speed = 0;
        // 启动残影特效
        player.CreateAfterImage();
        // 重置标记
        hasLanded = false;
        shadowTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();

        // 退出状态恢复动画播放
        player.anim.speed = 1;
    }

    public override void Update()
    {
        base.Update();

        if (!hasLanded)
        {
            // 检测是否触地
            if (player.IsGroundDetected())
            {
                hasLanded = true;
                player.SetZeroVelocity(); // 停止下冲
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
    }

    
}
