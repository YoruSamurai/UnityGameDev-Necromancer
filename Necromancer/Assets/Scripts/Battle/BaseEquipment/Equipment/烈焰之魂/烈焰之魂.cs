using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 烈焰之魂 : StaffEquipment
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("烈焰之魂，启动！");

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



        //这里就可以开始写攻击的各种逻辑了
        Attack();
/*
        FireProjectile();
        //CheckHit();*/

        // 攻击后重置combo和冷却时间
        ResetCombo();
    }




    public void HandleProjectileHit(MonsterStats monsterStats)
    {
        PlayerStats.Instance.OnPlayerHit(this, monsterStats);
    }

    public override void FireProjectile()
    {
        base.FireProjectile();
        GameObject projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        烈焰之魂子弹 projectileScript = projectile.GetComponent<烈焰之魂子弹>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(this, staffAttacks[currentCombo],GetFacingDirection());
        }
    }

    protected void Attack()
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
        if (monsterStats.isInPoison() || monsterStats.isInBleed())
        {
            canCrit = true;
            Debug.Log("敌人在毒状态/流血状态 造成暴击");
        }


        int dmg = (int)(currentDmg * _cMag * (canCrit ? critMag : 1));
        Debug.Log($"暴击{canCrit},造成伤害{dmg}");
        monsterStats.TakeDirectDamage(dmg);

    }
}
