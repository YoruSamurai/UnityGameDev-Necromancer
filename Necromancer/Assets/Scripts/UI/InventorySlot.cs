using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private IEquipableItem item;
    private InventoryUI ui;
    private InventoryMessage inventoryMessage;

    [SerializeField] private Text slotLevelText;
    [SerializeField] private Image slotSprite;


    public void Initialize(IEquipableItem item, InventoryUI ui)
    {
        this.item = item;
        this.ui = ui;
        inventoryMessage = item.GetEquipableItemMessage();

        slotLevelText.text = $"Lv{inventoryMessage.itemLevel}";
        slotSprite.sprite = inventoryMessage.sprite;
    }

    public void OnclickSlot()
    {
        ui.ShowItemDetail(item, inventoryMessage);
    }

    /*public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("fasf");
        ui.ShowItemDetail(item,inventoryMessage);
    }*/
}
