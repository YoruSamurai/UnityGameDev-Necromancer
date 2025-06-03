using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFxManager : MonoBehaviour
{
    public static PlayFxManager Instance;

    public ParticleSystem bloodLineParticle;

    [SerializeField] private GameObject fxControllerPrefab;

    private void Awake()
    {
        // 确保实例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 防止多个实例
        }
    }

    public void GenerateFX(AnimationClip clip, Transform spawner, bool facingRight, Vector2 offset)
    {
        GameObject obj = ObjectPoolManager.SpawnObject(fxControllerPrefab, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.FX);
        FxController controller = obj.GetComponent<FxController>();
        controller.Initialize(clip, spawner, facingRight, offset);
    }

    public void PlayBloodLine(Vector2 hitPoint, Vector2 attackerPosition)
    {
        Debug.Log("阿萨大大啊大苏打撒旦");
        // 计算方向
        Vector2 direction = (hitPoint - attackerPosition).normalized;

        // 生成血线粒子
        ParticleSystem bloodLineInstance = Instantiate(bloodLineParticle, hitPoint, Quaternion.identity);

        // 设置血线方向
        bloodLineInstance.transform.right = direction;



        // 播放粒子
        bloodLineInstance.Play();

        // 粒子结束后销毁
        Destroy(bloodLineInstance.gameObject, bloodLineInstance.main.duration + 0.5f);
    }
}
