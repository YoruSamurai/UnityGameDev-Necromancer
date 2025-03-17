using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeUpState : PlayerState
{
    public PlayerLedgeUpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void AnimationTriggerEvent(PlayerAnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if(triggerType == PlayerAnimationTriggerType.PlayerAnimationEndTrigger)
        {
            player.SetJumpCounter(0);
            stateMachine.ChangeState(player.idleState);
        }
        if(triggerType == PlayerAnimationTriggerType.PlayerLedgeUp)
        {
            // 获取当前玩家位置
            Vector2 startPosition = player.transform.position;
            Vector2 targetPosition = new Vector2(
                startPosition.x + player.facingDir * 2f,
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

        // 检测身体射线（命中墙）
        RaycastHit2D hitBody = Physics2D.Raycast(player.wallCheckBody.position, Vector2.right * player.facingDir, player.wallCheckDistance, player.whatIsGround);

        // 头部射线末端
        Vector2 headRayEnd = (Vector2)player.wallCheckHead.position + Vector2.right * player.facingDir * player.wallCheckDistance;

        // 头部射线末端向下检测地面
        RaycastHit2D hitGround = Physics2D.Raycast(headRayEnd, Vector2.down, Mathf.Infinity, player.whatIsGround);

        // 调试可视化
        Debug.DrawRay(player.wallCheckBody.position, Vector2.right * player.facingDir * player.wallCheckDistance, Color.green, 2f);
        Debug.DrawRay(headRayEnd, Vector2.down * 5f, Color.red, 2f);

        if (hitBody.collider != null && hitGround.collider != null)
        {
            float targetX = hitBody.point.x - (player.facingDir * 1.5f); // 让角色稍微离墙
            float groundY = hitGround.point.y; // 获取地面高度
            float targetY = groundY - 0.75f; // 计算手部攀爬高度

            newPosition = new Vector2(targetX, targetY);
        }
        else
        {
            Debug.Log("未能调整上墙位置，使用默认位置");
        }

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
        player.SetZeroVelocity();
    }

    
}
