using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class BaseEquipment : MonoBehaviour, IPickableItem,IEquipableItem
{

    [SerializeField] public EquipmentSO equipmentSO;

    [SerializeField] protected Player player;
    [SerializeField] public LayerMask enemyLayerMask;
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


    public void ClearEquipmentAffixList()
    {
        equipmentAffixList.Clear();
        foreach (Transform affix in transform)
        {
            Destroy(affix.gameObject);
        }
    }


    public void Initialize()
    {
        SetupEquipmentBase();
        SetUpEquipmentLevel();
        gameObject.name = equipmentName;//实例化的时候需要记住他的名字啊啊啊
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

    public void PlayHitSound(bool crit, MonsterStats monsterStats)
    {
        if(equipmentSO.critHitSfx != null && equipmentSO.normalHitSfx != null)
        {
            SoundData soundData = crit?equipmentSO.critHitSfx.GetSoundData():equipmentSO.normalHitSfx.GetSoundData();
            SoundManager.Instance.CreateSound()
            .WithSoundData(soundData)
            .WithPosition(player.transform.position)
            .Play();

        }
        else
        {
            Debug.LogWarning(equipmentName + "缺少命中音效！");
        }
        
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


    /// <summary>
    /// 捡起武器到背包的方法，首先去把背包里同样的东西丢出来，然后看看生成新物品到主手/副手/背包。
    /// </summary>
    public void OnPickup()
    {   
        Debug.Log("装备被拾取");

        //又要重构方法了，好累哦
        //捡起东西的时候不是丢下当前主武器的东西 而是丢下同名物品，那么我们在DropCurrentEquipment中 那是不是返回一个enum或者int比较好？
        //应该是去Inventory找到同名的物品，把它丢出来 然后如果这个东西在主副手 我们就 鹅 把它生成过去 然后是丢出inventory 和加入inventory。
        //在捡起装备的时候 调用BattleBattleManagerTest的DropCurrentEquipment
        int index = BattleManagerTest.Instance.DropSameEquipment(this);
        
        if(index == 4)
        {
            if(PlayerStats.Instance.baseEquipment1 == null)
            {
                index = 1;
            }
            else if(PlayerStats.Instance.baseEquipment2 == null)
            {
                index = 2;
            }
        }
        if(index == 1)
        {
            BaseEquipment instance = Instantiate(this, PlayerStats.Instance.mainWeaponParent);
            instance.gameObject.name = equipmentName;
            instance.OnEquip();
            PlayerStats.Instance.baseEquipment1 = instance;
            InventoryManager.Instance.AddToInventory(instance); 
        }
        else if(index == 2)
        {
            BaseEquipment instance = Instantiate(this, PlayerStats.Instance.secondaryWeaponParent);
            instance.gameObject.name = equipmentName;
            instance.OnEquip();
            PlayerStats.Instance.baseEquipment2 = instance;
            InventoryManager.Instance.AddToInventory(instance);
        }
        else
        {
            BaseEquipment instance = Instantiate(this, PlayerStats.Instance.inventoryEquipmentParent);
            instance.gameObject.name = equipmentName;
            instance.OnUnequip();
            InventoryManager.Instance.AddToInventory(instance);

        }
    }


    #endregion

    #region IEquipableItem

    /// <summary>
    /// 获取物品信息 InventoryMessage是一个结构 储存信息呃呃呃
    /// </summary>
    /// <returns></returns>
    public InventoryMessage GetEquipableItemMessage()
    {
        InventoryMessage inventoryMessage = new InventoryMessage();
        inventoryMessage.sprite = equipmentSprite;
        inventoryMessage.itemLevel = equipmentLevel;
        inventoryMessage.itemName = $"{equipmentName} LV{equipmentLevel}";
        inventoryMessage.itemDesc = equipmentDesc;

        inventoryMessage.itemAffix = string.Join("\n",
            equipmentAffixList
            .Where(a => a != null && !string.IsNullOrEmpty(a.affixDesc))
            .Select(a => a.affixDesc));
        return inventoryMessage;
    }

    public void AddToInventory()
    {

    }
    /// <summary>
    /// 把Item从背包里面丢出来
    /// </summary>
    public void DropFromInventory()
    {
        Debug.Log("尝试丢弃" + equipmentName);
        this.OnUnequip();
        BattleManagerTest.Instance.DropItem(this, PlayerStats.Instance.gameObject.transform.position);
    }


    public void EquipableItemLevelUp()
    {
        equipmentLevel += 1;
    }

    public void EquipableItemRecast()
    {
        /*equipmentAffixList.Clear();*/
        ClearEquipmentAffixList();
        equipmentAffixList.Add(BattleManagerTest.Instance.RecastEquipmentAffix(this));
    }

    public string GetEquipableItemName()
    {
        return equipmentName;
    }

    public int GetEquipableItemLevel()
    {
        return equipmentLevel;
    }

    public void OnEquip()
    {
        foreach(BaseAffix affix in equipmentAffixList)
        {
            affix.OnEquip();
        }
    }

    public void OnUnequip()
    {
        foreach (BaseAffix affix in equipmentAffixList)
        {
            affix.OnUnequip();
        }
    }





    #endregion
}
