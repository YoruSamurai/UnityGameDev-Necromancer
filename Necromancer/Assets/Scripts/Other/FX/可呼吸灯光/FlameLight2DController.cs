using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlameLight2DController : MonoBehaviour
{
    [Header("颜色渐变")]
    public Gradient colorOverTime = new Gradient()
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.yellow, 0f),
            new GradientColorKey(new Color(1f, 0.5f, 0f), 0.5f),
            new GradientColorKey(Color.red, 1f)
        },
        alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        }
    };

    [Header("呼吸曲线（影响光照强度）")]
    public AnimationCurve intensityOverTime = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);

    [Header("基础设置")]
    [Min(0.1f)] public float breathDuration = 1f; // 一次完整呼吸的时间（秒）

    [Header("随机闪烁")]
    public bool enableFlicker = true;
    [Range(0f, 0.5f)] public float flickerStrength = 0.2f;
    [Min(0.1f)] public float flickerSpeed = 20f;

    private Light2D _light;
    private float _breathTimer = 0f;
    private float _baseIntensity;

    void Awake()
    {
        _light = GetComponent<Light2D>();
        _baseIntensity = _light.intensity;
    }

    void Update()
    {
        _breathTimer += Time.deltaTime / breathDuration;
        float t = Mathf.PingPong(_breathTimer, 1f);

        // 1. 渐变颜色
        _light.color = colorOverTime.Evaluate(t);

        // 2. 呼吸强度
        float baseValue = _baseIntensity * intensityOverTime.Evaluate(t);

        // 3. 随机闪烁
        if (enableFlicker)
        {
            float flicker = (Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * 2f - 1f) * flickerStrength;
            _light.intensity = Mathf.Max(0f, baseValue * (1f + flicker));
        }
        else
        {
            _light.intensity = baseValue;
        }
    }
}
