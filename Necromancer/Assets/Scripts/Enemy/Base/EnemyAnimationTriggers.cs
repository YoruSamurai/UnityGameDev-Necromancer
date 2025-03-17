using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationTriggers : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    /*// 新增武器伤害检测事件
    private void AnimationHitTrigger()
    {
        if (PlayerStats.Instance.currentEquipmentIndex == 1)
        {
            PlayerStats.Instance.baseEquipment1.TriggerHitCheck();
        }
        else if (PlayerStats.Instance.currentEquipmentIndex == 2)
        {
            PlayerStats.Instance.baseEquipment2.TriggerHitCheck();
        }
    }*/

    private void AnimationTrigger(EnemyAnimationTriggerType triggerType)
    {
        enemy.AnimationTrigger(triggerType);
    }

    private void AnimationHitDetermineStart()
    {
        enemy.AnimationTrigger(EnemyAnimationTriggerType.EnemyHitDetermineStart);
    }

    private void AnimationHitDetermineEnd()
    {
        enemy.AnimationTrigger(EnemyAnimationTriggerType.EnemyHitDetermineEnd);
    }

    //Trigger when animation ends.在动画结束的时候触发 让玩家切换回基础状态
    //我草 这个名字不能随便改！
    private void AnimationEndTrigger()
    {
        enemy.AnimationTrigger(EnemyAnimationTriggerType.EnemyAttackEnd);
    }
}
