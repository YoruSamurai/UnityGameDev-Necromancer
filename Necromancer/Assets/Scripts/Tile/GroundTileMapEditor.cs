#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundTileMap))]
public class GroundTileMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GroundTileMap generator = (GroundTileMap)target;

        GUILayout.Space(10);
        if (GUILayout.Button("生成地图", GUILayout.Height(40)))
        {
            generator.GenerateMap();
        }

        if (GUILayout.Button("清除地图", GUILayout.Height(40)))
        {
            generator.ClearMap();
        }
    }
}
#endif