using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class 怪物编辑器 : EditorWindow
{
    private string csvFilePath;

    [MenuItem("Tools/怪物配置工具")]
    public static void ShowWindow()
    {
        GetWindow<怪物编辑器>("怪物配置工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("怪物配置工具", EditorStyles.boldLabel);

        if (GUILayout.Button("选择 CSV 文件"))
        {
            csvFilePath = EditorUtility.OpenFilePanel("选择 CSV 文件", "", "csv");
        }

        GUILayout.Label("当前选择的文件: " + csvFilePath);

        if (GUILayout.Button("更新怪物 SO"))
        {
            if (!string.IsNullOrEmpty(csvFilePath))
            {
                UpdateEnemyData();
            }
            else
            {
                Debug.LogWarning("请先选择一个 CSV 文件。");
            }
        }
    }

    private void UpdateEnemyData()
    {
        // List<string[]> 是一个字符串数组的列表
        List<string[]> csvData = ReadCSV(csvFilePath);

        // row 的类型是 string[]
        foreach (string[] row in csvData.Skip(1)) // 跳过表头
        {
            UpdateEnemySO(row);
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

    private void UpdateEnemySO(string[] row)
    {
        // 通过装备名称找到对应的 SO
        string assetPath = $"Assets/Scripts/Enemy/EnemyTypes/{row[1]}/{row[1]}属性{row[2]}.asset";
        EnemyProfileSO enemyProfile = AssetDatabase.LoadAssetAtPath<EnemyProfileSO>(assetPath);

        if (enemyProfile != null)
        {
            // 更新装备 SO 的属性
            enemyProfile.maxHealth = int.Parse(row[3]);
            enemyProfile.baseDamage = int.Parse(row[4]);
            enemyProfile.stunResistance = int.Parse(row[8]);
            enemyProfile.freezeResistance = int.Parse(row[6]);
            enemyProfile.stunDuration = float.Parse(row[9]);
            enemyProfile.freezeDuration = float.Parse(row[7]);
            //equipment.equipmentDesc = row[6];

            /*// 处理动画和图片路径
            equipment.attackAnimator = Resources.Load<AnimatorOverrideController>(row[7]);
            equipment.comboAnimations = LoadComboAnimations(row[8]);

            // 如果是近战武器，填充近战武器的额外属性
            }*/



            EditorUtility.SetDirty(enemyProfile); // 标记为脏数据以便保存
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

   
}
