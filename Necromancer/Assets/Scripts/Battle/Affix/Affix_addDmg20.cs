using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//直接加20%伤害，作为前提条件
public class Affix_addDmg20 : BaseAffix
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

    }

    public override void Initialize(BaseEquipment _baseEquipment)
    {
        base.Initialize(_baseEquipment);
        EventManager.Instance.AddListener(EventName.OnPlayerHit, InvokeOnPlayerHit);

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
        EventManager.Instance.RemoveListener(EventName.OnPlayerHit, InvokeOnPlayerHit);

    }

    public override void InvokeOnPlayerHit(object sender, EventArgs e)
    {
        base.InvokeOnPlayerHit(sender, e);
        OnPlayerHitEventArgs data = e as OnPlayerHitEventArgs;

        if (baseEquipment == data.baseEquipment)
        {
            Debug.Log("我是 baseEquipment 的子物体！");
        }
        else
        {
            Debug.Log("我不是 baseEquipment 的子物体！");
        }
    }

    public override void InvokeOnPlayerHitted(object sender, EventArgs e)
    {
        base.InvokeOnPlayerHitted(sender, e);
    }

    public override void InvokeOnKillMonster(object sender, EventArgs e)
    {
        base.InvokeOnKillMonster(sender, e);
    }

    public override void InvokeOnPlayerCrit(object sender, EventArgs e)
    {
        base.InvokeOnPlayerCrit(sender, e);
    }

    public override void InvokeOnPlayerParry(object sender, EventArgs e)
    {
        base.InvokeOnPlayerParry(sender, e);
    }

    public override void InvokeOnPlayerCombo(object sender, EventArgs e)
    {
        base.InvokeOnPlayerCombo(sender, e);
    }


}
