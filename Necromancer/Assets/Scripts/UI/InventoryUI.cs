using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{



    [SerializeField] private Transform InventoryPrefabParent;
    [SerializeField] private GameObject InventoryPrefab;

    [SerializeField] private Text slotNameText;
    [SerializeField] private Text slotDescText;
    [SerializeField] private Text slotAffixText;

    [SerializeField] private InventorySlot mainEquipmentSlot;
    [SerializeField] private InventorySlot subEquipmentSlot;


    public void TogglePanel()
    {
        if (gameObject.activeSelf)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        SetUpEquipment();
        SetUpInventory();



        gameObject.SetActive(true);
        UIManager.Instance.PauseGame();
    }

    private void SetUpEquipment()
    {
        if(PlayerStats.Instance.baseEquipment1 != null)
        {
            mainEquipmentSlot.Initialize(PlayerStats.Instance.baseEquipment1, this);
        }
        else
        {
            mainEquipmentSlot.SetDefault();
        }
        if (PlayerStats.Instance.baseEquipment2 != null)
        {
            subEquipmentSlot.Initialize(PlayerStats.Instance.baseEquipment2, this);
        }
        else
        {
            subEquipmentSlot.SetDefault();
        }
    }

    private void SetUpInventory()
    {

        //获取主武器和副武器的name
        string mainWeaponName = null;
        string subWeaponName = null;
        if(PlayerStats.Instance.baseEquipment1 != null)
        {
            mainWeaponName = PlayerStats.Instance.baseEquipment1.GetEquipableItemName();
        }
        if (PlayerStats.Instance.baseEquipment2 != null)
        {
            subWeaponName = PlayerStats.Instance.baseEquipment2.GetEquipableItemName();
        }

        //这里我们先清空 InventoryPrefabParent的子物体
        foreach (Transform child in InventoryPrefabParent)
        {
            Destroy(child.gameObject);
        }
        //然后开始通过InventoryManager.Instance.allItems获取所有的装备List<IEquipableItem>
        // 2. 遍历所有物品并生成槽位
        List<IEquipableItem> allItems = InventoryManager.Instance.allItems;

        //然后遍历allItems 生成InventoryPrefab，这时候获取InventoryPrefab的脚本InventorySlot 这时候对它进行Initialize
        foreach (var item in allItems)
        {
            if (item.GetEquipableItemName() == mainWeaponName || item.GetEquipableItemName() == subWeaponName)
                continue;
            GameObject slotObj = Instantiate(InventoryPrefab, InventoryPrefabParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slot.Initialize(item, this);
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UnpauseGame();
    }

    // 3. 显示右侧面板信息（被 InventorySlot 调用）
    public void ShowItemDetail(IEquipableItem item,InventoryMessage inventoryMessage)
    {
        slotNameText.text = inventoryMessage.itemName;
        slotDescText.text = inventoryMessage.itemDesc;
        slotAffixText.text = inventoryMessage.itemAffix;
    }

}
