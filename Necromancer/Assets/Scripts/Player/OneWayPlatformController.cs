using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI;

public class OneWayPlatformController : MonoBehaviour
{
    private Collider2D playerCollider;

    public float centerX ;

    // 用于控制下落时忽略碰撞的持续时间
    public float dropDuration = 0.5f;

    // 玩家脚本（用于获取状态和输入等，可选）
    private Player player;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
        player = GetComponent<Player>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            Collider2D platformCollider = collision.collider;
            Debug.Log("撞到了ll" + collision.gameObject.name + platformCollider.transform.position.x + platformCollider.transform.position.y + " " + transform.position.y);
            if (platformCollider != null)
            {
                Tilemap tilemap = platformCollider.GetComponentInParent<Tilemap>();
                if (tilemap != null)
                {

                    Vector2 hitPoint = player.transform.position; // 碰撞点的世界坐标
                    //Vector2 hitPoint = collision.GetContact(0).point; // 碰撞点的世界坐标
                    hitPoint.x = Mathf.Round(hitPoint.x - 0.5f) + 0.5f;
                    Debug.Log($"碰撞发生位置（世界坐标）: {hitPoint }");
                    Vector3Int cellPos = tilemap.WorldToCell(hitPoint + new Vector2(0, 1f));
                    Debug.Log("cellpos" + cellPos);
                    TileBase tile = tilemap.GetTile(cellPos);
                    
                    if (tile != null)
                    {
                        centerX = hitPoint.x;
                        Debug.Log($"1射中 Tile: {tile.name}，坐标: {cellPos} Centerx {centerX}");
                        player.stateMachine.ChangeState(player.oneWayState);
                        return;
                    }
                    else
                    {
                        Debug.Log("12313该位置没有 Tile");
                    }

                    cellPos = tilemap.WorldToCell(hitPoint + new Vector2(1, 1f));
                    Debug.Log("cellpos" + cellPos);
                    tile = tilemap.GetTile(cellPos);

                    if (tile != null)
                    {
                        centerX = hitPoint.x+ 1f;
                        Debug.Log($"2射中 Tile: {tile.name}，坐标: {cellPos} Centerx {centerX}");

                        player.stateMachine.ChangeState(player.oneWayState);
                        return;
                    }
                    else
                    {
                        Debug.Log("12313该位置没有 Tile");
                    }

                    cellPos = tilemap.WorldToCell(hitPoint + new Vector2(-1, 1f));
                    Debug.Log("cellpos" + cellPos);
                    tile = tilemap.GetTile(cellPos);

                    if (tile != null)
                    {
                        centerX = hitPoint.x - 1f;
                        Debug.Log($"3射中 Tile: {tile.name}，坐标: {cellPos} Centerx {centerX}");

                        player.stateMachine.ChangeState(player.oneWayState);
                        return;
                    }
                    else
                    {
                        Debug.Log("12313该位置没有 Tile");
                    }

                    cellPos = tilemap.WorldToCell(hitPoint + new Vector2(0, 0f));
                    Debug.Log("cellpos" + cellPos);
                    tile = tilemap.GetTile(cellPos);

                    if (tile != null)
                    {
                        centerX = hitPoint.x;
                        Debug.Log($"4射中 Tile: {tile.name}，坐标: {cellPos} Centerx {centerX}");
                        player.stateMachine.ChangeState(player.oneWayState);
                        return;
                    }
                    else
                    {
                        Debug.Log("12313该位置没有 Tile");
                    }

                    cellPos = tilemap.WorldToCell(hitPoint + new Vector2(1, 0f));
                    Debug.Log("cellpos" + cellPos);
                    tile = tilemap.GetTile(cellPos);

                    if (tile != null)
                    {
                        centerX = hitPoint.x + 1f;
                        Debug.Log($"5射中 Tile: {tile.name}，坐标: {cellPos} Centerx {centerX}");

                        player.stateMachine.ChangeState(player.oneWayState);
                        return;
                    }
                    else
                    {
                        Debug.Log("6该位置没有 Tile");
                    }

                    cellPos = tilemap.WorldToCell(hitPoint + new Vector2(-1, 0f));
                    Debug.Log("cellpos" + cellPos);
                    tile = tilemap.GetTile(cellPos);

                    if (tile != null)
                    {
                        centerX = hitPoint.x - 1f;
                        Debug.Log($"6射中 Tile: {tile.name}，坐标: {cellPos} Centerx {centerX}");

                        player.stateMachine.ChangeState(player.oneWayState);
                        return;
                    }
                    else
                    {
                        Debug.Log("12313该位置没有 Tile");
                    }
                }
                else
                {
                    if (platformCollider.transform.position.y > transform.position.y && !player.isClimbing)
                    {
                        Debug.Log("上墙");
                        player.stateMachine.ChangeState(player.oneWayState);
                    }
                }
            }

            
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            if (player.dropTimer > 0)
            {
                StartCoroutine(TemporarilyDisableCollision(collision.collider, dropDuration));
            }
            
        }
    }


    private IEnumerator TemporarilyDisableCollision(Collider2D platformCollider, float duration)
    {
        player.AddIgnoredPlatform(platformCollider);
        
        float timer = duration;
        while (timer > 0 && !player.isClimbing)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        
        // 仅当不在特殊状态时恢复
        if (!player.isClimbing &&  !player.IsInState(player.downDashState))
        {
            player.RemoveIgnoredPlatform(platformCollider);
        }
    }

}
