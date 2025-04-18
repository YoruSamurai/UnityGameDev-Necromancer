using UnityEditor;
using UnityEngine;
using System.IO;
using LDtkUnity;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class LDTK处理工具 : EditorWindow
{
    private int i = 0;

    [MenuItem("Tools/LDTK 处理工具")]
    public static void ShowWindow()
    {
        GetWindow<LDTK处理工具>("LDTK 处理工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("请输入 LDTK 关卡索引:", EditorStyles.boldLabel);
        i = EditorGUILayout.IntField("索引 (i):", i);

        if (GUILayout.Button("重新导入所有 LDtk Levels"))
        {
            ReimportLevels();
        }
    }

    private void ReimportLevels()
    {
        string levelsFolder = $"Assets/OtherAsset/Ldtk/Church_level/church_mi2/";
        if (!Directory.Exists(levelsFolder))
        {
            Debug.LogError($"文件夹不存在: {levelsFolder}");
            return;
        }

        string[] levelFiles = Directory.GetFiles(levelsFolder, "*.ldtkl", SearchOption.TopDirectoryOnly);
        if (levelFiles.Length == 0)
        {
            Debug.LogWarning($"在 {levelsFolder} 中未找到任何 .ldtkl 文件。");
            return;
        }

        List<LdtkLevelSO> allLevelSOs = new List<LdtkLevelSO>(); // 用于收集所有关卡 SO

        foreach (string file in levelFiles)
        {
            AssetDatabase.ImportAsset(file, ImportAssetOptions.ForceUpdate);
        }

        AssetDatabase.Refresh();

        // 延迟一帧确保资源都已刷新，再开始处理
        EditorApplication.delayCall += () =>
        {
            Debug.Log($"所有 LDtk Levels 重新导入完成（目录: {levelsFolder}）。开始创建 SO...");

            foreach (string file in levelFiles)
            {
                // 加载关卡 GameObject
                GameObject levelGO = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                if (levelGO == null)
                {
                    Debug.LogWarning($"无法加载关卡 GameObject: {file}");
                    continue;
                }

                LDtkComponentLevel level = levelGO.GetComponent<LDtkComponentLevel>();
                if (level == null)
                {
                    Debug.LogWarning($"GameObject 没有 LDtkComponentLevel 组件: {file}");
                    continue;
                }

                // 使用关卡 GameObject 的名字作为SO名称
                string levelName = level.name;
                LdtkLevelSO levelSO = CreateOrUpdateLevelSO(level, levelName);

                if (levelSO != null)
                {
                    allLevelSOs.Add(levelSO);
                }
            }

            // 在相同目录创建 LdtkLevelSoList
            CreateLdtkLevelSoList(allLevelSOs, $"Assets/OtherAsset/Ldtk/Church_level/church_mi2/SO/");
        };
    }
    private LdtkLevelSO CreateOrUpdateLevelSO(LDtkComponentLevel level, string levelName)
    {
        string soFolder = $"Assets/OtherAsset/Ldtk/Church_level/church_mi2/SO";
        if (!Directory.Exists(soFolder))
        {
            Directory.CreateDirectory(soFolder);
        }
        string soPath = $"{soFolder}/{levelName}_SO.asset";

        // 尝试加载现有的SO
        LdtkLevelSO levelSO = AssetDatabase.LoadAssetAtPath<LdtkLevelSO>(soPath);
        if (levelSO == null)
        {
            levelSO = ScriptableObject.CreateInstance<LdtkLevelSO>();
            AssetDatabase.CreateAsset(levelSO, soPath);
        }

        // 更新数据
        levelSO.levelData = level;
        levelSO.gameRoomType = level.FieldInstances.GetEnum<GameRoomType>("GameRoomType");
        levelSO.levelHeight = level.BorderRect.height;
        levelSO.levelWidth = level.BorderRect.width;
        levelSO.doorInfos = GetAllDoors(level); // 获取所有门信息

        // 标记为脏并保存
        EditorUtility.SetDirty(levelSO);
        AssetDatabase.SaveAssets();

        Debug.Log($"生成 Level SO: {soPath}");
        return levelSO;
    }

    private void CreateLdtkLevelSoList(List<LdtkLevelSO> levelSOs, string saveFolder)
    {
        if (levelSOs.Count == 0)
        {
            Debug.LogWarning("未找到任何 LdtkLevelSO，不创建 LdtkLevelSoList。");
            return;
        }

        string soListPath = $"{saveFolder}/LdtkLevelSoList.asset";

        // 尝试加载已存在的 LdtkLevelSoList
        LdtkLevelSoList levelSoList = AssetDatabase.LoadAssetAtPath<LdtkLevelSoList>(soListPath);
        if (levelSoList == null)
        {
            levelSoList = ScriptableObject.CreateInstance<LdtkLevelSoList>();
            AssetDatabase.CreateAsset(levelSoList, soListPath);
        }

        // 更新数据
        levelSoList.ldtkLevelSoList = levelSOs;

        // 标记为脏并保存
        EditorUtility.SetDirty(levelSoList);
        AssetDatabase.SaveAssets();

        Debug.Log($"生成 LdtkLevelSoList: {soListPath}");
    }

    private List<DoorInfo> GetAllDoors(LDtkComponentLevel room)
    {
        List<DoorInfo> doorInfos = new List<DoorInfo>();

        // 找到门对象（假设名字为 "Door"）
        Transform doorTransform = room.transform.Find("Door");
        if (doorTransform == null)
        {
            Debug.LogWarning($"No Door object found in room: {room.name}");
            return doorInfos;
        }

        // 获取门对象下的 Tilemap 组件
        Tilemap doorLayer = doorTransform.GetComponentInChildren<Tilemap>();
        if (doorLayer == null)
        {
            Debug.LogWarning($"No Tilemap component found on Door object in room: {room.name}");
            return doorInfos;
        }

        // 获取关卡的边界（用于计算门的方向）
        Rect levelRect = room.BorderRect;

        // 用 HashSet 记录已处理过的单元格
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        foreach (Vector3Int cellPos in doorLayer.cellBounds.allPositionsWithin)
        {
            //Debug.Log(cellPos);
            if (!doorLayer.HasTile(cellPos))
                continue;

            if (visited.Contains(cellPos))
                continue;

            // 根据单元格位置和关卡边界获取门方向（你已有此方法）
            DoorDir doorDir = GetDoorDirection(cellPos, levelRect);

            int length = 1;
            visited.Add(cellPos);

            // 对于垂直门，检查上方相邻单元格
            if (doorDir == DoorDir.Left || doorDir == DoorDir.Right)
            {
                Vector3Int nextCell = new Vector3Int(cellPos.x, cellPos.y + 1, cellPos.z);
                while (doorLayer.HasTile(nextCell) && GetDoorDirection(nextCell, levelRect) == doorDir)
                {
                    visited.Add(nextCell);
                    length++;
                    nextCell.y++;
                }
            }
            
            // 分组：对于水平门（假设沿 x 轴连续），检查右侧相邻单元格
            else if (doorDir == DoorDir.Top || doorDir == DoorDir.Bottom)
            {
                Vector3Int nextCell = new Vector3Int(cellPos.x + 1, cellPos.y, cellPos.z);
                while (doorLayer.HasTile(nextCell) && GetDoorDirection(nextCell, levelRect) == doorDir)
                {
                    visited.Add(nextCell);
                    length++;
                    nextCell.x++;
                }
                
            }

            DoorInfo info = new DoorInfo();
            info.startCell = cellPos;
            info.length = length;
            info.direction = doorDir;
            info.isLocked = false;
            info.node = -1;
            doorInfos.Add(info);
        }
        return doorInfos;
    }

    //获取门的方向 这样就差不多了
    private DoorDir GetDoorDirection(Vector3 cellPos, Rect levelRect)
    {
        float tolerance = 1f;
        if (Mathf.Abs(cellPos.x + .5f) < tolerance) return DoorDir.Left;
        if (Mathf.Abs(cellPos.x + .5f - levelRect.width) < tolerance) return DoorDir.Right;
        if (Mathf.Abs(cellPos.y + .5f) < tolerance) return DoorDir.Bottom;
        if (Mathf.Abs(cellPos.y + .5f - levelRect.height) < tolerance) return DoorDir.Top;
        return DoorDir.Left; // 默认值
    }
}