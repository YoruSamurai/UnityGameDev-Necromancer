using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizedDialogueSO))]
public class LocalizedDialogueSOEditor : Editor
{
    private bool foldout = true;

    public override void OnInspectorGUI()
    {
        // 获取目标对象
        LocalizedDialogueSO data = (LocalizedDialogueSO)target;

        if (data.localizedLines == null)
        {
            EditorGUILayout.HelpBox("localizedLines 为空", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("本地化条目总数: " + data.localizedLines.Count);

        foldout = EditorGUILayout.Foldout(foldout, "所有本地化内容", true);

        if (foldout)
        {
            EditorGUI.indentLevel++;
            foreach (var kvp in data.localizedLines)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($" 标识符: {kvp.identifier}", EditorStyles.boldLabel);

                if (kvp.identifier != null)
                {
                    EditorGUILayout.LabelField("简体中文", kvp.zh);
                    EditorGUILayout.LabelField("繁體中文", kvp.zh_TW);
                    EditorGUILayout.LabelField("English", kvp.en);
                    EditorGUILayout.LabelField("日本語", kvp.jp);
                }
                else
                {
                    EditorGUILayout.LabelField(" 内容为空");
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
