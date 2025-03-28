using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ldtk调试 : MonoBehaviour
{
    [SerializeField] public LayerMask whatIsGround;


    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5f, whatIsGround);
        if (hit.collider != null)
        {
            Tilemap tilemap = hit.collider.GetComponentInParent<Tilemap>();
            if (tilemap != null)
            {
                Vector3Int cellPos = tilemap.WorldToCell(hit.point - new Vector2(0,0.1f));
                TileBase tile = tilemap.GetTile(cellPos);
                if (tile != null)
                {
                    Debug.Log($"射中 Tile: {tile.name}，坐标: {cellPos}");
                }
                else
                {
                    Debug.Log("该位置没有 Tile");
                }
            }
            else
            {
                Debug.Log("射中的不是 Tilemap: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("没有射到任何物体");
        }
    }
}
