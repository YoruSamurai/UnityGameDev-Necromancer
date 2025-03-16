using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    // 新增武器伤害检测事件
    private void AnimationHitTrigger()
    {
        if (PlayerStats.Instance.currentEquipmentIndex == 1)
        {
            PlayerStats.Instance.baseEquipment1.TriggerHitCheck();
        }
        else if(PlayerStats.Instance.currentEquipmentIndex == 2)
        {
            PlayerStats.Instance.baseEquipment2.TriggerHitCheck();
        }
    }

    //Trigger when animation ends.在动画结束的时候触发 让玩家切换回基础状态
    //我草 这个名字不能随便改！
    private void AnimationEndTrigger()
    {
        Debug.Log("我在哪？" + this.gameObject.name);
        if(PlayerStats.Instance.isAttacking)
            PlayerStats.Instance.isAttacking = false;
        else if(PlayerStats.Instance.isParrying)
            PlayerStats.Instance.isParrying = false;
        PlayerStats.Instance.SetCurrentEquipmentIndex(0);
        player.AnimationTrigger();
    }

    private void LedgeUpOffsetTrigger()
    {
        /*// 获取当前玩家位置
        Vector2 newPosition = player.transform.position;
        // 计算新的位置
        newPosition.x += player.facingDir * 2f; // 向面向方向移动 2f
        newPosition.y += 2f; // 高度增加 2f

        // 应用新位置
        player.transform.position = newPosition;*/

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
                //player.AnimationTrigger();
            });
    }

    //只是一个trigger 触发
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

}
