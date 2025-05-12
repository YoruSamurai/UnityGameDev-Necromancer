using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManagerTest : MonoBehaviour
{

    public static BattleManagerTest Instance { get; private set; }

    //在这里我们存储所有装备和词条
    [SerializeField] public List<BaseEquipment> equipmentList;
    [SerializeField] public List<BaseAffix> affixList;

    [SerializeField] public EquipmentListSO equipmentPrefabList;

    //新增技能列表
    [SerializeField] public List<SkillController> skillList;

    [SerializeField] private GameObject pickablePrefab;
    [SerializeField] private Transform pickableParent;


    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }


    private void Start()
    {
        affixList = new List<BaseAffix>(GetComponentsInChildren<BaseAffix>(true));
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
            ClearEquipmentInTransform(PlayerStats.Instance.secondaryWeaponParent);
            return 2;
        }
        return 3;
    }


    public void DropPickableEquipment(Vector2 pos)
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
        GameObject obj = Instantiate(pickablePrefab,position, Quaternion.identity,pickableParent);
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
