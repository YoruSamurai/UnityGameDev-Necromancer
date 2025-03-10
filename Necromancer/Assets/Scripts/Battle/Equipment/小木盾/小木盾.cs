using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 小木盾 : ShieldEquipment
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("小木盾，启动！");

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void UseEquipment()
    {
        // 检查是否处于攻击CD中
        if (!GetCanUseEquipment())
        {
            Debug.Log("攻击还在冷却中！");
            return;
        }
        base.UseEquipment();

        

        // 可以攻击的时候 重置combo和冷却时间
        ResetCombo();
    }


   
    


    
}
