using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CSVToDialogueSO : EditorWindow
{
    private string csvFilePath;
    private string csvLocaleFilePath;

    [MenuItem("Tools/导入对话 CSV")]
    public static void OpenWindow()
    {
        GetWindow<CSVToDialogueSO>("CSV 导入器");///1
    }

    private void OnGUI()
    {
        GUILayout.Label("导入 CSV 文件生成 DialogueSO", EditorStyles.boldLabel);
        if (GUILayout.Button("选择对话 CSV 文件"))
        {
            csvFilePath = EditorUtility.OpenFilePanel("选择 CSV 文件", "", "csv");
        }

        GUILayout.Label("当前选择的文件: " + csvFilePath);

        if (GUILayout.Button("生成 DialogueSO"))
        {
            if (!string.IsNullOrEmpty(csvFilePath))
                GenerateSO();
            else
                Debug.LogError("请先选择一个 CSV 文件！");
        }

        if (GUILayout.Button("选择本地化 CSV 文件"))
        {
            csvLocaleFilePath = EditorUtility.OpenFilePanel("选择 CSV 文件", "", "csv");
        }

        GUILayout.Label("当前选择的文件: " + csvLocaleFilePath);

        if (GUILayout.Button("生成 LocaleSO"))
        {
            if (!string.IsNullOrEmpty(csvLocaleFilePath))
                GenerateLocaleSO();
            else
                Debug.LogError("请先选择一个 CSV 文件！");
        }
    }
    private void GenerateLocaleSO()
    {
        List<string> lines = ReadCSV(csvLocaleFilePath);
        Dictionary<string, LocalizedLine> _localizedLines = new();
        _localizedLines.Clear();
        for (int i = 1; i < lines.Count;i++)
        {
            string[] parts = lines[i].Split(',');
            if (string.IsNullOrEmpty(parts[0]))
            {
                Debug.LogWarning("读完了");
                break;
            }
            _localizedLines[parts[1]] = new LocalizedLine
            {
                zh = parts[2],
                zh_TW = parts[3],
                en = parts[4],
                jp = parts[5],
            };
        }
        LocalizedDialogueSO so = ScriptableObject.CreateInstance<LocalizedDialogueSO>();
        so.localizedLines = _localizedLines;
        AssetDatabase.CreateAsset(so, "Assets/ScriptableObjects/Dialogue/Locale.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("✅ 导入完成，共生成 " + _localizedLines.Count + " 个对话。");
    }

    private List<string> ReadCSV(string filePath)
    {
        // List<string[]> 是一个字符串数组的列表
        List<string> data = new List<string>();

        using (var reader = new StreamReader(filePath)) // reader 的类型是 StreamReader
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine(); // line 的类型是 string
                data.Add(line);
            }
        }

        return data; // 返回类型是 List<string[]>
    }

    private void GenerateSO()
    {
        List<string> lines = ReadCSV(csvFilePath);

        Dictionary<int, SingleDialogue> dialogueDict = new();

        int currentID = -1;
        string currentTrigger = "";
        string currentIdentifier = "";
        string currentType = "";
        string currentSpeaker = "";

        for (int i = 1; i < lines.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] parts = lines[i].Split(',');

            // 安全补足字段数（避免数组越界）
            while (parts.Length < 8)
            {
                System.Array.Resize(ref parts, 8);
            }

            if (!string.IsNullOrWhiteSpace(parts[0]))
            {
                currentID = int.Parse(parts[0]);
                currentTrigger = parts[1];

            }
            currentIdentifier = parts[2];
            currentType = parts[3];
            currentSpeaker = parts[6];
            

            // 防止非法行
            if (currentID == -1)
                continue;

            string Identifier = parts[2];
            if (string.IsNullOrWhiteSpace(Identifier))
            {
                Debug.LogWarning($"⚠️ 第 {i + 1} 行缺少标识符，已终止读取。");
                break;
            }

            int lineIndex = int.TryParse(parts[4], out int li) ? li : 0;
            int nextIndex = int.TryParse(parts[5], out int ni) ? ni : -1;
            string text = parts[7];

            // 添加或获取当前对话
            if (!dialogueDict.TryGetValue(currentID, out var sd))
            {
                sd = new SingleDialogue
                {
                    dialogueID = currentID,
                    triggerList = ParseTriggers(currentTrigger),
                    dialogueLineList = new List<DialogueLine>()
                };
                dialogueDict[currentID] = sd;
            }

            DialogueLine line = new DialogueLine
            {
                localizationIdentifier = currentIdentifier,
                lineIndex = lineIndex,
                nextLineIndex = nextIndex,
                dialogueType = ParseDialogueType(currentType),
                text = text
            };

            sd.dialogueLineList.Add(line);
        }

        DialogueSO so = ScriptableObject.CreateInstance<DialogueSO>();
        so.DialogueList = new List<SingleDialogue>(dialogueDict.Values);
        AssetDatabase.CreateAsset(so, "Assets/ScriptableObjects/Dialogue/1.asset");
        AssetDatabase.SaveAssets();

        Debug.Log("✅ 导入完成，共生成 " + so.DialogueList.Count + " 个对话。");
    }

    private List<DialogueTriggerType> ParseTriggers(string input)
    {
        List<DialogueTriggerType> list = new();
        if (string.IsNullOrWhiteSpace(input)) return list;

        var parts = input.Split('\\');
        foreach (var part in parts)
        {
            string parsed = part.Replace("地图-", "Map_").Replace("对话", "Nearby");
            if (System.Enum.TryParse(parsed, true, out DialogueTriggerType result))
                list.Add(result);
        }
        return list;
    }

    private DialogueType ParseDialogueType(string str)
    {
        Debug.Log(str);
        return str switch
        {
            "D" => DialogueType.Dialogue,
            "S" => DialogueType.Selection,
            "End" => DialogueType.End,
            _ => DialogueType.Dialogue
        };
    }
}
