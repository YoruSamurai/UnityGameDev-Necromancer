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

    public void AddToInventory(IEquipableItem item)
    {
        // 如果已有同名，替换并移除旧的
        IEquipableItem existing = allItems.Find(x => x.GetEquipableItemName() == item.GetEquipableItemName());
        if (existing != null) allItems.Remove(existing);
        allItems.Add(item);
        Debug.Log(allItems.Count);
    }

    public void RemoveFromInventory(IEquipableItem item)
    {

    }

    public void EquipItem(IEquipableItem item)
    {

    }


}
