using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField] private GameObject rightClickMenu;
    [SerializeField] private InventoryRightClickMenuUI rightClickMenuUI;


    private void Update()
    {
        // 如果右键菜单没激活，就不用管
        if (!rightClickMenu.activeSelf) return;

        // 检测左键或右键点击
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // 判断点击的是否是 UI（特别是右键菜单）
            if (!IsPointerOverUI(rightClickMenu))
            {
                HideRightClickMenu();
            }
        }
    }

    // 判断是否点击在某个特定UI对象及其子对象上
    private bool IsPointerOverUI(GameObject target)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == target || result.gameObject.transform.IsChildOf(target.transform))
            {
                return true;
            }
        }

        return false;
    }

    public void ShowRightClickMenu(IEquipableItem item, Vector2 position,bool isEquipmentSlot)
    {
        rightClickMenu.SetActive(true);
        rightClickMenu.transform.position = position;

        rightClickMenuUI.Setup(item, isEquipmentSlot);
    }

    public void HideRightClickMenu()
    {
        rightClickMenu.SetActive(false);
    }


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

    public void RefreshPanel()
    {
        SetUpEquipment();
        SetUpInventory();
    }

    public void RefreshPanel(string item1)
    {
        List<IEquipableItem> items = InventoryManager.Instance.allItems;

        int index1 = items.FindIndex(i => i.GetEquipableItemName() == item1);

        if (index1 >= 0)
        {
            IEquipableItem target = items[index1];
            items.RemoveAt(index1);           // 出队
            items.Add(target);                // 入队到最后
        }

        SetUpEquipment();
        SetUpInventory();
    }

    public void RefreshPanel(string item1,string item2)
    {
        List<IEquipableItem> items = InventoryManager.Instance.allItems;

        int index1 = items.FindIndex(i => i.GetEquipableItemName() == item1);
        int index2 = items.FindIndex(i => i.GetEquipableItemName() == item2);

        if (index1 >= 0 && index2 >= 0)
        {
            // 交换两个物品的位置
            IEquipableItem temp = items[index1];
            items[index1] = items[index2];
            items[index2] = temp;
        }
        SetUpEquipment();
        SetUpInventory();
    }

    /// <summary>
    /// 在打开背包的时候显示装备栏
    /// </summary>
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

    /// <summary>
    /// 在打开背包的时候显示背包物品
    /// </summary>
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



    /// <summary>
    /// 显示右侧面板信息（被 InventorySlot 调用）
    /// </summary>
    /// <param name="item"></param>
    /// <param name="inventoryMessage"></param>
    public void ShowItemDetail(IEquipableItem item,InventoryMessage inventoryMessage)
    {
        slotNameText.text = inventoryMessage.itemName;
        slotDescText.text = inventoryMessage.itemDesc;
        slotAffixText.text = inventoryMessage.itemAffix;
    }

}
