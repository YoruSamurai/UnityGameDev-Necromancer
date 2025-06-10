using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LdtkTest;

public class InventoryManager : SingletonManagerBase<InventoryManager>
{


    public List<IEquipableItem> allItems = new();

    public List<string> itemId = new();

    #region save and load
    private List<SerializableEquipableItemData> serializableEquipmentData;

    public void SetSerializableEquipableItemData(List<SerializableEquipableItemData> data)
    {
        serializableEquipmentData = data;
    }

    #endregion

    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
    }

    private void Start()
    {
        serializableEquipmentData = new List<SerializableEquipableItemData>();
    }

    private void Update()
    {
        itemId.Clear();
        foreach (var item in allItems)
        {
            itemId.Add(item.GetEquipableItemName());
        }
    }


    public void InitInventory(int sceneIndex)
    {
        if(sceneIndex == 0)
        {
            Debug.LogWarning("是主菜单哦 我先走了 走之前清空一下");
            allItems.Clear();
            return;
        }
        InitEquipment(sceneIndex);
    }


    /// <summary>
    /// initial equipment via gamedata
    /// </summary>
    /// <param name="sceneIndex"></param>
    /// <returns></returns>
    public void InitEquipment(int sceneIndex)
    {
        if(serializableEquipmentData != null)
        {
            allItems.Clear();
            foreach (var data in serializableEquipmentData)
            {
                BattleManagerTest.Instance.LoadBaseEquipment(data);
            }

        }

    }

    public List<SerializableEquipableItemData> GetSerializableEquipmentData()
    {
        List<SerializableEquipableItemData> datas = new List<SerializableEquipableItemData>();

        foreach (var item in allItems)
        {
            SerializableEquipableItemData data = item.GetSerializableEquipableItemData();
            datas.Add(data);
        }
        return datas;
    }


    /// <summary>
    /// 返回背包里是否有同样的装备 bool
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IsInventoryExistItem(IEquipableItem item)
    {
        for(int i = 0; i < allItems.Count;i++)
        {
            if(allItems[i].GetEquipableItemName() == item.GetEquipableItemName())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取背包中同名的Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IEquipableItem GetInventoryExistItem(IEquipableItem item)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            Debug.Log($"{allItems[i].GetEquipableItemName()} + {item.GetEquipableItemName()}");
            if (allItems[i].GetEquipableItemName() == item.GetEquipableItemName())
            {
                Debug.Log("返回！");
                return allItems[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 把Item加入到背包 如果背包里有同名物品则把它替换掉
    /// </summary>
    /// <param name="item"></param>
    public void AddToInventory(IEquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试添加一个 null 的物品到背包");
            return;
        }
        // 如果已有同名，替换并移除旧的
        IEquipableItem existing = allItems.Find(x => x.GetEquipableItemName() == item.GetEquipableItemName());
        if (existing != null) allItems.Remove(existing);
        allItems.Add(item);
        Debug.Log(allItems.Count);
    }

    public void RemoveFromInventory(IEquipableItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("尝试丢弃一个 null 的物品");
            return;
        }
        // 如果已有同名，替换并移除旧的
        IEquipableItem existing = allItems.Find(x => x.GetEquipableItemName() == item.GetEquipableItemName());
        if (existing != null) allItems.Remove(existing);
        Debug.Log(allItems.Count);
    }



    public void EquipItem(IEquipableItem item)
    {

    }


}
