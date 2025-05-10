
/// <summary>
/// 可以装备的物品如武器 技能 他们会有共同的被查询方法
/// </summary>
public interface IEquipableItem
{
    public string GetEquipableItemName();
    public string GetEquipableItemMessage();

    public void AddToInventory();
    public void RemoveFromInventory();

    public void OnEquip();

    public void OnUnequip();

}
