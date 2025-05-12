using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRightClickMenuUI : MonoBehaviour
{
    private IEquipableItem currentItem;

    [Header("右键背包里的东西/右键装备栏里的东西")]
    [SerializeField] private GameObject inventoryRClick;
    [SerializeField] private GameObject equipmentRClick;

    public void Setup(IEquipableItem item,bool isEquipmentRClick)
    {
        currentItem = item;
        if(isEquipmentRClick)
        {
            equipmentRClick.SetActive(true);
            inventoryRClick.SetActive(false);
        }
        else
        {
            equipmentRClick.SetActive(false);
            inventoryRClick.SetActive(true);
        }
    }

    #region 右键背包

    public void OnClickEquipMain()
    {
        Debug.Log($"装备{currentItem}到主手");
        if(PlayerStats.Instance.baseEquipment1 == null )
        {
            foreach (Transform obj in PlayerStats.Instance.inventoryEquipmentParent)
            {
                BaseEquipment item = obj.GetComponent<BaseEquipment>();

                if (item.GetEquipableItemName() == currentItem.GetEquipableItemName())
                {
                    Debug.Log("交换这个");
                    item.transform.SetParent(PlayerStats.Instance.mainWeaponParent);
                    PlayerStats.Instance.baseEquipment1 = item;
                }
            }
            UIManager.Instance.RefreshInventoryPanel();
            gameObject.SetActive(false);
        }
        else
        {
            string mainWeaponName = PlayerStats.Instance.baseEquipment1.GetEquipableItemName();
            //怎么装备？我们把装备那个一下 先把主手的挪下来 然后把这个挪上去
            foreach (Transform obj in PlayerStats.Instance.inventoryEquipmentParent)
            {
                BaseEquipment item = obj.GetComponent<BaseEquipment>();

                if (item.GetEquipableItemName() == currentItem.GetEquipableItemName())
                {
                    Debug.Log("交换这个");
                    PlayerStats.Instance.baseEquipment1.transform.SetParent(PlayerStats.Instance.inventoryEquipmentParent);
                    item.transform.SetParent(PlayerStats.Instance.mainWeaponParent);
                    PlayerStats.Instance.baseEquipment1 = item;
                }
            }
            UIManager.Instance.RefreshInventoryPanel(currentItem.GetEquipableItemName(), mainWeaponName);

            gameObject.SetActive(false);
        }
        
    }

    public void OnClickEquipSub()
    {
        Debug.Log($"装备{currentItem}到副手");
        if (PlayerStats.Instance.baseEquipment2 == null)
        {
            foreach (Transform obj in PlayerStats.Instance.inventoryEquipmentParent)
            {
                BaseEquipment item = obj.GetComponent<BaseEquipment>();

                if (item.GetEquipableItemName() == currentItem.GetEquipableItemName())
                {
                    Debug.Log("交换这个");
                    item.transform.SetParent(PlayerStats.Instance.secondaryWeaponParent);
                    PlayerStats.Instance.baseEquipment2 = item;
                }
            }
            UIManager.Instance.RefreshInventoryPanel();
            gameObject.SetActive(false);
        }
        else
        {
            string mainWeaponName = PlayerStats.Instance.baseEquipment2.GetEquipableItemName();
            //怎么装备？我们把装备那个一下 先把主手的挪下来 然后把这个挪上去
            foreach (Transform obj in PlayerStats.Instance.inventoryEquipmentParent)
            {
                BaseEquipment item = obj.GetComponent<BaseEquipment>();

                if (item.GetEquipableItemName() == currentItem.GetEquipableItemName())
                {
                    Debug.Log("交换这个");
                    PlayerStats.Instance.baseEquipment2.transform.SetParent(PlayerStats.Instance.inventoryEquipmentParent);
                    item.transform.SetParent(PlayerStats.Instance.secondaryWeaponParent);
                    PlayerStats.Instance.baseEquipment2 = item;
                }
            }
            UIManager.Instance.RefreshInventoryPanel(currentItem.GetEquipableItemName(), mainWeaponName);

            gameObject.SetActive(false);
        }
    }

    public void OnDrop()
    {
        Debug.Log($"丢掉{currentItem}");
        int index = BattleManagerTest.Instance.DropSameEquipment(currentItem);
        InventoryManager.Instance.RemoveFromInventory(currentItem);
        UIManager.Instance.RefreshInventoryPanel();
        gameObject.SetActive(false);
    }

    #endregion 


    #region 右键装备

    public void OnClickSwitch()
    {
        var playerStats = PlayerStats.Instance;
        if (playerStats.baseEquipment1 == null || playerStats.baseEquipment2 == null) {
            gameObject.SetActive(false);
            return;
        }

        // 交换主副手
        BaseEquipment temp = playerStats.baseEquipment1;
        playerStats.baseEquipment1 = playerStats.baseEquipment2;
        playerStats.baseEquipment2 = temp;
        UIManager.Instance.RefreshInventoryPanel();

        gameObject.SetActive(false);
    }

    public void OnClickUnmount()
    {
        int setUnmount = 0;
        if (PlayerStats.Instance.baseEquipment1 != null)
        {
            if (PlayerStats.Instance.baseEquipment1.GetEquipableItemName() == currentItem.GetEquipableItemName())
            {
                setUnmount = 1;
            }
        }
        if (PlayerStats.Instance.baseEquipment2 != null)
        {
            if (PlayerStats.Instance.baseEquipment2.GetEquipableItemName() == currentItem.GetEquipableItemName())
            {
                setUnmount = 2;
            }
        }

        if (setUnmount == 1)
        {
            PlayerStats.Instance.baseEquipment1.transform.SetParent(PlayerStats.Instance.inventoryEquipmentParent);
            PlayerStats.Instance.baseEquipment1 = null;
        }
        else if (setUnmount == 2)
        {
            PlayerStats.Instance.baseEquipment2.transform.SetParent(PlayerStats.Instance.inventoryEquipmentParent);
            PlayerStats.Instance.baseEquipment2 = null;
        }

        UIManager.Instance.RefreshInventoryPanel(currentItem.GetEquipableItemName());
        gameObject.SetActive(false);
    }

    public void OnClickDropEquipment()
    {
        Debug.Log($"丢掉{currentItem}");
        int setNull = 0;
        if(PlayerStats.Instance.baseEquipment1 != null)
        {
            if(PlayerStats.Instance.baseEquipment1.GetEquipableItemName() == currentItem.GetEquipableItemName())
            {
                setNull = 1;
            }
        }
        else if(PlayerStats.Instance.baseEquipment2 != null)
        {
            if(PlayerStats.Instance.baseEquipment2.GetEquipableItemName() == currentItem.GetEquipableItemName())
            {
                setNull= 2;
            }
        }
        int index = BattleManagerTest.Instance.DropSameEquipment(currentItem);
        InventoryManager.Instance.RemoveFromInventory(currentItem);

        if(setNull == 1)
        {
            PlayerStats.Instance.baseEquipment1 = null;
        }
        else if(setNull == 2)
        {
            PlayerStats.Instance.baseEquipment2 = null;
        }

        UIManager.Instance.RefreshInventoryPanel();
        gameObject.SetActive(false);
    }

    #endregion 右键装备
}
