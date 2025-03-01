using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 施虐者匕首 : MeleeEquipment
{

    protected override void Start()
    {
        base.Start();
        Debug.Log("匕首，启动！");

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

        // 触发攻击特效（蓝色闪烁）
        TriggerAttackEffect();

        //这里就可以开始写攻击的各种逻辑了
        Attack();

        // 可以攻击的时候 重置combo和冷却时间
        ResetCombo();
    }


    public override void TriggerHitCheck()
    {
        CheckHit();
    }


    private void CheckHit()
    {
        Collider2D[] hits = GetHitEnemy();

        if (hits.Length > 0)
        {
            foreach (Collider2D hit in hits)
            {
                MonsterStats monsterStats = hit.GetComponent<MonsterStats>();
                PlayerStats.Instance.OnHit(this, monsterStats);
            }
        }
        else
        {
            Debug.Log("未命中敌人");
        }
    }

    private void Attack()
    {
        PlayerStats.Instance.OnAttack();
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
