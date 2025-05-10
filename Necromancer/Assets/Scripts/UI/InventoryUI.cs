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
            GameObject slotObj = Instantiate(InventoryPrefab, InventoryPrefabParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            slot.Initialize(item, this);
        }



        gameObject.SetActive(true);
        UIManager.Instance.PauseGame();
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
