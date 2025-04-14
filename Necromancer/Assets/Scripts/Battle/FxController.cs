using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxController : MonoBehaviour
{
    // 参考 Animator，可以在 Inspector 中绑定
    [SerializeField] private Animator anim;

    // 存储播放的动画片段长度
    [SerializeField] private float animLength = 0f;

    // 初始化方法，由外部传入动画片段、偏移量等参数
    public void Initialize(AnimationClip slashClip, Vector3 offset, bool facingRight)
    {
        // 将位置偏移叠加到当前位置上
        if (facingRight)
            transform.position += new Vector3(offset.x, offset.y);
        else
        {
            transform.position += new Vector3(-offset.x, offset.y);
            transform.Rotate(0, 180, 0);
        }
            

        // 获取自身 Animator
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("FXSlash 缺少 Animator 组件!");
            return;
        }

        // 创建一个新的 AnimatorOverrideController，替换 "Slash" 状态对应的动画片段
        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        // 假设原始状态名为 "Slash"
        aoc["FX"] = slashClip;
        anim.runtimeAnimatorController = aoc;

        // 播放动画，确保从头开始
        anim.Play("FX", 0, 0f);

        // 获取动画片段时长（clip.length）
        animLength = slashClip.length;


        // 启动协程，等待动画播放完成后销毁物体
        StartCoroutine(DestroyAfterAnimation(animLength));
    }

    private IEnumerator DestroyAfterAnimation(float duration)
    {
        // 等待指定的时长
        yield return new WaitForSeconds(duration);

        // 销毁当前物体
        Destroy(gameObject);
    }
}
