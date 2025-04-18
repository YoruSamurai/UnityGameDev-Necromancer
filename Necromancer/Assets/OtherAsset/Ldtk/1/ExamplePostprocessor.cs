

using LDtkUnity;
using LDtkUnity.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Cinemachine.DocumentationSortingAttribute;
namespace LDtkUnity
{


    public class ExamplePostprocessor : LDtkPostprocessor
    {
        protected override void OnPostprocessProject(GameObject root)
        {
            // 获取所有.ldtkl文件路径
            string folderPath = $"Assets/OtherAsset/Ldtk/Church_level/{root.name}";
            string[] ldtklPaths = System.IO.Directory.GetFiles(folderPath, "*.ldtkl");
            List<LDtkComponentLevel> ldtkLevels = GetAllLdtkLevels(ldtklPaths);
            
                
        }

        private List<LDtkComponentLevel> GetAllLdtkLevels(string[] paths)
        {
            List<LDtkComponentLevel> ldtkLevels = new List<LDtkComponentLevel>();
            foreach (string path in paths)
            {
                // 加载LDtk Level文件
                GameObject levelFile = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (levelFile == null)
                {
                    Debug.LogWarning($"无法加载LDtk文件: {path}");
                }
                LDtkComponentLevel level = levelFile.GetComponent<LDtkComponentLevel>();
                if(level != null)
                {
                    Debug.Log("把"  + level.name + "加入直播间");
                    ldtkLevels.Add(level);
                }
            }
            return ldtkLevels;
        }
        

        //1:获取所有LDtkComponentLevel
        //2：对所有Level进行后处理 修改他们的sortingLayer 和那个
        //3：我们创建一个新SO 存储LDTKLEVEL 还有JSON里面的信息 各种吧

        protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
        {
            LDtkComponentLevel level = root.GetComponent<LDtkComponentLevel>();
            
            ProcessSortingLayer(level);
            
        }

        private void ProcessSortingLayer(LDtkComponentLevel level)
        {
            foreach (LDtkComponentLayer layer in level.LayerInstances)
            {
                if (layer == null)
                {
                    continue;
                }
                Debug.Log(layer);
                if (layer.name == "AutoLadderTile")
                {
                    TilemapRenderer map = layer.GetComponentInChildren<TilemapRenderer>();
                    map.sortingLayerID = SortingLayer.NameToID("Tile");
                    map.sortingOrder = 4;

                }
                if(layer.name == "Ladder")
                {
                    CompositeCollider2D[] collider2D = layer.GetComponentsInChildren<CompositeCollider2D>();
                    foreach (var collider in collider2D)
                    {
                        if (collider.name.Contains("IntGrid"))
                        {
                            collider.isTrigger = true;
                        }
                    }

                }
                if (layer.name == "AutoOneWayTile")
                {
                    TilemapRenderer map = layer.GetComponentInChildren<TilemapRenderer>();
                    map.sortingLayerID = SortingLayer.NameToID("Tile");
                    map.sortingOrder = 3;
                }
                if (layer.name == "AutoGroundTile")
                {
                    TilemapRenderer map = layer.GetComponentInChildren<TilemapRenderer>();
                    map.sortingLayerID = SortingLayer.NameToID("Tile");
                    map.sortingOrder = 5;


                }
            }

        }


    }

 

}
