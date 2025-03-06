using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Enemy/Components/Chase/弓箭手闪现")]
public class Component_弓箭手闪现 : EnemyBehaviorComponent
{
    [SerializeField] private float teleportDistance = 20f; // 瞬移距离

    private bool teleportStarted = false;

    public override void OnEnter()
    {
        // 计算瞬移方向：朝向玩家
        int teleportDirection = playerTransform.position.x > enemy.transform.position.x ? 1 : -1;

        // 如果当前朝向不正确，则翻转
        if (teleportDirection != enemy.facingDir)
        {
            enemy.Flip();
        }

        enemy.anim.SetBool("Idle", false);
        enemy.anim.SetBool("Flash", true); // 播放闪现起始动画

        teleportStarted = true;
    }

    public override void OnUpdate()
    {
        // 确保在闪现期间不会受到外界干扰
        if (teleportStarted)
        {
            enemy.SetVelocity(0, 0);
        }
    }

    public override void OnAnimationTrigger(AnimationTriggerType triggerType)
    {
        // 在闪现动画播放到一半时触发
        if (triggerType == AnimationTriggerType.EnemyFlashMid)
        {
            Teleport();
        }
        // 闪现动画结束后触发
        else if (triggerType == AnimationTriggerType.EnemyFlashEnd)
        {
            enemy.anim.SetBool("Flash", false);
            teleportStarted = false;
            // 切换到攻击状态
            enemy.stateMachine.ChangeState(enemy.attackState);
        }
    }

    private void Teleport()
    {
        int teleportDirection = enemy.facingDir;
        Vector3 teleportTarget = enemy.transform.position + new Vector3(teleportDistance * teleportDirection, 0, 0);
        enemy.transform.position = teleportTarget;

        Debug.Log("弓箭手瞬移: 已瞬移");
    }
}

