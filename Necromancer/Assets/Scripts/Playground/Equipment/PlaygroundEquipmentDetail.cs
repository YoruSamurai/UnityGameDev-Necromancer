using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundEquipmentDetail : MonoBehaviour
{

    [SerializeField] private Image slotSprite;
    [SerializeField] private Text equipmentName;
    [SerializeField] private Text equipmentDescription;
    [SerializeField] private Text equipmentAffixList;
    [SerializeField] private EquipmentSO equipmentSO;

    public void Initialize(EquipmentSO _equipmentSO)
    {
        equipmentSO = _equipmentSO;

        if(equipmentSO != null)
        {
            equipmentName.text = equipmentSO.equipmentName;
            equipmentDescription.text = equipmentSO.equipmentDesc;
            List<int> affixCanEquipToEquipment = BattleManagerTest.Instance.GetAffixsCanAddToEquipment(equipmentSO);

            if (affixCanEquipToEquipment.Count > 0)
            {
                equipmentAffixList.text = "可用词缀：" + string.Join(" / ", affixCanEquipToEquipment);
            }
            else
            {
                equipmentAffixList.text = "无可用词缀";
            }

            slotSprite.sprite = equipmentSO.equipmentSprite;
        }
    }

    public void GenerateThisEquipment()
    {
        if(equipmentSO != null)
        {
            Debug.Log("给我一把吧 电棍");
            BaseEquipment baseEquipment = BattleManagerTest.Instance.GetWeaponByID(equipmentSO.equipmentID);
            BattleManagerTest.Instance.Playground_GiveWeapon(baseEquipment,PlayerStats.Instance.player.transform.position);
        }
        else
        {
            Debug.LogWarning("艾玛给不了你阿");
        }
    }


}
