using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTriggerClass : MonoBehaviour
{
    public static BattleTriggerClass Instance { get; private set; }


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        
    }


    public void TriggerOnAttack()
    {
        /*if(playerAttackAffixes.Count > 0)
        {
            Debug.Log("有攻击词条");
        }
        else
        {
            Debug.Log("没有攻击词条");
            return;
        }*/
    }

    public float TriggerOnHit(BaseEquipment _baseEquipment,MonsterStats _monsterStats)
    {

        float cMag = 1;//伤害倍率c
        if(_baseEquipment.equipmentAffixList.Count > 0)
        {
            foreach(BaseAffix affix in _baseEquipment.equipmentAffixList)
            {
                if(affix.triggerCondition == BattleTriggerCondition.playerHit)
                {
                    // 临时存储当前词缀的伤害倍率修改事件
                    System.Action<float> modifyDamageEvent = (multiplier) => { cMag *= multiplier; };

                    // 绑定事件
                    affix.OnModifyDamageMultiplier += modifyDamageEvent;

                    // 触发词缀效果
                    affix.InvokeAffixPre(PlayerStats.Instance, _monsterStats);

                    // 立即解绑，防止影响下一次攻击
                    affix.OnModifyDamageMultiplier -= modifyDamageEvent;
                }
                    
            }
            foreach (BaseAffix affix in _baseEquipment.equipmentAffixList)
            {
                if (affix.triggerCondition == BattleTriggerCondition.playerHit)
                {
                    // 触发词缀效果
                    affix.InvokeAffixPost(PlayerStats.Instance, _monsterStats, cMag);
                }

            }
        }

        return cMag;

    }


}
