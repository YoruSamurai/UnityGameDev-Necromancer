using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class 装备编辑器 : EditorWindow
{
    private string csvFilePath;

    [MenuItem("Tools/装备配置工具")]
    public static void ShowWindow()
    {
        GetWindow<装备编辑器>("装备配置工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("装备配置工具", EditorStyles.boldLabel);

        if (GUILayout.Button("选择 CSV 文件"))
        {
            csvFilePath = EditorUtility.OpenFilePanel("选择 CSV 文件", "", "csv");
        }

        GUILayout.Label("当前选择的文件: " + csvFilePath);

        if (GUILayout.Button("更新装备 SO"))
        {
            if (!string.IsNullOrEmpty(csvFilePath))
            {
                UpdateEquipmentData();
            }
            else
            {
                Debug.LogWarning("请先选择一个 CSV 文件。");
            }
        }
    }

    private void UpdateEquipmentData()
    {
        // List<string[]> 是一个字符串数组的列表
        List<string[]> csvData = ReadCSV(csvFilePath);

        // row 的类型是 string[]
        foreach (string[] row in csvData.Skip(1)) // 跳过表头
        {
            UpdateMeleeEquipmentSO(row);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("装备 SO 更新完成。");
    }

    private List<string[]> ReadCSV(string filePath)
    {
        // List<string[]> 是一个字符串数组的列表
        List<string[]> data = new List<string[]>();

        using (var reader = new StreamReader(filePath)) // reader 的类型是 StreamReader
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine(); // line 的类型是 string
                var values = line.Split(','); // values 的类型是 string[]
                data.Add(values);
            }
        }

        return data; // 返回类型是 List<string[]>
    }

    private void UpdateMeleeEquipmentSO(string[] row)
    {
        // 通过装备名称找到对应的 SO
        string assetPath = $"Assets/Scripts/Battle/BaseEquipment/Equipment/{row[1]}/{row[1]}.asset";
        EquipmentSO equipment = AssetDatabase.LoadAssetAtPath<EquipmentSO>(assetPath);

        if (equipment != null)
        {
            // 更新装备 SO 的属性
            equipment.baseDmg = int.Parse(row[2]);
            equipment.baseCritChance = int.Parse(row[5]);
            equipment.critMag = float.Parse(row[6]);
            //equipment.equipmentDesc = row[6];

            /*// 处理动画和图片路径
            equipment.attackAnimator = Resources.Load<AnimatorOverrideController>(row[7]);
            equipment.comboAnimations = LoadComboAnimations(row[8]);

            // 如果是近战武器，填充近战武器的额外属性
            }*/
            MeleeEquipmentSO meleeEquipment = equipment as MeleeEquipmentSO;
            if (meleeEquipment != null)
            {
                meleeEquipment.fullComboAttackTimes = int.Parse(row[10]);
                meleeEquipment.meleeAttacks = LoadMeleeAttacks(row);
            }


            EditorUtility.SetDirty(equipment); // 标记为脏数据以便保存
        }
        else
        {
            Debug.LogWarning($"找不到装备 SO: {row[0]}");
        }
    }

    private AnimationClip[] LoadComboAnimations(string path)
    {
        string[] animationPaths = path.Split(';');
        AnimationClip[] animations = new AnimationClip[animationPaths.Length];

        for (int i = 0; i < animationPaths.Length; i++)
        {
            animations[i] = Resources.Load<AnimationClip>(animationPaths[i]);
        }

        return animations;
    }

    private List<MeleeAttackStruct> LoadMeleeAttacks(string[] row)
    {
        List<MeleeAttackStruct> meleeAttacks = new List<MeleeAttackStruct>();

        // 1. 先解析“连击次数”
        int comboCount = int.Parse(row[10]);

        // 2. 将 CSV 中以 '\' 分隔的多段数据解析成数组
        string[] timeArr = row[11].Split('\\');  // 攻击时间
        string[] magArr = row[12].Split('\\');  // 攻击倍率
        string[] stunArr = row[13].Split('\\');  // 攻击晕眩值
        string shapeStr = row[14];              // 攻击形状（单一字符串）
        string centerStr = row[15];              // 攻击中心（单一字符串）
        string[] radiusArr = row[16].Split('\\');  // 攻击半径
        string[] lengthArr = row[17].Split('\\');  // 攻击长度
        string[] widthArr = row[18].Split('\\');  // 攻击宽度
        string[] breakArr = row[19].Split('\\');  // 连击中断时间
        string[] knockArr = row[20].Split('\\');  // 击退力度
        string[] chargeArr = row[21].Split('\\');  // 重武器蓄力时间

        // 3. 解析攻击形状与攻击中心
        MeleeAttackShapeEnum attackShape = (MeleeAttackShapeEnum)Enum.Parse(typeof(MeleeAttackShapeEnum), shapeStr, true);
        MeleeAttackCenterEnum attackCenter = (MeleeAttackCenterEnum)Enum.Parse(typeof(MeleeAttackCenterEnum), centerStr, true);

        // 4. 根据 comboCount 生成若干段攻击数据
        for (int i = 0; i < comboCount; i++)
        {
            MeleeAttackStruct attack = new MeleeAttackStruct();

            // 攻击时间
            attack.attackTime = float.Parse(timeArr[i]);
            // 攻击倍率
            attack.attackMag = float.Parse(magArr[i]);
            // 攻击晕眩值
            attack.attackStun = float.Parse(stunArr[i]);

            // 攻击形状和中心（全段共用）
            attack.attackShape = attackShape;
            attack.attackCenter = attackCenter;

            // 攻击半径、长度、宽度
            attack.attackRadius = float.Parse(radiusArr[i]);
            attack.attackLength = float.Parse(lengthArr[i]);
            attack.attackWidth = float.Parse(widthArr[i]);

            // 连击中断时间
            attack.comboBreakTime = float.Parse(breakArr[i]);


            attack.knockbackForce = float.Parse(knockArr[i]);
            attack.chargeThreshold = float.Parse(chargeArr[i]);

            // 其他字段如果有的话继续赋值
            // attack.xxx = ...

            meleeAttacks.Add(attack);
        }

        return meleeAttacks;
    }
}
