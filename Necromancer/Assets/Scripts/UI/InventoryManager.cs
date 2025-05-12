using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager Instance;

    public List<IEquipableItem> allItems = new();

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
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
