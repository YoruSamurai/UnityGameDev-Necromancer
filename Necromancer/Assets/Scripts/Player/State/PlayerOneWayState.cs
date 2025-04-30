using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//在这个状态，玩家的rb会被设置成kinematic，也就是不受物理碰撞影响
public class PlayerOneWayState : PlayerState
{

    // 创建一个包含多个Layer的LayerMask
    public int combinedGroundLayers = LayerMask.GetMask("Ground", "OneWayPlatform");
    public PlayerOneWayState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    //是否在攀上去结束前退出？
    public bool isMotionEnd;


    public override void Enter()
    {
        base.Enter();
        player.rb.isKinematic = true;
        isMotionEnd = false;
        AdjustLedgePosition();
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.isKinematic = false;
        /*if(!isMotionEnd)
        {
            if (stateMachine.lastState == player.climbState)
            {
                // 获取当前玩家位置
                Vector2 startPosition = player.transform.position;
                OneWayPlatformController oneWayPlatformController = player.gameObject.GetComponent<OneWayPlatformController>();
                Debug.Log(oneWayPlatformController.centerX + "a jsfoiajosaf");
                Vector2 targetPosition = new Vector2(
                    player.transform.position.x,
                    startPosition.y + 1f
                );
                player.transform.position = targetPosition;
                player.SetVelocity(rb.velocity.x, 0);
            }
            else
            {
                // 获取当前玩家位置
                Vector2 startPosition = player.transform.position;
                OneWayPlatformController oneWayPlatformController = player.gameObject.GetComponent<OneWayPlatformController>();
                Debug.Log(oneWayPlatformController.centerX + "a jsfoiajosaf");
                Vector2 targetPosition = new Vector2(
                    oneWayPlatformController.centerX,
                    startPosition.y + 1f
                );

                player.transform.position = targetPosition;
                player.SetVelocity(rb.velocity.x, 0);
            }

        }*/

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
            if(stateMachine.lastState == player.climbState)
            {
                // 获取当前玩家位置
                Vector2 startPosition = player.transform.position;
                OneWayPlatformController oneWayPlatformController = player.gameObject.GetComponent<OneWayPlatformController>();
                Debug.Log(oneWayPlatformController.centerX + "a jsfoiajosaf");
                Vector2 targetPosition = new Vector2(
                    player.transform.position.x,
                    startPosition.y + 1f
                );

                // 使用DoTween平滑移动
                player.transform.DOMove(targetPosition, 0.08f) // 0.5秒完成位移
                    .SetEase(Ease.OutQuad) // 设置缓动效果，OutQuad是先快后慢
                    .OnComplete(() =>
                    {
                        // 移动结束后的回调，可以处理后续状态
                        isMotionEnd = true;
                        player.AnimationTrigger(PlayerAnimationTriggerType.PlayerAnimationEndTrigger);
                    });
            }
            else
            {
                // 获取当前玩家位置
                Vector2 startPosition = player.transform.position;
                OneWayPlatformController oneWayPlatformController = player.gameObject.GetComponent<OneWayPlatformController>();
                Debug.Log(oneWayPlatformController.centerX +"a jsfoiajosaf");
                Vector2 targetPosition = new Vector2(
                    oneWayPlatformController.centerX,
                    startPosition.y + 1f
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
    }
    private void AdjustLedgePosition()
    {
        // 获取玩家当前位置
        Vector2 currentPosition = player.transform.position;
        if(stateMachine.lastState != player.climbState)
        {
            OneWayPlatformController oneWayPlatformController = player.gameObject.GetComponent<OneWayPlatformController>();
            currentPosition.x = oneWayPlatformController.centerX;
        }
        // 定义射线的起始位置（玩家位置）和方向（向上）
        Vector2 rayOrigin = currentPosition - new Vector2(0,2f);
        Vector2 rayDirection = Vector2.up;

        // 定义射线的长度
        float rayLength = 5f; // 你可以根据需要调整这个值

        // 发射射线
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, combinedGroundLayers);

        if (hit.collider != null)
        {
            // 获取射线检测到的地面的Y坐标，然后加上1f
            Debug.Log("hitpo" + hit.point.y);
            float groundYPosition = Mathf.Round(hit.point.y) + 2f;

            // 更新玩家位置
            Vector2 newPosition = new Vector2(currentPosition.x, groundYPosition);
            player.transform.position = newPosition;

            Debug.Log("调整上墙位置到：" + newPosition);
        }
        else
        {
            // 发射射线
            hit = Physics2D.Raycast(rayOrigin + new Vector2(player.facingDir , 0), rayDirection, rayLength, combinedGroundLayers);
            // 如果没有检测到地面，可以选择保持原位置或进行其他处理
            if (hit.collider != null)
            {
                // 获取射线检测到的地面的Y坐标，然后加上1f
                float groundYPosition = Mathf.Round(hit.point.y) + 2f;

                // 更新玩家位置
                Vector2 newPosition = new Vector2(currentPosition.x, groundYPosition);
                player.transform.position = newPosition;

                Debug.Log("调整上墙位置到：" + newPosition);
            }
            else
            {
                Debug.LogWarning("未检测到地面，保持原位置：" + currentPosition);

            }
        }
    }

    

}
