using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MessageShowUtil : MonoBehaviour
{
    public static MessageShowUtil Instance { get; private set; }

    [SerializeField] private TextMeshPro leftWeapon;
    [SerializeField] private TextMeshPro rightWeapon;
    [SerializeField] private TextMeshPro soulText;
    [SerializeField] private TextMeshPro goldText;

    [SerializeField] private GameObject pickableItemMessagePrefab;
    [SerializeField] private Transform pickableMessageParent;
    [SerializeField] private List<PickableItemPrefab> messageList = new List<PickableItemPrefab>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Update()
    {
        if(PlayerStats.Instance.baseEquipment1 != null)
        {
            leftWeapon.text = $"{PlayerStats.Instance.baseEquipment1.equipmentName} + {PlayerStats.Instance.baseEquipment1.equipmentLevel}级";
        }
        if (PlayerStats.Instance.baseEquipment2 != null)
        {
            rightWeapon.text = $"{PlayerStats.Instance.baseEquipment2.equipmentName} + {PlayerStats.Instance.baseEquipment2.equipmentLevel}级";
        }
        soulText.text = $"灵魂数:{PlayerStats.Instance.soul}";
        goldText.text = $"金币数:{PlayerStats.Instance.gold}";

    }

    public PickableItemPrefab ShowPickableMessage(bool canShow, string name, string message,Vector2 pos,PickableItemPrefab itemMessage)
    {
        if (canShow)
        {
            foreach (var item in messageList)
            {
                item.DestroyPrefab();
            }
            messageList.Clear();
            GameObject obj = Instantiate(pickableItemMessagePrefab,pos + new Vector2(0,5f),Quaternion.identity, pickableMessageParent);
            PickableItemPrefab messageShow = obj.GetComponent<PickableItemPrefab>();
            messageShow.Initialize(name,message);
            messageList.Add(messageShow);
            return messageShow;
        }
        else
        {
            foreach(var item in messageList)
            {
                if (item.Equals(itemMessage))
                {
                    messageList.Remove(item);
                    item.DestroyPrefab();
                    return null;
                }
            }
        }
        return null;
    }

    




}
