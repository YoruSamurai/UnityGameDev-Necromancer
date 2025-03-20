using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class 虚空行走 : BaseSkillComponent
{
    [Header("虚空行走参数")]
    public float dashDistance = 10f;          // 闪烁固定距离
    public float dashDelay = 0.2f;            // 技能启动后的延迟时间
    public float invincibleDuration = 2f;     // 无敌持续时间
    public Color dashColor = new Color(0.5f, 0, 0.5f, 0.5f); // 半透明紫色

    [Header("充能设置")]
    public float chargeInterval = 10f;  // 每隔10秒增加一次充能
    public int maxCharges = 3;          // 最大充能数
    private int currentCharges = 0;
    private float chargeTimer = 0f;

    public override void Execute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        // 检查是否有充能
        if (currentCharges <= 0)
        {
            Debug.Log("虚空行走技能没有充能，无法施放！");
            return;
        }

        OnSkillStart(player, skillData);

        // 消耗1个充能
        currentCharges--;
        Debug.Log("施放虚空行走，剩余充能：" + currentCharges);

        // 启动协程，在预备时间后执行位移和无敌效果
        StartCoroutine(DelayedVoidWalk(player, skillData));

        OnSkillEnd(player, skillData);
    }

    private IEnumerator DelayedVoidWalk(Player player, SkillSO skillData)
    {
        yield return new WaitForSeconds(dashDelay);

        // 获取玩家SpriteRenderer，保存原始颜色
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        Color originalColor = sr != null ? sr.color : Color.white;

        // 改变颜色，进入半透明紫色
        if (sr != null)
            sr.color = dashColor;

        // 计算目标位置：沿着玩家面向方向移动 dashDistance
        Vector2 targetPos = player.transform.position;
        targetPos.x += player.facingDir * dashDistance;

        // 目标检测：如果目标位置碰到Ground层，则继续向前一点（这里简单处理）
        LayerMask groundMask = LayerMask.GetMask("Ground");
        while (Physics2D.OverlapCircle(targetPos, 0.1f, groundMask) != null)
        {
            targetPos.x -= player.facingDir * 0.5f;
        }

        // 利用DOTween进行瞬移效果（闪烁）
        player.transform.DOMove(targetPos, 0.1f).SetEase(Ease.Linear);

        // 进入无敌状态
        player.SetInvincible(true);
        yield return new WaitForSeconds(invincibleDuration);
        player.SetInvincible(false);

        // 恢复原始颜色
        if (sr != null)
            sr.color = originalColor;
    }

    private void Update()
    {
        // 充能计时：每隔 chargeInterval 增加一格充能，最多 maxCharges
        chargeTimer += Time.deltaTime;
        if (chargeTimer >= chargeInterval)
        {
            chargeTimer -= chargeInterval;
            if (currentCharges < maxCharges)
            {
                currentCharges++;
                Debug.Log("虚空行走充能 +1, 当前充能: " + currentCharges);
            }
        }
    }
}
