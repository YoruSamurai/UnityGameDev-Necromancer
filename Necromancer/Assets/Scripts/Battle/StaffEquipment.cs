using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffEquipment : BaseEquipment
{

    [Header("远程武器设置")]
    public int fullComboAttackTimes;//连击次数
    [Header("投射预制体")]
    public GameObject projectilePrefab;   // 弹丸预制体
    public List<StaffAttackStruct> staffAttacks;


    protected override void Start()
    {
        base.Start();
        // 使用 as 关键字进行类型转换
        StaffEquipmentSO staffEquipmentSO = equipmentSO as StaffEquipmentSO;
        if (staffEquipmentSO != null)
        {
            fullComboAttackTimes = staffEquipmentSO.fullComboAttackTimes;
            staffAttacks = staffEquipmentSO.staffAttacks;
            projectilePrefab = staffEquipmentSO.projectilePrefab;
        }
        else
        {
            Debug.LogWarning("The equipmentSO is not a MeleeEquipmentSO");
        }


    }


    protected override void Update()
    {
        base.Update();
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

    public override void DoDamage(float _cMag, MonsterStats monsterStats)
    {
        base.DoDamage(_cMag, monsterStats);
    }

    public override void UseEquipment()
    {
        base.UseEquipment();
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

    protected void ResetCombo()
    {
        Debug.Log("当前combo为" + currentCombo);
        SetAttackCooldown(staffAttacks[currentCombo].attackTime);

        // 只有一段连击的情况
        if (staffAttacks.Count == 1)
        {
            Debug.Log("只有一段连击，重置combo");
            SetCombo(0);
            SetComboBreakTime(0);
            return;
        }

        //重置combo
        if (comboBreakTimer > 0)
        {

            Debug.Log(staffAttacks.Count);
            if (currentCombo + 1 == staffAttacks.Count)
            {
                Debug.Log("连击结束");
                SetCombo(0);
                SetComboBreakTime(0);
            }
            else
            {
                Debug.Log("进入下一段连击");
                SetCombo(currentCombo + 1);
                SetComboBreakTime(staffAttacks[currentCombo - 1].comboBreakTime);
            }
        }
        else
        {
            Debug.Log("开始连击");
            SetCombo(currentCombo + 1);
            SetComboBreakTime(staffAttacks[currentCombo - 1].comboBreakTime);
        }

    }
    #endregion


}
