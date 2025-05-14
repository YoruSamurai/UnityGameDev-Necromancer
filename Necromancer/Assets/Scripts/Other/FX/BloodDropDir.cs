using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDropDir : MonoBehaviour
{
    [SerializeField] private float duration = 3f;       // 整个效果持续时间
    [SerializeField] private float targetAlpha = 0f;      // 最终透明度（0 表示完全透明）
    [SerializeField] private float moveDownDistance = 1f; // 向下移动的距离
    [SerializeField] private Color targetColor = new Color(0.3f, 0.5f, 1f); // 目标颜色（蓝白色调）

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("BloodEffect: 未找到 SpriteRenderer 组件！");
            return;
        }
        
        // 创建 DOTween 动画序列
        Sequence seq = DOTween.Sequence();

        // 动画1：颜色渐变到目标颜色，并同时渐隐到 targetAlpha
        // DOColor 会同时改变 RGB 和 Alpha，因此这里构造目标颜色
        Color finalColor = new Color(targetColor.r, targetColor.g, targetColor.b, targetAlpha);
        seq.Append(sr.DOColor(finalColor, duration));

        // 动画2：同时向下移动一定距离，模拟血浆沿重力流动的效果
        seq.Join(transform.DOMoveY(transform.position.y - moveDownDistance, duration)
                      .SetEase(Ease.InQuad));

        // 效果完成后自动销毁该对象
        seq.OnComplete(() => Destroy(gameObject));
    }
}
