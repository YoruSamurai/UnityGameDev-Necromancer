using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : MonoBehaviour, IPickableItem,IEquipableItem
{

    [SerializeField] public EquipmentSO equipmentSO;

    [SerializeField] protected Player player;

    [Header("攻击冷却时间")]
    [SerializeField] public float attackCooldown; // 每次攻击CD时间
    [SerializeField] public float attackCooldownTimer;

    [Header("装备基础信息")]
    [SerializeField] public string equipmentName;
    [SerializeField] public int baseDmg;
    [SerializeField] public Sprite equipmentSprite;
    [SerializeField] public EquipmentType equipmentType;
    [SerializeField] public List<EquipmentTag> equipmentTags = new List<EquipmentTag>();
    [SerializeField] public int baseCritChance;
    [SerializeField] public float critMag;
    [SerializeField] public string equipmentDesc;

    [Header("连击管理器")]
    [SerializeField] public float comboBreakTimer;
    [SerializeField] public int currentCombo;

    [Header("造成伤害时的参数")]
    [SerializeField] public int currentDmg;
    [SerializeField] public bool canCrit;

    [Header("装备等级 词缀 基于等级伤害")]
    [SerializeField] public int equipmentLevel;
    [SerializeField] public float eDamageMag;
    [SerializeField] public List<BaseAffix> equipmentAffixList;


    public void Initialize()
    {
        SetupEquipmentBase();
        SetUpEquipmentLevel();
        attackCooldownTimer = 0f;
        player = PlayerStats.Instance.gameObject.GetComponent<Player>();
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {

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

    private void SetUpEquipmentLevel()
    {
        //在这里 可能可以去通过一个manager根据地图等级 进行一个随机值的获取 从而设定等级和基于等级的eDamageMag
        // 随机生成 1-5 级
        equipmentLevel = UnityEngine.Random.Range(1, 6);

        // 根据等级设置伤害倍率
        switch (equipmentLevel)
        {
            case 1:
                eDamageMag = 1.0f;
                break;
            case 2:
                eDamageMag = 1.1f;
                break;
            case 3:
                eDamageMag = 1.25f;
                break;
            case 4:
                eDamageMag = 1.4f;
                break;
            case 5:
                eDamageMag = 1.6f;
                break;
            default:
                eDamageMag = 1.0f;
                break;
        }

        // 输出调试信息，方便查看
        Debug.Log($"装备等级: {equipmentLevel}, 伤害倍率: {eDamageMag}");

    }

    public void UpgradeLevel()
    {
        equipmentLevel += 1;
        switch (equipmentLevel)
        {
            case 1:
                eDamageMag = 1.0f;
                break;
            case 2:
                eDamageMag = 1.1f;
                break;
            case 3:
                eDamageMag = 1.25f;
                break;
            case 4:
                eDamageMag = 1.4f;
                break;
            case 5:
                eDamageMag = 1.6f;
                break;
            case 6:
                eDamageMag = 1.8f;
                break;
            case 7:
                eDamageMag = 2.1f;
                break;
            case 8:
                eDamageMag = 2.25f;
                break;
            case 9:
                eDamageMag = 2.4f;
                break;
            case 10:
                eDamageMag = 2.6f;
                break;
            default:
                eDamageMag = 1.0f;
                break;
        }
    }

    private void SetupEquipmentBase()
    {
        equipmentName = equipmentSO.equipmentName;
        baseDmg = equipmentSO.baseDmg;
        currentDmg = equipmentSO.baseDmg;
        equipmentSprite = equipmentSO.equipmentSprite;
        equipmentType = equipmentSO.equipmentType;
        equipmentTags = equipmentSO.equipmentTags;

        baseCritChance = equipmentSO.baseCritChance;
        critMag = equipmentSO.critMag;

        equipmentDesc = equipmentSO.equipmentDesc;

        attackCooldownTimer = 0f;

        Debug.Log(this.equipmentDesc);
    }

    public virtual void UseEquipment()
    {
        
    }

    public virtual void TriggerHitCheckStart()
    {
        // 供子类重写
    }

    public virtual void TriggerHitCheckEnd()
    {

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
        PlayerStats.Instance.isAttacking = true;
    }

    public void SetShieldCooldown(float _timer)
    {
        attackCooldown = _timer;
        attackCooldownTimer = _timer;
    }

    public bool GetCanUseEquipment()
    {
        if (attackCooldownTimer > 0f)
            return false;
        return true;
    }


    #endregion

    #region IPickableItem
    public string GetItemName()
    {
        return equipmentName + "LV" + equipmentLevel;
    }

    public string GetItemMessage()
    {
        return equipmentDesc;
    }

    public Sprite GetSprite()
    {
        return equipmentSprite;
    }

    public void OnPickup()
    {
        Debug.Log("装备被拾取");
        //在捡起装备的时候 调用BattleBattleManagerTest的DropCurrentEquipment
        BattleManagerTest.Instance.DropCurrentEquipment();
        BaseEquipment instance = Instantiate(this, PlayerStats.Instance.mainWeaponParent);
        PlayerStats.Instance.baseEquipment1 = instance;
        InventoryManager.Instance.AddToInventory(instance);
    }


    #endregion

    #region IEquipableItem
    public InventoryMessage GetEquipableItemMessage()
    {
        InventoryMessage inventoryMessage = new InventoryMessage();
        inventoryMessage.sprite = equipmentSprite;
        inventoryMessage.itemLevel = equipmentLevel;
        inventoryMessage.itemName = equipmentName;
        inventoryMessage.itemDesc = equipmentDesc;
        inventoryMessage.itemAffix = "先占位";
        return inventoryMessage;
    }

    public void AddToInventory()
    {

    }

    public void RemoveFromInventory()
    {

    }

    public void OnEquip()
    {

    }

    public void OnUnequip()
    {

    }

    public string GetEquipableItemName()
    {
        return equipmentName;
    }





    #endregion
}
