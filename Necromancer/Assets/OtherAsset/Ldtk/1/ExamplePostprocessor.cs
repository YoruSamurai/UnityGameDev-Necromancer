using LDtkUnity;
using LDtkUnity.Editor;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace LDtkUnity
{


    public class ExamplePostprocessor : LDtkPostprocessor
    {
        protected override void OnPostprocessProject(GameObject root)
        {
            if (root == null)
            {
                Debug.LogError("LDtk Postprocess: root is null!");
                return;
            }

            LDtkComponentProject project = root.GetComponent<LDtkComponentProject>();
            if (project == null)
            {
                Debug.LogError("LDtk Postprocess: LDtkComponentProject is missing on root GameObject!");
                return;
            }

            Debug.Log($"Post process LDtk project: {root.name}");

            foreach (LDtkComponentWorld world in project.Worlds)
            {
                foreach (LDtkComponentLevel level in world.Levels)
                {
                    foreach (LDtkComponentLayer layer in level.LayerInstances)
                    {
                        LDtkComponentLayerTilesetTiles tiles = layer.GridTiles;
                        LDtkComponentLayerIntGridValues intGrid = layer.IntGrid;
                        Debug.Log(layer);
                        /*foreach (LDtkComponentEntity entity in layer.EntityInstances)
                        {
                            // Process entities
                            
                        }*/
                    }
                }
            }
        }

        protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
        {
            Debug.Log($"Post process LDtk level: {root.name}");
            LDtkComponentLevel level = root.GetComponent<LDtkComponentLevel>();
            foreach (LDtkComponentLayer layer in level.LayerInstances)
            {
                Debug.Log(layer.name);
                if (layer.name == "Climb")
                {
                    TilemapRenderer map = layer.GetComponentInChildren<TilemapRenderer>();
                    if (map.name.Contains("AutoLayer"))
                    {
                        map.sortingLayerID = SortingLayer.NameToID("Tile");
                        map.sortingOrder = 2;
                    }
                    CompositeCollider2D[] collider2D = layer.GetComponentsInChildren<CompositeCollider2D>();
                    foreach(var collider in collider2D)
                    {
                        if (collider.name.Contains("IntGrid"))
                        {
                            collider.isTrigger = true;
                        }
                    }

                }
                if (layer.name == "Ground1")
                {
                    Debug.Log("fukd");
                    TilemapRenderer map = layer.GetComponentInChildren<TilemapRenderer>();
                    if (map.name.Contains("AutoLayer"))
                    {
                        map.sortingLayerID = SortingLayer.NameToID("Tile");
                        map.sortingOrder = 3;
                    }
                    
                }
                
                //iterate upon layers
            }
        }
    }

 

}