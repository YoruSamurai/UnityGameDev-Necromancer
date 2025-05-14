using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 猛攻！在受到伤害2s内 造成伤害50倍 受到伤害变成0.1倍！
/// </summary>
public class Ability_猛攻 : BaseAbility
{

    private Coroutine abilityCoroutine;

    [SerializeField] private bool isCoroutineRunning;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.AddListener(EventName.OnPlayerHitted, InvokeOnPlayerHitted);

    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventManager.Instance.RemoveListener(EventName.OnPlayerHitted, InvokeOnPlayerHitted);

    }

    public override void InvokeOnPlayerHit(object sender, EventArgs e)
    {
        Debug.Log("猛攻协程攻击被触发");
        OnPlayerHitEventArgs data = e as OnPlayerHitEventArgs;
        Debug.Log(data.baseEquipment);
        //触发的这个方法 直接给invoker x50;
        abilityInvoker.MultiplyDamageMag(50f);

    }

    public override void InvokeOnPlayerHitted(object sender, EventArgs e)
    {
        OnPlayerHittedEventArgs data = e as OnPlayerHittedEventArgs;

        // 如果协程已在运行，先停止它
        if (abilityCoroutine != null)
        {
            // 先移除监听
            EventManager.Instance.RemoveListener(EventName.OnPlayerHit, InvokeOnPlayerHit);

            // 触发减伤逻辑
            abilityInvoker.MultiplyReduceMag(0.1f);

            // 停止旧协程
            StopCoroutine(abilityCoroutine);
            abilityCoroutine = null;
            Debug.Log(abilityCoroutine + "Asdadasa");
        }
        abilityCoroutine = StartCoroutine(受伤后加攻击防御协程());

    }

    private IEnumerator 受伤后加攻击防御协程()
    {
        EventManager.Instance.AddListener(EventName.OnPlayerHit, InvokeOnPlayerHit);

        // 等待 10 秒（游戏时间，受 Time.timeScale 影响）
        yield return new WaitForSeconds(2f);

        EventManager.Instance.RemoveListener(EventName.OnPlayerHit, InvokeOnPlayerHit);

        // 结束时清空协程引用（表示协程已停止）1
        abilityCoroutine = null;
    }
}
