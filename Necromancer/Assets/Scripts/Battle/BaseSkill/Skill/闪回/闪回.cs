using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class 闪回 : BaseSkillComponent
{
    [Header("闪回参数")]
    public float dashDistance = 10f;          // 正向冲刺距离
    public float dashDuration = 0.1f;          // 冲刺持续时间
    public float activationWindow = 3f;        // 第二次触发的时间窗口
    public float overallCooldown = 5f;         // 技能总体冷却时间

    private bool hasActivatedOnce = false;
    private float firstActivationTime = 0f;     // 第一次冲刺时间
    private float lastUseTime = -999f;          // 上次使用时间（初始-999确保一开始能用）
    private int initialDashDir;                 // 记录第一次冲刺方向

    /// <summary>
    /// 当技能被施放时执行。第一次执行时向前冲刺，
    /// 如果在激活窗口内再次施放，则沿第一次冲刺方向反向冲刺。
    /// </summary>
    public override void Execute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        // 冷却时间检查
        if (Time.time - lastUseTime < overallCooldown)
        {
            Debug.Log("Flashback: 技能冷却中，还需要等待 " + (overallCooldown - (Time.time - lastUseTime)).ToString("F1") + " 秒");
            return;
        }

        OnSkillStart(player, skillData);

        if (!hasActivatedOnce)
        {
            // 第一次施放：向前冲刺
            initialDashDir = player.facingDir;  // 记录第一次冲刺方向
            Dash(player, dashDistance * initialDashDir);
            hasActivatedOnce = true;
            firstActivationTime = Time.time;
            Debug.Log("Flashback: 第一次向前冲刺");
        }
        else
        {
            // 检查是否在激活窗口内
            if (Time.time - firstActivationTime <= activationWindow)
            {
                // 第二次触发，沿着第一次方向反向冲刺
                Dash(player, -dashDistance * initialDashDir);
                Debug.Log("Flashback: 在激活窗口内，沿第一次方向反向冲刺");

                // 重置状态，并开始冷却
                hasActivatedOnce = false;
                lastUseTime = Time.time;
            }
            else
            {
                // 超出激活窗口，重新执行正向冲刺
                initialDashDir = player.facingDir;  // 重置为当前面向方向
                Dash(player, dashDistance * initialDashDir);
                firstActivationTime = Time.time;
                Debug.Log("Flashback: 超出激活窗口，重新向前冲刺");
            }
        }

        OnSkillEnd(player, skillData);
    }

    /// <summary>
    /// 使用 DOTween 快速移动玩家到目标位置。
    /// </summary>
    private void Dash(Player player, float distance)
    {
        Vector2 startPos = player.transform.position;
        Vector2 direction = new Vector2(distance, 0);  // 冲刺方向

        // 用射线检测路径
        LayerMask groundMask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction.normalized, Mathf.Abs(distance), groundMask);

        Vector2 targetPos;

        if (hit.collider != null)
        {
            // 如果射线撞到障碍物，落点调整到障碍物前一点
            targetPos = hit.point - direction.normalized * 0.1f;
            Debug.Log("⚡ 闪回: 检测到障碍物，调整落点");
        }
        else
        {
            // 没有撞到障碍物，正常冲刺
            targetPos = startPos + direction;
        }

        // 启动残影特效
        StartCoroutine(CreateAfterImage(player));

        //使用 DOTween 进行瞬间移动，并在结束后重置速度
        player.transform.DOMove(targetPos, dashDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                player.SetVelocity(0,player.rb.velocity.y);
                Debug.Log("⚡ 闪回结束，速度归零");
            });

        Debug.DrawLine(startPos, targetPos, Color.red, 1f); // 可视化冲刺路径
    }

    private IEnumerator CreateAfterImage(Player player)
    {
        SpriteRenderer sprite = player.GetComponentInChildren<SpriteRenderer>();
        float afterImageDuration = 0.5f; // 残影存活时间
        float interval = 0.01f; // 残影生成间隔

        Color afterImageColor = new Color(1f, 1f, 1f, 0.5f); // 白色半透明残影
        List<GameObject> afterImages = new List<GameObject>();

        float timer = 0f;
        while (timer < dashDuration)
        {
            // 创建残影对象
            GameObject afterImage = new GameObject("AfterImage");
            SpriteRenderer afterImageRenderer = afterImage.AddComponent<SpriteRenderer>();

            // 复制玩家的Sprite
            afterImageRenderer.sprite = sprite.sprite;
            afterImageRenderer.sortingLayerID = sprite.sortingLayerID;
            afterImageRenderer.sortingOrder = sprite.sortingOrder - 1; // 残影在玩家身后
            afterImageRenderer.color = afterImageColor;

            // 设置残影位置
            afterImage.transform.position = player.transform.position;
            afterImage.transform.rotation = player.transform.rotation;

            // 残影淡出效果
            afterImageRenderer.DOFade(0, afterImageDuration).OnComplete(() => Destroy(afterImage));

            afterImages.Add(afterImage);

            yield return new WaitForSeconds(interval);
            timer += interval;
        }
    }
}
