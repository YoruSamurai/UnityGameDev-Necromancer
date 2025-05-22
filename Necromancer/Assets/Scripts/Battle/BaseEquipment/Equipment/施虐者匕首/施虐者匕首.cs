using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 施虐者匕首 : MeleeEquipment
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.layer == 6 && !isHitInAttack)
        {
            Debug.Log("是敌人！");
            isHitInAttack = true;
            
        }

        MonsterStats monsterStats = collision.GetComponent<MonsterStats>();
        PlayerStats.Instance.OnPlayerHit(this, monsterStats);

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
    }


    public override void DoDamage(float _cMag, MonsterStats monsterStats)
    {
        base.DoDamage(_cMag, monsterStats);
        canCrit = false;
        if (UnityEngine.Random.Range(0, 100) < baseCritChance)
        {
            canCrit = true;

        }
        if(monsterStats.isInPoison() || monsterStats.isInBleed())
        {
            canCrit = true;
            Debug.Log("敌人在毒状态/流血状态 造成暴击");
        }


        int dmg = (int)(currentDmg * _cMag * (canCrit ? critMag : 1));
        monsterStats.TakeDirectDamage(dmg);
        Debug.Log($"暴击{canCrit},造成伤害{dmg}");
    }
}
