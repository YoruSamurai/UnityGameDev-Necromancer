using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();


    private void AnimationTrigger(PlayerAnimationTriggerType triggerType)
    {
        player.AnimationTrigger(triggerType);
    }

}


#region 玩家动画枚举
public enum PlayerAnimationTriggerType
{
    PlayerAttackEnd,//玩家攻击结束的时候触发 对应AnimationEndTrigger 
    PlayerHitDetermineStart,//伤害判定开始判定
    PlayerHitDetermineEnd,//伤害判定结束
    PlayerOnShoot,//玩家开始射击！
    PlayerOnParry,//玩家开始招架
    PlayerOnDefense,//玩家开始防御
    PlayerLedgeUp,//玩家上墙最后一下起来
    PlayerAnimationEndTrigger,//结束当前状态
}
#endregion
