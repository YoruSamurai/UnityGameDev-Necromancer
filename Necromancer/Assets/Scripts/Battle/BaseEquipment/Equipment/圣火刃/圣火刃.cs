using UnityEngine;

public class 圣火刃 : MeleeEquipment
{
    //
    protected override void Start()
    {
        base.Start();
        Debug.Log("圣火刃，启动！");
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
        int dmg = (int)(currentDmg * _cMag);
        Debug.Log(dmg);
        monsterStats.TakeDirectDamage(dmg);

    }
}