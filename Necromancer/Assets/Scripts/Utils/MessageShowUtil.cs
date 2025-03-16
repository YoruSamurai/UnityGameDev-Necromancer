using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageShowUtil : MonoBehaviour
{
    public static MessageShowUtil Instance { get; private set; }

    [SerializeField] private TextMeshPro leftWeapon;
    [SerializeField] private TextMeshPro rightWeapon;
    [SerializeField] private TextMeshPro soulText;
    [SerializeField] private TextMeshPro goldText;

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




}
