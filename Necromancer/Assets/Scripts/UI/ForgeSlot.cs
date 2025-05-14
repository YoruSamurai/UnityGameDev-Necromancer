using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForgeSlot : MonoBehaviour, IPointerClickHandler
{
    private IEquipableItem item;
    private ForgeUI ui;
    private InventoryMessage inventoryMessage;

    [SerializeField] private bool isEquipSlot;

    [SerializeField] private Text slotLevelText;
    [SerializeField] private Image slotSprite;

    /// <summary>
    /// 初始化slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ui"></param>
    public void Initialize(IEquipableItem item, ForgeUI ui)
    {
        this.item = item;
        this.ui = ui;
        inventoryMessage = item.GetEquipableItemMessage();
        Debug.Log($"{item.GetEquipableItemName()} + LV{item.GetEquipableItemLevel()}");
        slotLevelText.text = $"Lv{inventoryMessage.itemLevel}";
        slotSprite.sprite = inventoryMessage.sprite;
    }

    /// <summary>
    /// 用于装备栏 在没有东西的时候清空。
    /// </summary>
    public void SetDefault()
    {
        slotSprite.sprite = null;
        slotLevelText.text = null;
    }

    /// <summary>
    /// 左键点击Slot UI显示信息
    /// </summary>
    public void OnclickSlot()
    {

        // 每次点击时强制刷新 inventoryMessage
        inventoryMessage = item.GetEquipableItemMessage();  // 确保重新获取最新的 item 信息

        ui.ShowRightClickMenu(item,this);
        ui.ShowItemDetail(item, inventoryMessage);
    }

    // ...原有变量
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null || ui == null)
        {
            Debug.LogWarning("InventorySlot 缺少必要引用：item 或 ui 为 null");
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("WODIAN;");
            OnclickSlot();
        }
    }
}
