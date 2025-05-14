using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//直接加40%伤害，作为前提条件
public class Affix_addDmg40 : BaseAffix
{
    

    protected override void Start()
    {
        base.Start();
        //Debug.Log("词条+40%伤害，启动！");
    }


    /*public override void InvokeAffixPre(PlayerStats playerStats, MonsterStats monsterStats)
    {
        base.InvokeAffixPre(playerStats, monsterStats);
        Debug.Log("触发了" + this.affixDesc);

        // 触发伤害倍率增加
        OnModifyDamageMultiplier?.Invoke(1.4f); // 伤害倍率提高 20%

    }*/
}
