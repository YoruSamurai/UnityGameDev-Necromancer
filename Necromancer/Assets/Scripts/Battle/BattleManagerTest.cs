using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerTest : MonoBehaviour
{

    public static BattleManagerTest Instance { get; private set; }

    //在这里我们存储所有装备和词条
    [SerializeField] public List<BaseEquipment> equipmentList;
    [SerializeField] public List<BaseAffix> affixList;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }


    private void Start()
    {
        // 获取所有子对象中的 BaseEquipment 组件
        equipmentList = new List<BaseEquipment>(GetComponentsInChildren<BaseEquipment>(true));
        affixList = new List<BaseAffix>(GetComponentsInChildren<BaseAffix>(true));

    }

    //生成装备给玩家的这些部分 以后都需要进行重写 现在追求实用性
    private void ClearEquipmentInTransform(Transform targetTransform)
    {
        foreach (Transform child in targetTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public BaseEquipment GetRandomWeapon(Transform parentTransform)
    {
        if (equipmentList.Count == 0) return null;
        ClearEquipmentInTransform(parentTransform);

        //生成一个随机武器
        BaseEquipment originalEquipment = equipmentList[Random.Range(0, equipmentList.Count)];
        BaseEquipment newEquipment = Instantiate(originalEquipment, parentTransform);

        //可能这里需要初始化等级？什么的

        //根据情况添加词条
        BaseAffix affix = GetEquipmentAffix(newEquipment);
        newEquipment.equipmentAffixList.Add(affix);

        return newEquipment;
    }

    public BaseAffix GetEquipmentAffix(BaseEquipment _equipment)
    {
        List<BaseAffix> possibleAffixList = new List<BaseAffix>();
        foreach(BaseAffix affix in affixList)
        {
            bool canAddAffix = true;
            if (affix.needEquipmentTags.Count > 0 && canAddAffix)
            {
                //装备标签必须满足词缀的全部需求标签
                foreach(EquipmentTag tag in affix.needEquipmentTags)
                {
                    if (!_equipment.equipmentTags.Contains(tag))
                    {
                        canAddAffix = false;
                        break;
                    }
                }
            }
            if (affix.contrastEquipmentTags.Count > 0 && canAddAffix)
            {
                //装备标签必须满足词缀的全部需求标签
                foreach (EquipmentTag tag in affix.contrastEquipmentTags)
                {
                    if (_equipment.equipmentTags.Contains(tag))
                    {
                        canAddAffix = false;
                        break;
                    }
                }
            }
            if(_equipment.equipmentAffixList.Count > 0 && canAddAffix)
            {
                foreach(BaseAffix equipmentAffix in _equipment.equipmentAffixList)
                {
                    if(equipmentAffix.contrastAffixTags.Count > 0 && equipmentAffix.contrastAffixTags.Contains(affix.affixTag))
                    {
                        canAddAffix = false;
                        break;
                    }
                }
            }
            if (canAddAffix)
            {
                possibleAffixList.Add(affix);
            }
        }
        if(possibleAffixList.Count > 0)
        {
            return possibleAffixList[Random.Range(0, possibleAffixList.Count)];
        }
        return null;
    }

}
