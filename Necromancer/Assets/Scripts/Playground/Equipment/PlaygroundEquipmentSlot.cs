using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundEquipmentSlot : MonoBehaviour
{
    private IEquipableItem item;

    private PlaygroundEquipmentUI ui;
    private EquipmentSO equipmentSO;

    [SerializeField] private Image slotSprite;


    /// <summary>
    /// 初始化slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ui"></param>
    public void Initialize(IEquipableItem item, PlaygroundEquipmentUI ui)
    {
        this.item = item;
        this.ui = ui;
        equipmentSO = item.GetEquipmentSO();
        slotSprite.sprite = equipmentSO.equipmentSprite;
    }


    /// <summary>
    /// 左键点击Slot UI显示信息
    /// </summary>
    public void OnclickSlot()
    {
        ui.ShowEquipmentDetail(equipmentSO);
    }

}
