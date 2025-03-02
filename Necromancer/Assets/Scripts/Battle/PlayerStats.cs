using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public static PlayerStats Instance { get; private set; }

    [SerializeField] public int health;

    //是否可以攻击
    [SerializeField] public bool isAttacking;
    [SerializeField] public int currentEquipmentIndex;

    [SerializeField] public BoxCollider2D attackCheck;

    [SerializeField] public LayerMask whatIsEnemy;


    public Transform mainWeaponParent; // 在Inspector中指定主武器父对象
    public Transform secondaryWeaponParent; // 在Inspector中指定副武器父对象
    public BaseEquipment baseEquipment1;
    public BaseEquipment baseEquipment2;

    [SerializeField] public Player player;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        health = 500;
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !isAttacking && !player.isBusy && baseEquipment1 != null)
        {
            SetCurrentEquipmentIndex(1);
            baseEquipment1.UseEquipment();
        }
        if (Input.GetKey(KeyCode.Mouse1) && !isAttacking && !player.isBusy && baseEquipment2 != null)
        {
            SetCurrentEquipmentIndex(2);
            baseEquipment2.UseEquipment();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            baseEquipment1 = BattleManagerTest.Instance.GetRandomWeapon(mainWeaponParent);
            baseEquipment2 = BattleManagerTest.Instance.GetRandomWeapon(secondaryWeaponParent);
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



    #region 玩家进行攻击 玩家命中 玩家被命中（无防御） 玩家被命中（防御中） 玩家被命中（格挡） 玩家翻滚中 玩家死亡了
    public void OnAttack()
    {
        //Debug.Log("进入攻击词条");
        BattleTriggerClass.Instance.TriggerOnAttack();

    }

    public void OnHit(BaseEquipment _baseEquipment, MonsterStats _monsterStats)
    {
        //Debug.Log("进入命中词条");
        BattleTriggerClass.Instance.TriggerOnHit(_baseEquipment , _monsterStats);


    }

    public void OnHitted()
    {

    }

    public void OnDefense()
    {

    }

    public void OnBlock()
    {

    }

    public void OnRoll()
    {

    }

    public void OnDeath()
    {

    }
    #endregion


}
