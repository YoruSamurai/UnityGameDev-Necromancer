using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEquipment : BaseEquipment
{

    //近战武器要什么属性？   一次combo的总次数 每次攻击的：时长time 伤害倍率mag 晕值
    //伤害box又是另一个麻烦的故事了 比如说 形状是一个枚举 以玩家为中心还是以玩家面前1/2box为中心又是一个枚举？ 半径？长？宽？
    [SerializeField] public int fullComboAttackTimes;//一次combo的总攻击次数
    [SerializeField] public List<MeleeAttackStruct> meleeAttacks;

    [SerializeField] public float comboBreakTimer;
    [SerializeField] public int currentCombo;


    [Header("Attack Effect Settings")]
    [SerializeField] protected GameObject rectangleEffectPrefab; // 蓝色闪烁特效预制件
    [SerializeField] protected GameObject circleEffectPrefab; // 蓝色闪烁特效预制件

    protected override void Start()
    {
        base.Start();
        // 使用 as 关键字进行类型转换
        MeleeEquipmentSO meleeEquipmentSO = equipmentSO as MeleeEquipmentSO;
        if (meleeEquipmentSO != null)
        {
            fullComboAttackTimes = meleeEquipmentSO.fullComboAttackTimes;
            meleeAttacks = meleeEquipmentSO.meleeAttacks;
        }
        else
        {
            Debug.LogWarning("The equipmentSO is not a MeleeEquipmentSO");
        }

    }

    protected override void Update()
    {
        base.Update();
        if(comboBreakTimer > 0)
        {
            comboBreakTimer -= Time.deltaTime;
        }
        else
        {
            SetCombo(0);
        }

    }

    public Collider2D[] GetHitEnemy()
    {
        if (currentCombo >= meleeAttacks.Count) return null;
        Collider2D[] hits;
        MeleeAttackStruct currentAttack = meleeAttacks[currentCombo];

        bool isFacingRight = GetFacingDirection();
        Vector2 faceDirection = isFacingRight ? Vector2.right : Vector2.left;

        Vector2 attackCenter = currentAttack.attackCenter switch
        {
            MeleeAttackCenterEnum.player => transform.position,
            MeleeAttackCenterEnum.front => currentAttack.attackShape == MeleeAttackShapeEnum.circle ?
                (Vector2)transform.position + faceDirection * currentAttack.attackRadius : (Vector2)transform.position + faceDirection * currentAttack.attackLength / 2,
            _ => transform.position
        };

        
        if (currentAttack.attackShape == MeleeAttackShapeEnum.circle)
        {
            hits = Physics2D.OverlapCircleAll(attackCenter, currentAttack.attackRadius, PlayerStats.Instance.whatIsEnemy);
        }
        else
        {
            Vector2 size = new Vector2(currentAttack.attackLength, currentAttack.attackWidth);
            hits = Physics2D.OverlapBoxAll(
                attackCenter,
                size,
                isFacingRight ? 0 : 180,
                PlayerStats.Instance.whatIsEnemy
            );
        }
        return hits;
    }

    protected virtual void TriggerAttackEffect()
    {
        if (meleeAttacks == null || currentCombo >= meleeAttacks.Count) return;

        MeleeAttackStruct currentAttack = meleeAttacks[currentCombo];

        // 获取玩家朝向（假设通过transform.localScale判断）
        bool isFacingRight = GetFacingDirection();
        Vector2 faceDirection = isFacingRight ? Vector2.right : Vector2.left;

        // 计算攻击中心位置
        Vector2 attackCenter = currentAttack.attackCenter switch
        {
            MeleeAttackCenterEnum.player => transform.position,
            MeleeAttackCenterEnum.front => currentAttack.attackShape==MeleeAttackShapeEnum.circle? 
                (Vector2)transform.position + faceDirection * currentAttack.attackRadius: (Vector2)transform.position + faceDirection * currentAttack.attackLength/2,
            _ => transform.position
        };

        // 选择预制体并实例化
        GameObject effectPrefab = currentAttack.attackShape switch
        {
            MeleeAttackShapeEnum.circle => circleEffectPrefab,
            MeleeAttackShapeEnum.rectangle => rectangleEffectPrefab,
            _ => null
        };

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, attackCenter, Quaternion.identity, transform);

            // 设置特效尺寸
            if (currentAttack.attackShape == MeleeAttackShapeEnum.circle)
            {
                effect.transform.localScale = Vector3.one * (currentAttack.attackRadius * 2);
            }
            else
            {
                Vector3 effectScale = new Vector3(
                    currentAttack.attackLength,
                    currentAttack.attackWidth,
                    1
                );

                // 根据朝向调整方向
                if (!isFacingRight)
                {
                    effectScale.x *= -1;
                }

                effect.transform.localScale = effectScale;
            }

            Destroy(effect, 0.2f);
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
        SetAttackCooldown(meleeAttacks[currentCombo].attackTime);

        // 只有一段连击的情况
        if (meleeAttacks.Count == 1)
        {
            Debug.Log("只有一段连击，重置combo");
            SetCombo(0);
            SetComboBreakTime(0);
            return;
        }
        //重置combo
        if (comboBreakTimer > 0)
        {
            
            Debug.Log(meleeAttacks.Count);
            if(currentCombo + 1 == meleeAttacks.Count)
            {
                Debug.Log("连击结束");
                SetCombo(0);
                SetComboBreakTime(0);
            }
            else
            {
                Debug.Log("进入下一段连击");
                SetCombo(currentCombo + 1);
                SetComboBreakTime(meleeAttacks[currentCombo - 1].comboBreakTime);
            }
        }
        else
        {
            Debug.Log("开始连击");
            SetCombo(currentCombo + 1);
            SetComboBreakTime(meleeAttacks[currentCombo - 1].comboBreakTime);
        }
        
    }
    #endregion
}
