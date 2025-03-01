using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MeleeEquipment
{
    protected override void Start()
    {
        base.Start();
        Debug.Log("斧头，启动！");
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void UseEquipment()
    {
        // 检查是否处于攻击CD中
        if (attackCooldownTimer > 0f)
        {
            //Debug.Log("攻击还在冷却中！");
            return;
        }
        base.UseEquipment();

        // 触发攻击特效（蓝色闪烁）
        TriggerAttackEffect();

        //这里就可以开始写攻击的各种逻辑了
        Attack();
        CheckHit();

        // 攻击后重置combo和冷却时间
        ResetCombo();
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
        int dmg = (int)(currentDmg * _cMag);
        Debug.Log(dmg);

        //额外效果
        int poisonTotalDmg = (dmg / 4);
        float poisonDurantion = 10f;
        float poisonInterval = 1f;
        monsterStats.ApplyPoison(poisonTotalDmg/4, poisonDurantion,poisonInterval);
        monsterStats.TakeDirectDamage(dmg);

    }
}
