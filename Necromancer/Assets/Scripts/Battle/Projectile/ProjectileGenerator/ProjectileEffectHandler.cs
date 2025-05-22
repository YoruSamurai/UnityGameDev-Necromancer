using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffectHandler : MonoBehaviour
{
    private BaseProjectileGenerator generator;

    private ParticleSystem ps;
    public void Initialize(BaseProjectileGenerator _generator)
    {
        generator = _generator;
        StartEffect();
    }

    /// <summary>
    /// 开始效果的播放
    /// </summary>
    public void StartEffect()
    {

        ps = GetComponentInChildren<ParticleSystem>();
        Debug.Log("你在吗" + this.gameObject);
        Debug.Log(ps);
        if (ps != null)
        {
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Rectangle; // 形状改为矩形
                                                                 //shape.scale = new Vector3(10f, 10f, 1f); // 形状缩放 10x10

            var emission = ps.emission;
            //emission.rateOverTimeMultiplier *= 10f; // 发射率放大 10 倍
        }

        //Destroy(gameObject, 1f); // 1秒后销毁爆炸特效
    }

    /// <summary>
    /// 告诉generator 让他进行下一步处理
    /// </summary>
    public void EffectEnd()
    {

    }



}
