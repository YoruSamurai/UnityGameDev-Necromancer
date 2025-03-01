using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 均衡之刃 : MeleeEquipment
{
    //均衡之刃特有 连段伤害
    [SerializeField] private float dmgComboTimer;
    [SerializeField] private float currentDmgComboTimer;
    [SerializeField] private int maxDmgCombo;
    [SerializeField] private int currentDmgCombo;

    

    


    protected override void Start()
    {
        base.Start();
        Debug.Log("剑，启动！");
        dmgComboTimer = 3f;
        maxDmgCombo = 10;
        currentDmgCombo = 0;
        currentDmgComboTimer = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if(currentDmgComboTimer > 0)
        {
            // 更新连击计时器
            currentDmgComboTimer -= Time.deltaTime;
        }
        else
        {
            currentDmgCombo = 0;
        }
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

        // 攻击后重置攻击冷却计时器
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
            currentDmgComboTimer = dmgComboTimer;
            if (currentDmgCombo < maxDmgCombo)
            {
                currentDmg = (int)(baseDmg * (1 + (.5f * currentDmgCombo / maxDmgCombo)));
                currentDmgCombo += 1;
            }
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
        if(UnityEngine.Random.Range(0,100) < baseCritChance)
        {
            canCrit = true;
            
        }
        if(currentDmgCombo == maxDmgCombo)
        {
            canCrit = true;
            Debug.Log("10次攻击必定暴击");
        }
            
            
        int dmg = (int)(currentDmg * _cMag * (canCrit ? critMag : 1));
        Debug.Log($"暴击{canCrit},造成伤害{dmg}");
        monsterStats.TakeDirectDamage(dmg);

    }
}
