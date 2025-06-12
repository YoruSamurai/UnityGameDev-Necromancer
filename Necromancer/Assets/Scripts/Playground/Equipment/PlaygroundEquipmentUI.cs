using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundEquipmentUI : PlaygroundBaseSetupUI
{

    [SerializeField] private Transform EquipmentPrefabParent;
    [SerializeField] private GameObject EquipmentPrefab;

    [SerializeField] private PlaygroundEquipmentDetail playgroundEquipmentDetail;

    protected override void Start()
    {
        base.Start();

        playgroundEquipmentDetail = GetComponent<PlaygroundEquipmentDetail>();
    }


    public override void OpenPanel()
    {
        base.OpenPanel();

        SetUpInventory();

    }

    /// <summary>
    /// 在打开装备操场的时候显示所有装备
    /// </summary>
    private void SetUpInventory()
    {


        //这里我们先清空 InventoryPrefabParent的子物体
        foreach (Transform child in EquipmentPrefabParent)
        {
            Destroy(child.gameObject);
        }
        //然后开始通过InventoryManager.Instance.allItems获取所有的装备List<IEquipableItem>
        // 2. 遍历所有物品并生成槽位
        List<IEquipableItem> allItems = new List<IEquipableItem>();
        foreach (var item in BattleManagerTest.Instance.equipmentPrefabList.equipmentList)
        {
            allItems.Add(item); // 合法：BaseEquipment 实现了 IEquipableItem
        }

        //然后遍历allItems 生成InventoryPrefab，这时候获取InventoryPrefab的脚本InventorySlot 这时候对它进行Initialize
        foreach (var item in allItems)
        {
            GameObject slotObj = Instantiate(EquipmentPrefab, EquipmentPrefabParent);
            PlaygroundEquipmentSlot slot = slotObj.GetComponent<PlaygroundEquipmentSlot>();
            slot.Initialize(item, this);
        }
    }

    public void ShowEquipmentDetail(EquipmentSO equipmentSO)
    {
        playgroundEquipmentDetail.Initialize(equipmentSO);
    }
}
