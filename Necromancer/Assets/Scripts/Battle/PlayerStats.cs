using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, ISaveableGameData
{

    public static PlayerStats Instance { get; private set; }

    [SerializeField] public int currentHealth;
    [SerializeField] public int maxHealth;
    [SerializeField] public float healthPercentage;


    [SerializeField] private AbilityInvoker abilityInvoker;


    [SerializeField] public int strLevel;
    [SerializeField] public float strPercentage;
    [SerializeField] public int agileLevel;
    [SerializeField] public float agilePercentage;
    [SerializeField] public int magicLevel;
    [SerializeField] public float magicPercentage;

    [Header("灵魂 金币数")]
    [SerializeField] public int soul;
    [SerializeField] public int gold;

    /// <summary>
    /// 查看够不够钱做接下来的操作
    /// </summary>
    /// <returns></returns>
    public bool IsGoldLimit()
    {
        return false;
    }

    //是否可以攻击
    [SerializeField] public bool isAttacking;
    [SerializeField] public bool isParrying;
    [SerializeField] public bool isDefensing;
    [SerializeField] public bool canInterrupt;
    [SerializeField] public int currentEquipmentIndex;

    

    [SerializeField] public LayerMask whatIsEnemy;

    /// <summary>
    /// 一个新的节点 在库存里但不在装备栏的BaseEquipment都存在这里
    /// </summary>
    public Transform inventoryEquipmentParent;

    public Transform mainWeaponParent; // 在Inspector中指定主武器父对象
    public Transform secondaryWeaponParent; // 在Inspector中指定副武器父对象
    public BaseEquipment baseEquipment1;
    public BaseEquipment baseEquipment2;


    [Header("技能挂载点")]
    public Transform primarySkillParent;
    public Transform secondarySkillParent;
    public SkillController primarySkill;
    public SkillController secondarySkill;

    [SerializeField] public Player player;
    [SerializeField] public PlayerDetection playerDetection;

    public string SaveID => "PlayerStats"; // 唯一标识符

    public void SaveData(GameData data)
    {
        data.playerData = new PlayerData
        {
            currentHealth = this.currentHealth,
            maxHealth = this.maxHealth,
            soul = this.soul,
            gold = this.gold,
            position = new SerializableVector2(transform.position) // 保存位置
            // 其他需要保存的字段
        };
    }

    public void LoadData(GameData data)
    {
        if (data.playerData != null)
        {
            currentHealth = data.playerData.currentHealth;
            maxHealth = data.playerData.maxHealth;
            soul = data.playerData.soul;
            gold = data.playerData.gold;
            Vector2 loadedPosition = data.playerData.position.ToVector2(); // 加载位置
            transform.position = new Vector3(loadedPosition.x, loadedPosition.y, transform.position.z); // 保持 z 坐标不变
            // 其他字段加载
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        //EventManager.Instance.AddListener(EventName.PlayerAttack, Test);
    }

    private void OnDestroy()
    {
        //EventManager.Instance.RemoveListener(EventName.PlayerAttack, Test);
    }

    private void Test(object sender, EventArgs e)
    {
        Debug.Log("攻击中");
        var data = e as PlayerDeadEventArgs;
        if (data != null)
        {
            Debug.Log(data.playerName);
        }
    }


    private void Start()
    {
        SaveManager.Instance.RegisterGameData(this);
        currentHealth = 100;
        maxHealth = 100;
        healthPercentage = 100;
        strLevel = 1;
        strPercentage = 100;
        agileLevel = 1;
        agilePercentage = 100;
        magicLevel = 1;
        magicPercentage = 100;
        player = GetComponent<Player>();
        playerDetection = GetComponent<PlayerDetection>();
        soul = 100;
        gold = 50000;

        abilityInvoker = GetComponentInChildren<AbilityInvoker>();

        canInterrupt = true;
    }

    

    private void Update()
    {
        if(Time.timeScale < .3f)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerDetection.PickUp();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !player.IsUsingEquipment() && !player.isBusy && baseEquipment1 != null)
        {
            SetCurrentEquipmentIndex(1);
            this.TriggerEvent(EventName.PlayerAttack, new PlayerDeadEventArgs
            {
                playerName = "asfaf"
            });
            baseEquipment1.UseEquipment();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !player.IsUsingEquipment() && !player.isBusy && baseEquipment2 != null)
        {
            SetCurrentEquipmentIndex(2);
            baseEquipment2.UseEquipment();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            BaseEquipment baseEquipment = BattleManagerTest.Instance.GetRandomWeapon(mainWeaponParent);
            if (baseEquipment != null)
            {
                if(baseEquipment1 != null) { baseEquipment1.OnUnequip(); }
                    
                baseEquipment1 = baseEquipment;
                baseEquipment1.OnEquip();
                InventoryManager.Instance.AddToInventory(baseEquipment1);
            }
            BaseEquipment equipment = BattleManagerTest.Instance.GetRandomWeapon(secondaryWeaponParent);
            if (equipment != null)
            {
                if (baseEquipment2 != null) { baseEquipment2.OnUnequip(); }
                baseEquipment2 = equipment;
                baseEquipment2.OnEquip();
                InventoryManager.Instance.AddToInventory(baseEquipment2);
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            primarySkill = BattleManagerTest.Instance.GetRandomSkill(primarySkillParent);
            secondarySkill = BattleManagerTest.Instance.GetRandomSkill(secondarySkillParent);
        }

        // 按下 Q 释放主技能，E 释放副技能
        if (Input.GetKeyDown(KeyCode.Q))
        {
            primarySkill.CastSkill(player, this);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            secondarySkill.CastSkill(player, this);
        }
    }

    #region 一些要被重构的方法

    public bool GetFacingDirection()
    {
        return gameObject.transform.localEulerAngles.y < 90;
    }

    public void ChangeToAttackState()
    {
        player.ChangeStateByPlayerStats(player.primaryAttack);
    }

    public void ChangeToParryState()
    {
        player.ChangeStateByPlayerStats(player.parryState);
    }

    public void ChangeToDefenseState()
    {
        player.ChangeStateByPlayerStats(player.defenseState);
    }

    public void SetCurrentEquipmentIndex(int _equipmentIndex)
    {
        this.currentEquipmentIndex = _equipmentIndex;
        return;
    }
    public int GetCurrentEquipmentIndex(int _equipmentIndex)
    {
        return this.currentEquipmentIndex;
    }

    #endregion

    #region 生命之泉升级

    public void UpgradeStr(float strPercent,float healthPercent)
    {
        // 提升力量等级
        strLevel++;

        // 增加力量百分比
        strPercentage *= strPercent;

        healthPercentage *= healthPercent;

        maxHealth = (int)(maxHealth * healthPercent);
        currentHealth = (int)(currentHealth * healthPercent);
        Debug.Log($"力量等级提升到 {strLevel}%");

    }

    public void UpgradeAgile(float agilePercent, float healthPercent)
    {
        // 提升力量等级
        agileLevel++;

        // 增加力量百分比
        agilePercentage *= agilePercent;

        healthPercentage *= healthPercent;

        maxHealth = (int)(maxHealth * healthPercent);
        currentHealth = (int)(currentHealth * healthPercent);
        Debug.Log($"力量等级提升到 {agileLevel}%");

    }

    public void UpgradeMagic(float magicPercent, float healthPercent)
    {
        // 提升力量等级
        magicLevel++;

        // 增加力量百分比
        magicPercentage *= magicPercent;

        healthPercentage *= healthPercent;

        maxHealth = (int)(maxHealth * healthPercent);
        currentHealth = (int)(currentHealth * healthPercent);
        Debug.Log($"力量等级提升到 {magicLevel}%");

    }

    #endregion

    #region 装备升级雕像

    public void UpgradeWeapon(int index)
    {
        if(index == 1)
        {
            baseEquipment1.UpgradeLevel();
        }
        else if(index == 2)
        {
            baseEquipment2.UpgradeLevel();
        }
    }

    #endregion


    #region 玩家进行攻击 玩家命中 玩家被命中（无防御） 玩家被命中（防御中） 玩家被命中（格挡） 玩家翻滚中 玩家死亡了
    public void OnAttack()
    {
        //Debug.Log("进入攻击词条");

    }
    /// <summary>
    /// 在玩家命中敌人的时候会调用这个逻辑！进行词条的判定 能力的判定 最后判定伤害
    /// </summary>
    /// <param name="_baseEquipment"></param>
    /// <param name="_monsterStats"></param>
    public void OnPlayerHit(BaseEquipment _baseEquipment, MonsterStats _monsterStats)
    {
        //Debug.Log("进入命中词条");
        //float mag = BattleTriggerClass.Instance.TriggerOnHit(_baseEquipment , _monsterStats);
        float start = Time.realtimeSinceStartup;
        float abilityDamageMag = abilityInvoker.InvokeOnPlayerHit(_baseEquipment, _monsterStats);

        float duration = Time.realtimeSinceStartup - start;
        UnityEngine.Debug.Log($"OnPlayerHit 耗时: {duration} s");
        _baseEquipment.DoDamage(abilityDamageMag, _monsterStats);

    }

    /// <summary>
    /// 在玩家被敌人命中的时候会调用这个逻辑
    /// </summary>
    /// <param name="_monsterStats"></param>
    /// <param name="damageToPlayer"></param>
    public void OnHitted(MonsterStats _monsterStats,float damageToPlayer)
    {
        //伤害降低的倍率
        float abilityReduceMag = abilityInvoker.InvokeOnPlayerHitted(_monsterStats);
        Debug.Log(damageToPlayer + " " + abilityReduceMag);
    }
    public void OnKillMonster(MonsterStats _monsterStats)
    {
        abilityInvoker.InvokeOnKillMonster(_monsterStats);
    }

    public void OnDefense()
    {

    }

    public void OnPlayerParry(BaseEquipment _baseEquipment, MonsterStats _monsterStats)
    {
        abilityInvoker.InvokeOnPlayerParry(_baseEquipment, _monsterStats);
    }

    public void OnRoll()
    {

    }

    public void OnDeath()
    {

    }


    #endregion


}
