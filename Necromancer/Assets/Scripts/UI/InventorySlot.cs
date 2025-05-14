using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private IEquipableItem item;
    private InventoryUI ui;
    private InventoryMessage inventoryMessage;

    [SerializeField] private bool isEquipSlot;

    [SerializeField] private Text slotLevelText;
    [SerializeField] private Image slotSprite;

    /// <summary>
    /// 初始化slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ui"></param>
    public void Initialize(IEquipableItem item, InventoryUI ui)
    {
        this.item = item;
        this.ui = ui;
        inventoryMessage = item.GetEquipableItemMessage();

        slotLevelText.text = $"Lv{inventoryMessage.itemLevel}";
        slotSprite.sprite = inventoryMessage.sprite;
    }

    /// <summary>
    /// 用于装备栏 在没有东西的时候清空。1
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


        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Vector2 slotScreenPos = transform.position; // 这是点击的 UI 元素的位置（屏幕坐标）

            if (isEquipSlot)
            {
                Debug.Log("哎哟我");
            }

            //Debug.Log(slotScreenPos.x + " " + slotScreenPos.y);
            Vector2 mouseViewportPos = Camera.main.ScreenToViewportPoint(slotScreenPos) ;
            mouseViewportPos += new Vector2(0.05f, 0f);
            if(mouseViewportPos.x > 1f)
            {
                mouseViewportPos -= new Vector2(0.1f, 0f);
            }
            //Debug.Log($"视口坐标: {mouseViewportPos.x}, {mouseViewportPos.y}");
            // 转回屏幕坐标
            Vector2 newScreenPos = Camera.main.ViewportToScreenPoint(mouseViewportPos);
            // 显示右键菜单，传递屏幕坐标
            ui.ShowRightClickMenu(item, newScreenPos,isEquipSlot);
            OnclickSlot();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnclickSlot();
        }
    }
}
