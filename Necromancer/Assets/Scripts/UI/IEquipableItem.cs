
using System;
using UnityEngine;

/// <summary>
/// 可以装备的物品如武器 技能 他们会有共同的被查询方法
/// </summary>
public interface IEquipableItem
{
    public string GetEquipableItemName();
    public InventoryMessage GetEquipableItemMessage();

    public int GetEquipableItemLevel();

    public void OnEquip();

    public void OnUnequip();

    public void AddToInventory();
    /// <summary>
    /// 从背包里丢出物品
    /// </summary>
    public void DropFromInventory();

    /// <summary>
    /// 升级装备
    /// </summary>
    public void EquipableItemLevelUp();

    /// <summary>
    /// 重铸装备
    /// </summary>
    public void EquipableItemRecast();

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