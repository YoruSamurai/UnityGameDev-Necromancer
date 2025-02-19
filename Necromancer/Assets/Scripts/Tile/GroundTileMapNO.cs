using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GroundTileMapNO : MonoBehaviour
{
    public Tilemap ground_tilemap;//拖动获取地面瓦片地图
    public Tilemap environ_tilemap;
    public TileBase groundTile;//拖动获取地面规则瓦片Rule Tile,-
    public TileBase environTile;//拖动获取环境规则瓦片
    public Vector2Int lowerLeftCoo = new Vector2Int(-18, -7);//地图起始左下角位置
    public int width = 20;//地图宽度
    public int length = 100;//地图长度
    public float groundStartPro = 0.10f;//生成地面起始点的概率
    public Vector2Int groundLenRan = new Vector2Int(3, 10);//起始地面点生成的长度范围
    public float environmentRich = 0.5f;//环境丰富度

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(GroundStartPoi());
    }

    // Update is called once per frame
    IEnumerator GroundStartPoi()//生成地面起始点 用协程可以缓慢一步步生成地图，性能消耗少
    {
        //ground_tilemap.ClearAllTiles();
        Debug.Log("开始生成地面");
        for (int i = lowerLeftCoo.x; i < (this.length + lowerLeftCoo.x); i++)
        {
            for (int j = lowerLeftCoo.y; j < (this.width + lowerLeftCoo.y); j++)
            {
                yield return null;
                bool IsGround = j < (this.width + 3) ? (Random.value <= groundStartPro) : (Random.value <= groundStartPro + 0.05);//三元表达式，地面三行更容易生成地面起始点
                if (IsGround) StartCoroutine(GroundExtension(i, j));

            }
        }
        Debug.Log("结束生成地面");
        StartCoroutine(ClearChannel());//执行完GroundStartPoi()，GroundExtension（）生成地面，开始清除产生能走的通道

    }
    IEnumerator GroundExtension(int i, int j)//从地面起始点开始延长
    {
        int groundLen = Random.Range(groundLenRan.x, groundLenRan.y);
        for (int m = i; m <= i + groundLen; m++)
        {
            yield return null;
            ground_tilemap.SetTile(new Vector3Int(m, j, 0), groundTile);
        }

    }
    //清除，产生通道，思路就是从底层有方块的地方，开始判断上面。如果没有方块的话，就清除俩层/三层的通道
    IEnumerator ClearChannel()
    {
        Debug.Log("开始产生通道");
        for (int i = lowerLeftCoo.x; i < (this.length + lowerLeftCoo.x); i++)
        {
            for (int j = lowerLeftCoo.y; j < (this.width + lowerLeftCoo.y - 1); j++)//最高层上面必然没有方块，不需要判断，-1
            {
                if (ground_tilemap.GetTile(new Vector3Int(i, j, 0)) != null)//如果此处不为空方块
                {
                    if (ground_tilemap.GetTile(new Vector3Int(i, j + 1, 0)) == null)//如果此处上方为空方块
                    {
                        ground_tilemap.SetTilesBlock(new BoundsInt(i - 2, j + 1, 0, 3, 3, 1), new TileBase[] { null, null, null, null, null, null, null, null, null });//将上方3x3格子置空

                    }


                }
                yield return null;

            }
        }
        Debug.Log("产生通道完成");
        StartCoroutine(GenerateEnviron());
    }
    IEnumerator GenerateEnviron()
    {
        Debug.Log("开始生成花草");
        yield return null;
        for (int i = lowerLeftCoo.x; i < (this.length + lowerLeftCoo.x); i++)
        {
            for (int j = lowerLeftCoo.y; j < (this.width + lowerLeftCoo.y); j++)
            {
                if (ground_tilemap.GetTile(new Vector3Int(i, j, 0)) == groundTile && ground_tilemap.GetTile(new Vector3Int(i, j + 1, 0)) == null)//如果此处为地面，上面为空方块
                {

                    if (Random.value < environmentRich)//随机
                    { environ_tilemap.SetTile(new Vector3Int(i, j + 1, 0), environTile); }
                }
                yield return null;

            }
        }

    }
}