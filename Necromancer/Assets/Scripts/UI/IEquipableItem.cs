
using System;
using UnityEngine;

/// <summary>
/// 可以装备的物品如武器 技能 他们会有共同的被查询方法
/// </summary>
public interface IEquipableItem
{
    public string GetEquipableItemName();
    public InventoryMessage GetEquipableItemMessage();

    public void AddToInventory();
    /// <summary>
    /// 从背包里丢出物品
    /// </summary>
    public void DropFromInventory();

    public void OnEquip();

    public void OnUnequip();

}


[Serializable]
public struct InventoryMessage
{
    public int itemLevel;
    public string itemName;
    public string itemDesc;
    public string itemAffix;
    public Sprite sprite;
}