using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : MonoBehaviour
{

    [SerializeField] public EquipmentSO equipmentSO;

    [SerializeField] public float attackCooldown; // 每次攻击CD时间
    [SerializeField] public float attackCooldownTimer;

    [SerializeField] public string equipmentName;
    [SerializeField] public int baseDmg;
    [SerializeField] public Sprite equipmentSprite;
    [SerializeField] public List<EquipmentTag> equipmentTags = new List<EquipmentTag>();
    [SerializeField] public int baseCritChance;
    [SerializeField] public float critMag;
    [SerializeField] public string equipmentDesc;


    [SerializeField] public int currentDmg;
    [SerializeField] public bool canCrit;


    [SerializeField] public int equipmentLevel;
    [SerializeField] public List<BaseAffix> equipmentAffixList;
    


    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupEquipmentBase();
        attackCooldownTimer = 0f;
    }

    

    // Update is called once per frame
    protected virtual void Update()
    {
        // 更新连击冷却计时器
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    private void SetupEquipmentBase()
    {
        equipmentName = equipmentSO.equipmentName;
        baseDmg = equipmentSO.baseDmg;
        currentDmg = equipmentSO.baseDmg;
        equipmentSprite = equipmentSO.equipmentSprite;
        equipmentTags = equipmentSO.equipmentTags;

        baseCritChance = equipmentSO.baseCritChance;
        critMag = equipmentSO.critMag;

        equipmentDesc = equipmentSO.equipmentDesc;

        attackCooldownTimer = 0f;

        Debug.Log(this.equipmentDesc);
    }

    public virtual void UseEquipment()
    {
        // 检查是否处于攻击CD中
        if (attackCooldownTimer > 0f)
        {
            Debug.Log("BASE攻击还在冷却中！");
            return;
        }
        else
        {
            PlayerStats.Instance.ChangeToAttackState();
        }
    }

    public virtual void DoDamage(float _cMag,MonsterStats monsterStats)
    {
        
    }

    public bool GetFacingDirection()
    {
        return PlayerStats.Instance.GetFacingDirection();
    }

    #region 连击和冷却时间相关函数

    public void SetAttackCooldown(float _timer)
    {
        attackCooldown = _timer;
        attackCooldownTimer = _timer;
    }

    public bool IsCanEquipment()
    {
        if (attackCooldownTimer > 0f)
            return false;
        return true;
    }
    #endregion


}
