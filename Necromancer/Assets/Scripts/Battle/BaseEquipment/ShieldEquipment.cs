using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEquipment : BaseEquipment
{
    //盾牌武器要什么属性？   一次combo（盾牌也可能连击？哈哈）的总次数 每次攻击的：时长time 伤害倍率mag 晕值
    //伤害box又是另一个麻烦的故事了 比如说 形状是一个枚举 以玩家为中心还是以玩家面前1/2box为中心又是一个枚举？ 半径？长？宽？
    [SerializeField] public int fullComboAttackTimes;//一次combo的总攻击次数
    [SerializeField] public List<ShieldAttackStruct> shieldAttacks;

    [SerializeField] private float holdThreshold; // 长按阈值
    [SerializeField] private float holdTimer = 0f;
    private bool isHolding = false;
    private bool inputStarted = false;
    private bool isProcessingInput = false; // 标记是否正在处理盾牌输入
    private int currentWeaponIndex;

    protected override void Start()
    {
        base.Start();
        // 使用 as 关键字进行类型转换
        ShieldEquipmentSO shieldEquipmentSO = equipmentSO as ShieldEquipmentSO;
        if (shieldEquipmentSO != null)
        {
            fullComboAttackTimes = shieldEquipmentSO.fullComboAttackTimes;
            shieldAttacks = shieldEquipmentSO.shieldAttacks;
            holdThreshold = shieldEquipmentSO.holdThreshold;
        }
        else
        {
            Debug.LogWarning("The equipmentSO is not a shieldAttacksEquipmentSO");
        }

    }

    protected override void Update()
    {
        base.Update();
        if (isProcessingInput)
        {
            // 持续按住时累加计时
            if (currentWeaponIndex == 1? 
                Input.GetKey(KeyCode.Mouse0) : Input.GetKey(KeyCode.Mouse1))
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdThreshold && !isHolding)
                {
                    isHolding = true;
                    ActivateDefense(); // 长按进入防御状态
                }
            }

            // 当按键松开时，根据是否长按来执行对应动作
            if (currentWeaponIndex == 1 ?
                Input.GetKeyUp(KeyCode.Mouse0) : Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (!isHolding)
                {
                    ActivateParry(); // 短按执行弹反
                }
                else
                {
                    DeactivateDefense(); // 长按松开退出防御
                }
                // 重置输入处理状态
                isProcessingInput = false;
                inputStarted = false;
                holdTimer = 0f;
                isHolding = false;
            }
        }
        if (comboBreakTimer > 0)
        {
            comboBreakTimer -= Time.deltaTime;
        }
        else
        {
            if (currentCombo == 0)
                return;
            SetCombo(0);
        }



    }

    // ShieldEquipment 的 UseEquipment 将处理输入来区分短按与长按
    public override void UseEquipment()
    {

        
        // 检查是否处于攻击CD中
        if (!GetCanUseEquipment())
        {
            Debug.Log("BASE攻击还在冷却1中！");
            return;
        }
        else
        {
            // 如果已经开始处理输入，就不重复初始化
            if (!isProcessingInput)
            {
                isProcessingInput = true;
                inputStarted = true;
                holdTimer = 0f;
                isHolding = false;
                currentWeaponIndex = PlayerStats.Instance.currentEquipmentIndex;
            }
        }
    }

    private void ActivateParry()
    {
        Debug.Log("ShieldEquipment: Parry activated");
        PlayerStats.Instance.ChangeToParryState();
        PlayerStats.Instance.isParrying = true;
        // 可以弹反的时候 重置combo和冷却时间
        ResetCombo();
        // 在这里触发弹反效果，比如播放弹反动画，产生短暂的无敌效果，甚至反击
        // 可以调用 PlayerStats 或 Player 的方法切换状态或播放动画
    }

    private void ActivateDefense()
    {
        Debug.Log("ShieldEquipment: Defense activated");
        PlayerStats.Instance.ChangeToDefenseState();
        PlayerStats.Instance.isDefensing = true;
        // 可以防御的时候 重置combo和冷却时间
        ResetCombo();
        // 进入防御状态：可以让玩家进入防御状态，减少伤害或完全免伤
        // 比如调用 Player.SetInvincible(true) 或 ChangeState 到防御状态
    }

    private void DeactivateDefense()
    {
        Debug.Log("ShieldEquipment: Defense deactivated");
        PlayerStats.Instance.isDefensing = false;
        PlayerStats.Instance.SetCurrentEquipmentIndex(0);
        PlayerStats.Instance.CallPlayerTrigger();
        // 退出防御状态：恢复正常状态
        // 比如调用 Player.SetInvincible(false) 或切换回默认状态
    }

    #region 连击和冷却时间相关函数

    public void SetComboBreakTime(float _breakTime)
    {
        comboBreakTimer = _breakTime;
    }

    public void SetCombo(int _combo)
    {
        currentCombo = _combo;
    }

    //在使用装备攻击的时候 设置combo数和冷却时间
    protected void ResetCombo()
    {
        Debug.Log("当前combo为" + currentCombo);
        //在这里不但要给武器设置 还要给PlayerStat（管理人物攻击的单例）设置一个锁
        if (PlayerStats.Instance.isDefensing)
        {
            SetShieldCooldown(shieldAttacks[currentCombo].attackTime);

            // 只有一段连击的情况
            if (shieldAttacks.Count == 1)
            {
                Debug.Log("只有一段连击，重置combo");
                SetCombo(0);
                SetComboBreakTime(0);
                return;
            }
            //重置combo
            if (comboBreakTimer > 0)
            {

                Debug.Log(shieldAttacks.Count);
                if (currentCombo + 1 == shieldAttacks.Count)
                {
                    Debug.Log("连击结束");
                    SetCombo(0);
                    SetComboBreakTime(0);
                }
                else
                {
                    Debug.Log("进入下一段连击");
                    SetCombo(currentCombo + 1);
                    SetComboBreakTime(shieldAttacks[currentCombo - 1].comboBreakTime);
                }
            }
            else
            {
                Debug.Log("开始连击");
                SetCombo(currentCombo + 1);
                SetComboBreakTime(shieldAttacks[currentCombo - 1].comboBreakTime);
            }
        }
        

    }
    #endregion

}
