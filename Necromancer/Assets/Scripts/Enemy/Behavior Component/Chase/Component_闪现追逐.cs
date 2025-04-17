using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Chase/闪现追逐")]
public class Component_闪现追逐 : EnemyBehaviorComponent
{
    private bool teleportStarted = false;

    private Vector3 teleportPosition; 

    public override void OnEnter()
    {
        // 计算瞬移方向：朝向玩家
        teleportPosition = playerTransform.position;

        enemy.anim.SetBool("Flash", true); // 播放闪现起始动画

        teleportStarted = true;
    }

    public override void OnUpdate()
    {
        Debug.Log(123);
        // 确保在闪现期间不会受到外界干扰
        if (teleportStarted)
        {
            enemy.SetVelocity(0, 0);
        }
    }

    public override void OnAnimationTrigger(EnemyAnimationTriggerType triggerType)
    {
        // 在闪现动画播放到一半时触发
        if (triggerType == EnemyAnimationTriggerType.EnemyFlashMid)
        {
            Teleport();
        }
        // 闪现动画结束后触发
        else if (triggerType == EnemyAnimationTriggerType.EnemyFlashEnd)
        {
            enemy.anim.SetBool("Flash", false);
            teleportStarted = false;
            // 切换到攻击状态
            enemy.stateMachine.ChangeState(enemy.chaseState);
        }
    }

    private void Teleport()
    {
        Vector3 teleportTarget = teleportPosition;
        enemy.transform.position = teleportTarget;

        Debug.Log("闪现追逐: 已闪现");
    }
}
