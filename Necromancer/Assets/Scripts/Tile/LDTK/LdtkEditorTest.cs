

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LdtkTest))]
public class LdtkEditorTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LdtkTest generator = (LdtkTest)target;

        GUILayout.Space(10);
        if (GUILayout.Button("输出一些信息", GUILayout.Height(40)))
        {
            generator.OutputSomeMsg();
        }

        if (GUILayout.Button("分析门方向", GUILayout.Height(40)))
        {
           // generator.AnalyzeTileDirections();
        }

        if (GUILayout.Button("房间拼接1", GUILayout.Height(40)))
        {
            generator.GenerateLevelTest();
        }
    }
}

