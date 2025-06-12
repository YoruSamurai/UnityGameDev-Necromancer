using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManagerTest : SingletonManagerBase<BattleManagerTest>
{


    //在这里我们存储所有装备和词条
    //[SerializeField] public List<BaseAffix> affixList;

    [SerializeField] public EquipmentListSO equipmentPrefabList;
    [SerializeField] public AffixListSO affixPrefabList;

    //新增技能列表
    [SerializeField] public List<SkillController> skillList;

    [SerializeField] private GameObject pickablePrefab;

    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
    }


    private void Start()
    {
        skillList = new List<SkillController>(GetComponentsInChildren<SkillController>(true));

    }

    /// <summary>
    /// 丢弃同名的装备，返回值为丢弃的装备的位置 1主武器 2副武器 3背包 4不在背包里
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int DropSameEquipment(IEquipableItem item)
    {
        IEquipableItem sameEquipment = InventoryManager.Instance.GetInventoryExistItem(item);
        if (sameEquipment == null)
        {
            Debug.Log("背包里没有捏");
            return 4;
        }
        Debug.Log("我丢");
        sameEquipment.DropFromInventory();


        if(PlayerStats.Instance.baseEquipment1 != null && PlayerStats.Instance.baseEquipment1.GetEquipableItemName() == sameEquipment.GetEquipableItemName())
        {
            ClearEquipmentInTransform(PlayerStats.Instance.mainWeaponParent);
            return 1;
        }
        if (PlayerStats.Instance.baseEquipment2 != null && PlayerStats.Instance.baseEquipment2.GetEquipableItemName() == sameEquipment.GetEquipableItemName())
        {
            ClearEquipmentInTransform(PlayerStats.Instance.subWeaponParent);
            return 2;
        }
        return 3;
    }


    public void TreasureDropPickableEquipment(Vector2 pos)
    {
        BaseEquipment baseEquipment1 = GetRandomSpriteWeapon();
        DropItem(baseEquipment1, pos);
    }

    /// <summary>
    /// 把物品从XX里面丢出来，
    /// 注意在GameObject itemGO = itemBehaviour.gameObject;中，我们会获取装备然后把它设置到新的obj也就是可拾取物体那里
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    public void DropItem(IPickableItem item, Vector2 position)
    {
        GameObject obj = Instantiate(pickablePrefab,position, Quaternion.identity,
            LevelManager.Instance.storage.scatterEquipmentTransform);
        Pickable pickable = obj.GetComponent<Pickable>();
        if (item is MonoBehaviour itemBehaviour)
        {
            GameObject itemGO = itemBehaviour.gameObject;

            // 设置为 obj 的子物体
            itemGO.transform.SetParent(obj.transform);
            itemGO.transform.localPosition = Vector3.zero; // 可选：让它居中
        }
        pickable.SetPickable(item);
    }

    /// <summary>
    /// 在把新的武器生成到Transform前，我们需要把之前在的gameObject移到背包的Transform。
    /// </summary>
    /// <param name="targetTransform"></param>
    public void ClearEquipmentInTransform(Transform targetTransform)
    {
        foreach (Transform child in targetTransform)
        {
            child.gameObject.transform.SetParent(PlayerStats.Instance.inventoryEquipmentParent);
            //Destroy(child.gameObject);
        }
    }

    public SkillController GetRandomSkill(Transform parentTransform)
    {
        if (skillList.Count == 0) return null;
        ClearEquipmentInTransform(parentTransform);

        //生成一个随机武器
        SkillController originalSkill = skillList[Random.Range(0, skillList.Count)];
        SkillController newSkill = Instantiate(originalSkill, parentTransform);

        //可能这里需要初始化等级？什么的
        return newSkill;
    }

    /// <summary>
    /// 生成那种丢出来的武器 给玩家去捡起来
    /// </summary>
    /// <returns></returns>
    public BaseEquipment GetRandomSpriteWeapon()
    {

        // 随机从预制体列表中取一个装备预制体
        BaseEquipment prefab = equipmentPrefabList.equipmentList[Random.Range(0, equipmentPrefabList.equipmentList.Count)];

        // 实例化它（临时用 null parent，因为之后会销毁）
        BaseEquipment instance = Instantiate(prefab);
        instance.Initialize();
        // 添加词缀
        BaseAffix affix = GetEquipmentAffix(instance);
        if (affix != null)
            instance.equipmentAffixList.Add(affix);

        return instance;
    }

    /// <summary>
    /// 生成随机武器给玩家
    /// </summary>
    /// <param name="parentTransform"></param>
    /// <returns></returns>
    public BaseEquipment GetRandomWeapon(Transform parentTransform)
    {
        
        BaseEquipment originalEquipment = equipmentPrefabList.equipmentList[Random.Range(0, equipmentPrefabList.equipmentList.Count)];
        BaseEquipment tempInstance = Instantiate(originalEquipment);
        tempInstance.Initialize();

        //这里判断随机武器是不是已经有了
        //怎么判断？我们去InventoryManager检查一下？
        bool haveSame = InventoryManager.Instance.IsInventoryExistItem(tempInstance);
        Destroy(tempInstance.gameObject); // 只用于比对，不是真正用的物体
        if (haveSame) return null;

        //这行好像不太需要鹅
        originalEquipment.Initialize();
        
        ClearEquipmentInTransform(parentTransform);

        BaseEquipment newEquipment = Instantiate(originalEquipment, parentTransform);
        newEquipment.Initialize();
        //可能这里需要初始化等级？什么的

        //根据情况添加词条
        BaseAffix affix = GetEquipmentAffix(newEquipment);
        newEquipment.equipmentAffixList.Add(affix);

        return newEquipment;
    }


    /// <summary>
    /// 通过武器ID 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BaseEquipment GetWeaponByID(int id)
    {
        BaseEquipment equipment = null;
        foreach(var item in equipmentPrefabList.equipmentList)
        {
            if(item.equipmentSO.equipmentID == id)
            {
                equipment = item; break;
            }
        }
        if(equipment == null)
        {
            Debug.LogWarning("没有对应的武器诶");
        }
        return equipment;
    }

    /// <summary>
    /// 通过装备数据去加载装备
    /// </summary>
    /// <param name="data"></param>
    public void LoadBaseEquipment(SerializableEquipableItemData data)
    {
        BaseEquipment equipment = GetBaseEquipmentByID(data.itemID);
        if (equipment == null) 
        {
            Debug.LogWarning("没有加载到对应的装备 你干了什么");
        }
        BaseEquipment newEquipment = Instantiate(equipment, PlayerStats.Instance.inventoryEquipmentParent);
        newEquipment.Initialize(data.itemLevel);
        foreach(int affixId in data.itemAffixs)
        {
            //根据情况添加词条
            BaseAffix affix = GetAffixById(affixId);
            if(affix != null)
                newEquipment.equipmentAffixList.Add(affix);
        }
        InventoryManager.Instance.AddToInventory(newEquipment);
        if(data.slotPositionIndex == SlotPositionIndex.mainSlot)
        {
            PlayerStats.Instance.baseEquipment1 = newEquipment;
            newEquipment.transform.SetParent(PlayerStats.Instance.mainWeaponParent);
        }
        else if(data.slotPositionIndex == SlotPositionIndex.subSlot)
        {
            PlayerStats.Instance.baseEquipment2 = newEquipment;
            newEquipment.transform.SetParent(PlayerStats.Instance.subWeaponParent);
        }
    }

    private BaseAffix GetAffixById(int affixID)
    {
        foreach (BaseAffix affix in affixPrefabList.affixList)
        {
            if(affix.affixSO.affixID == affixID)
            {
                return affix;
            }
        }
        Debug.LogWarning("没有加载到对应的词条 你干了什么");
        return null;
    }

    public BaseEquipment GetBaseEquipmentByID(int id)
    {
        foreach(var equipment in equipmentPrefabList.equipmentList)
        {
            if(equipment.equipmentID == id) return equipment;
        }
        return null;
    }

    public BaseAffix GetEquipmentAffix(BaseEquipment _equipment)
    {
        List<BaseAffix> possibleAffixList = new List<BaseAffix>();
        foreach(BaseAffix affix in affixPrefabList.affixList)
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
            // 从可选列表中随机一个 affixPrefab 并实例化它
            BaseAffix selectedPrefab = possibleAffixList[Random.Range(0, possibleAffixList.Count)];
            BaseAffix newAffix = Instantiate(selectedPrefab, _equipment.transform);
            newAffix.Initialize(_equipment);
            return newAffix;
        }
        return null;
    }

    /// <summary>
    /// 重铸装备的词条。后续进一步扩展。
    /// </summary>
    /// <param name="_equipment"></param>
    /// <returns></returns>
    public BaseAffix RecastEquipmentAffix(BaseEquipment _equipment)
    {
        List<BaseAffix> possibleAffixList = new List<BaseAffix>();
        foreach (BaseAffix affix in affixPrefabList.affixList)
        {
            bool canAddAffix = true;
            if (affix.needEquipmentTags.Count > 0 && canAddAffix)
            {
                //装备标签必须满足词缀的全部需求标签
                foreach (EquipmentTag tag in affix.needEquipmentTags)
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
            if (_equipment.equipmentAffixList.Count > 0 && canAddAffix)
            {
                foreach (BaseAffix equipmentAffix in _equipment.equipmentAffixList)
                {
                    if (equipmentAffix.contrastAffixTags.Count > 0 && equipmentAffix.contrastAffixTags.Contains(affix.affixTag))
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
        if (possibleAffixList.Count > 0)
        {
            // 从可选列表中随机一个 affixPrefab 并实例化它
            BaseAffix selectedPrefab = possibleAffixList[Random.Range(0, possibleAffixList.Count)];
            BaseAffix newAffix = Instantiate(selectedPrefab, _equipment.transform);
            newAffix.Initialize(_equipment);
            return newAffix;
        }
        return null;
    }


    
    #region Playground 专属于playground的方法

    public List<int> GetAffixsCanAddToEquipment(EquipmentSO equipmentSO)
    {
        List<int> result = new List<int>();

        foreach (BaseAffix affix in affixPrefabList.affixList)
        {
            bool canAddAffix = true;
            if (affix.needEquipmentTags.Count > 0 && canAddAffix)
            {
                //装备标签必须满足词缀的全部需求标签
                foreach (EquipmentTag tag in affix.needEquipmentTags)
                {
                    if (!equipmentSO.equipmentTags.Contains(tag))
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
                    if (equipmentSO.equipmentTags.Contains(tag))
                    {
                        canAddAffix = false;
                        break;
                    }
                }
            }
            if (canAddAffix)
            {
                result.Add(affix.affixSO.affixID);
            }
        }
        return result;
    }


    public void Playground_GiveWeapon(BaseEquipment prefab, Vector2 pos)
    {
        // 实例化它（临时用 null parent，因为之后会销毁）
        BaseEquipment instance = Instantiate(prefab);
        instance.Initialize();
        // 添加词缀
        BaseAffix affix = GetEquipmentAffix(instance);
        if (affix != null)
            instance.equipmentAffixList.Add(affix);
        DropItem(instance, pos);
    }

    #endregion


}
