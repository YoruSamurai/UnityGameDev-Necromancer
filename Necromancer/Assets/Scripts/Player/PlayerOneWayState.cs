using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerOneWayState : PlayerState
{
    public PlayerOneWayState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if (triggerType == PlayerAnimationTriggerType.PlayerAnimationEndTrigger)
        {
            player.SetJumpCounter(0);
            stateMachine.ChangeState(player.idleState);
        }
        if (triggerType == PlayerAnimationTriggerType.PlayerLedgeUp)
        {
            // 获取当前玩家位置
            Vector2 startPosition = player.transform.position;
            Vector2 targetPosition = new Vector2(
                startPosition.x + player.facingDir * 1f,
                startPosition.y + 2f
            );

            // 使用DoTween平滑移动
            player.transform.DOMove(targetPosition, 0.08f) // 0.5秒完成位移
                .SetEase(Ease.OutQuad) // 设置缓动效果，OutQuad是先快后慢
                .OnComplete(() =>
                {
                    // 移动结束后的回调，可以处理后续状态
                    player.AnimationTrigger(PlayerAnimationTriggerType.PlayerAnimationEndTrigger);
                });
        }
    }


    public override void Enter()
    {
        base.Enter();
        player.rb.isKinematic = true;
        AdjustLedgePosition();
    }

    private void AdjustLedgePosition()
    {
        Vector2 newPosition = player.transform.position;

        


        newPosition = new Vector2(newPosition.x, newPosition.y + .5f);
        
        

        player.transform.position = newPosition;
        Debug.Log("调整上墙位置到：" + newPosition);
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.isKinematic = false;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        player.SetZeroVelocity();
    }

}
