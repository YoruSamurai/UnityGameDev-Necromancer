using UnityEngine;
using TMPro;
using DG.Tweening;
using static RoomGraphGenerator;
using System.Collections.Generic;

namespace Yoru
{
    public static class GameObjectExtensions
    {
        public static T GetOrAdd<T> (this GameObject gameObject) where T : Component
        {
            T conponent = gameObject.GetComponent<T>();
            if(!conponent)  conponent = gameObject.AddComponent<T>();
            return conponent;
        }
    }

    public static class yoruUtils
    {
        
        

        public static void JumpNumber(int number, GameObject target)
        {
            if (target == null) return;

            // 创建文本对象
            GameObject textObj = new GameObject("FloatingNumber");
            TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
            tmp.text = number.ToString();
            tmp.fontSize = 4;
            tmp.color = Color.red;
        
            tmp.alignment = TextAlignmentOptions.Center;

            // 设置Sorting Layer & Order
            MeshRenderer renderer = tmp.GetComponent<MeshRenderer>();
            renderer.sortingLayerName = "Enemy"; // 2D的Sorting Layer名字
            renderer.sortingOrder = 5; // 层级数值（越大越靠前）

            // 设为 target 的子物体
            textObj.transform.SetParent(target.transform);

            // 设置初始位置（相对 target）
            textObj.transform.localPosition = new Vector3(-0.3f, 1f, 0); // 目标物体上方

            // 抛物线动画（向上跳跃）
            float jumpPower = 1.5f;
            float duration = 0.3f;
            Vector3 jumpTarget = textObj.transform.localPosition + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1f, .3f), 0);

            textObj.transform.DOLocalJump(jumpTarget, jumpPower, 1, duration)
                .SetEase(Ease.OutQuad);

            // 淡出效果并销毁
            tmp.DOFade(0, 0.3f).SetDelay(0.3f).OnComplete(() => GameObject.Destroy(textObj));
        }
    }

}
