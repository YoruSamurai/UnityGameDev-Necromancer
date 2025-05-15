using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class 关卡流程编辑器 : EditorWindow
{
    private string csvFilePath;


    private List<GameRoomType?[,]> lastRoomGrid = null; // 用于可视化
    private string lastBlockName;          // 标题显示

    [MenuItem("Tools/关卡流程编辑")]
    public static void ShowWindow()
    {
        GetWindow<关卡流程编辑器>("关卡流程编辑器");
    }

    private void OnGUI()
    {
        GUILayout.Label("关卡流程编辑器", EditorStyles.boldLabel);

        if (GUILayout.Button("选择 CSV 文件"))
        {
            csvFilePath = EditorUtility.OpenFilePanel("选择 CSV 文件", "", "csv");
        }

        GUILayout.Label("当前选择的文件: " + csvFilePath);

        if (GUILayout.Button("更新关卡 SO"))
        {
            if (!string.IsNullOrEmpty(csvFilePath))
            {
                UpdateRoomGraphData();
            }
            else
            {
                Debug.LogWarning("请先选择一个 CSV 文件。");
            }
        }

        // 可视化房间网格
        if (lastRoomGrid != null)
        {
            foreach (var grid in lastRoomGrid)
            {
                GUILayout.Space(20);
                GUILayout.Label($"可视化房间结构：{lastBlockName}", EditorStyles.boldLabel);

                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);

                for (int r = 0; r < rows; r++)
                {
                    GUILayout.BeginHorizontal();
                    for (int c = 0; c < cols; c++)
                    {
                        GameRoomType? cell = grid[r, c];
                        string label = cell?.ToString() ?? "-";
                        GUILayout.Button(label, GUILayout.Width(60), GUILayout.Height(30));
                    }
                    GUILayout.EndHorizontal();
                }

            }
        }
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

    private void UpdateRoomGraphData()
    {
        // List<string[]> 是一个字符串数组的列表
        // 清空并初始化
        lastRoomGrid = new List<GameRoomType?[,]>();
        List<string[]> csvData = ReadCSV(csvFilePath);
        UpdateRoomGraphSO(csvData);
        
    }

    private void UpdateRoomGraphSO(List<string[]> csvData)
    {
        int currentRow = 1; // 跳过表头
        while (currentRow < csvData.Count)
        {
            // 检查当前是否为房间块的起始
            string[] row = csvData[currentRow];
            if (string.IsNullOrWhiteSpace(row[0]))
            {
                currentRow++;
                continue;
            }

            string blockName = row[0].Trim();
            string blockNum = row[1].Trim();
            int startRow = currentRow;

            // 向下查找此段块的结束（下一个非空第一列行 或 结尾）
            int endRow = startRow + 1;
            while (endRow < csvData.Count && string.IsNullOrWhiteSpace(csvData[endRow][0]))
            {
                endRow++;
            }

            int roomIndex = 0;
            int colCount = csvData[startRow].Length - 2; // 忽略前两列（名称和类型）
            int rowCount = endRow - startRow;
            int[,] roomIDGrid = new int[rowCount, colCount]; // 用于记录每个房间对应的 roomID
            // 构建二维房间表
            GameRoomType?[,] roomGrid = new GameRoomType?[rowCount, colCount];

            for (int c = 2; c < csvData[startRow].Length; c++) // 列，从第3列开始
            {
                int realCol = c - 2;

                for (int r = 0; r < rowCount; r++) // 行，从第startRow行开始
                {
                    string[] dataRow = csvData[startRow + r];
                    if (c >= dataRow.Length) continue;

                    string cell = dataRow[c].Trim();
                    if (string.IsNullOrEmpty(cell)) continue;

                    GameRoomType roomType = Enum.TryParse(cell, out GameRoomType parsedType)
                                            ? parsedType : GameRoomType.Normal;

                    roomGrid[r, realCol] = roomType;

                    roomIndex++;
                    roomIDGrid[r, realCol] = roomIndex; // 💡 记录该房间的 roomID
                }
            }


            List<RoomNode> roomNodes = new();
            int rows = roomGrid.GetLength(0);
            int cols = roomGrid.GetLength(1);

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    GameRoomType? cell = roomGrid[r, c];
                    if (cell.HasValue)
                    {
                        GameRoomType roomType = cell.Value;
                        RoomNode node = new RoomNode();
                        node.roomID = roomIDGrid[r, c]; // 💡 当前房间 ID
                        node.roomType = roomType;
                        node.connectionRoom = new List<int>();

                        // 下
                        if (r < rows - 1 && roomGrid[r + 1, c].HasValue)
                        {
                            node.connectionRoom.Add(roomIDGrid[r + 1, c]);
                        }

                        /*// 上
                        if (r > 0 && roomGrid[r - 1, c].HasValue)
                        {
                            node.connectionRoom.Add(roomIDGrid[r - 1, c]);
                        }

                        // 左
                        if (c > 0 && roomGrid[r, c - 1].HasValue)
                        {
                            node.connectionRoom.Add(roomIDGrid[r, c - 1]);
                        }*/

                        // 右
                        if (c < cols - 1 && roomGrid[r, c + 1].HasValue)
                        {
                            node.connectionRoom.Add(roomIDGrid[r, c + 1]);
                        }

                        roomNodes.Add(node);
                    }
                }
            }

            // 生成 ScriptableObject
            RoomGraph so = ScriptableObject.CreateInstance<RoomGraph>();
            so.allRooms = roomNodes;
            // 缓存当前房间网格用于可视化

            lastRoomGrid.Add(roomGrid);
            lastBlockName = blockName;
            string dirPath = "Assets/ScriptableObjects/关卡流程SO配置/教堂";
            Directory.CreateDirectory(dirPath);
            string fileName = $"{blockName}_{blockNum}.asset";
            string assetPath = Path.Combine(dirPath, fileName);

            AssetDatabase.CreateAsset(so, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"生成成功：{assetPath}");

            currentRow = endRow;
        }
    }

    

}
