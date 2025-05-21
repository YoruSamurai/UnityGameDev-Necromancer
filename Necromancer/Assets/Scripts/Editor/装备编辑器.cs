#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

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

        UpdateEquipmentPrefab(csvData);

        LoadEquipmentPrefab();

        Debug.Log("装备 SO 更新完成。");
    }

    /// <summary>
    /// 把装备prefab加载到装备List
    /// </summary>
    private void LoadEquipmentPrefab()
    {
        // 1. 加载 EquipmentList.asset
        EquipmentListSO equipmentList = AssetDatabase.LoadAssetAtPath<EquipmentListSO>("Assets/Prefab/Equipment/EquipmentList.asset");

        // 2. 清空原本列表
        equipmentList.equipmentList.Clear();

        // 3. 遍历文件夹
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefab/Equipment" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                // 判断是否挂着 BaseEquip 开头的组件
                var baseEquipComponent = prefab.GetComponent<BaseEquipment>();
                if (baseEquipComponent != null)
                {
                    equipmentList.equipmentList.Add(baseEquipComponent);
                }
            }
        }

        // 4. 保存
        EditorUtility.SetDirty(equipmentList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    

    private System.Type FindTypeByName(string className)
    {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(MonoBehaviour)) && type.Name == className)
                {
                    return type;
                }
            }
        }
        return null;
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
        if (row[1] == "")
        {
            Debug.Log("读完了 返回");
            return;
        }
        string assetPath = $"Assets/Scripts/Battle/BaseEquipment/Equipment/{row[1]}/{row[1]}.asset";
        EquipmentSO equipment = AssetDatabase.LoadAssetAtPath<EquipmentSO>(assetPath);
        if (equipment == null)
        {
            // 如果不存在，创建一个新的 MeleeEquipmentSO
            MeleeEquipmentSO newEquipment = ScriptableObject.CreateInstance<MeleeEquipmentSO>();

            // 确保目录存在
            string directoryPath = System.IO.Path.GetDirectoryName(assetPath);
            if (!AssetDatabase.IsValidFolder(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
                AssetDatabase.Refresh(); // 刷新，让Unity知道新建了文件夹
            }

            // 创建新的 Asset
            AssetDatabase.CreateAsset(newEquipment, assetPath);
            AssetDatabase.SaveAssets();

            equipment = newEquipment; // 把它赋值回来
        }
        if (equipment != null)
        {
            // 更新装备 SO 的属性
            equipment.equipmentName = row[1];
            equipment.baseDmg = int.Parse(row[2]);
            equipment.baseCritChance = int.Parse(row[5]);
            equipment.critMag = float.Parse(row[6]);
            equipment.equipmentDesc = row[7];

            if (Enum.TryParse(row[4].Trim(), true, out EquipmentType type))
            {
                equipment.equipmentType = type;
            }
            else
            {
                Debug.LogWarning($"未知的装备类型: {row[4]}");
            }

            // 处理动画和图片路径
            // 加载 AnimatorOverrideController
            string aocPath = $"Assets/Scripts/Battle/BaseEquipment/Equipment/D武器攻击覆盖Animator.overrideController";
            equipment.attackAnimator = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(aocPath);
            Debug.Log(row[1]  + row[10]);
            Debug.Log(row[1] + int.Parse(row[10]) + row[10]);
            equipment.comboAnimations = LoadComboAnimations(row[1] , int.Parse(row[10]));

            // 如果是近战武器，填充近战武器的额外属性
        
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

    private AnimationClip[] LoadComboAnimations(string weaponName , int clipNum)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(weaponName))
        {
            Debug.LogWarning("武器名称为空，无法加载动画");
            return null;
        }

        AnimationClip[] animations = new AnimationClip[clipNum];

        for (int i = 1; i <= clipNum; i++)
        {
            string folderPath = $"Assets/OtherAsset/Animation/WeaponClip/DefaultPlayer/{weaponName}";
            string clipName = $"{weaponName}{i}.anim";
            string fullPath = $"{folderPath}/{clipName}";

            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(fullPath);

            if (clip == null)
            {
                Debug.LogWarning($"找不到动画Clip: {fullPath}，正在自动创建一个空的动画");

                // 自动创建空动画
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                    AssetDatabase.Refresh();
                }

                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, fullPath);
                AssetDatabase.SaveAssets();
            }

            animations[i - 1] = clip;
        }

        AddClipsToAnimatorController(weaponName, animations);

        return animations;
#else
    Debug.LogError("该功能只能在编辑器中使用！");
    return null;
#endif
    }


#if UNITY_EDITOR
    private void AddClipsToAnimatorController(string weaponName, AnimationClip[] clips)
    {
        string controllerPath = "Assets/Scripts/Battle/BaseEquipment/Equipment/武器攻击动画.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

        if (controller == null)
        {
            Debug.LogError("无法加载 AnimatorController: " + controllerPath);
            return;
        }

        AnimatorControllerLayer layer = controller.layers[0]; // 只处理默认层

        foreach (AnimationClip clip in clips)
        {
            if (clip == null) continue;

            bool alreadyExists = layer.stateMachine.states.Any(state => state.state.motion == clip);
            if (!alreadyExists)
            {
                AnimatorState newState = layer.stateMachine.AddState(clip.name);
                newState.motion = clip;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"已将 {clips.Length} 个动画添加到 AnimatorController。");
    }
#endif

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

    /// <summary>
    /// 更新SO和Prefab 如果没有会自动创建
    /// </summary>
    /// <param name="csvData"></param>
    private void UpdateEquipmentPrefab(List<string[]> csvData)
    {
        // ----------------------------------
        // 遍历Prefab文件夹
        string prefabFolderPath = "Assets/Prefab/Equipment/";
        foreach (string[] row in csvData.Skip(1))
        {
            string weaponName = row[1];
            string prefabPath = prefabFolderPath + weaponName + ".prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                // 如果 prefab 不存在，创建新的
                GameObject newGO = new GameObject(weaponName);

                // 动态找到对应脚本并添加
                System.Type scriptType = FindTypeByName(weaponName);

                // 检查脚本类型是否找到
                if (scriptType != null)
                {
                    newGO.AddComponent(scriptType);
                    BaseEquipment equipment = newGO.GetComponent<BaseEquipment>();
                    if (equipment != null)
                    {
                        string assetPath = $"Assets/Scripts/Battle/BaseEquipment/Equipment/{row[1]}/{row[1]}.asset";
                        EquipmentSO equipmentSO = AssetDatabase.LoadAssetAtPath<EquipmentSO>(assetPath);
                        equipment.equipmentSO = equipmentSO;
                        equipment.enemyLayerMask = LayerMask.GetMask("Enemy"); 
                    }
                }
                else
                {
                    Debug.LogWarning($"找不到名为 {weaponName} 的组件脚本，尝试自动创建一个继承自 MeleeEquipment 的脚本。");

                    // 创建脚本
                    string scriptDir = $"Assets/Scripts/Battle/BaseEquipment/Equipment/{weaponName}";
                    string scriptPath = $"{scriptDir}/{weaponName}.cs";

                    // 确保目录存在
                    if (!Directory.Exists(scriptDir))
                    {
                        Directory.CreateDirectory(scriptDir);
                    }

                    // 脚本内容
                    string scriptContent = $@"using UnityEngine;

public class {weaponName} : MeleeEquipment
{{
    //
    protected override void Start()
    {{
        base.Start();
        Debug.Log(""{weaponName}，启动！"");
    }}

    protected override void Update()
    {{
        base.Update();
    }}

    public override void OnTriggerEnter2D(Collider2D collision)
    {{
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.layer == 6 && !isHitInAttack)
        {{
            Debug.Log(""是敌人！"");
            isHitInAttack = true;
        }}

        MonsterStats monsterStats = collision.GetComponent<MonsterStats>();
        PlayerStats.Instance.OnPlayerHit(this, monsterStats);

    }}

    public override void UseEquipment()
    {{
        if (!GetCanUseEquipment())
        {{
            Debug.Log(""攻击还在冷却中！"");
            return;
        }}
        base.UseEquipment();
    }}



    public override void DoDamage(float _cMag, MonsterStats monsterStats)
    {{
        base.DoDamage(_cMag, monsterStats);
        int dmg = (int)(currentDmg * _cMag);
        Debug.Log(dmg);
        monsterStats.TakeDirectDamage(dmg);

    }}
}}";

                    // 写入脚本文件
                    File.WriteAllText(scriptPath, scriptContent);
                    AssetDatabase.Refresh();  // 刷新以让 Unity 识别脚本

                    Debug.Log($"已自动创建脚本: {scriptPath}，请手动重新运行一次以加载新脚本。");
                    continue;

                }

                // 加上 BoxCollider2D 和 CircleCollider2D
                BoxCollider2D box = newGO.AddComponent<BoxCollider2D>();
                CircleCollider2D circle = newGO.AddComponent<CircleCollider2D>();

                box.isTrigger = true;
                circle.isTrigger = true;

                // 保存为 prefab
                string newPrefabPath = prefabPath;
                PrefabUtility.SaveAsPrefabAsset(newGO, newPrefabPath);

                // 创建完记得销毁临时GameObject
                GameObject.DestroyImmediate(newGO);

                Debug.Log($"创建了新的武器Prefab: {weaponName}");
            }
            else
            {
                Debug.Log($"Prefab已存在: {weaponName}");
            }
        }
    }
}
