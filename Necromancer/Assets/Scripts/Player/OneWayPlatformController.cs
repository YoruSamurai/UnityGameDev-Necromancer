using System.Collections;
using UnityEngine;

public class OneWayPlatformController : MonoBehaviour
{
    private Collider2D playerCollider;

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
            // 获取平台的 Collider2D
            Collider2D platformCollider = collision.collider;
            if (platformCollider.transform.position.y > transform.position.y && !player.isClimbing)
            {
                Debug.Log("上墙");
                player.stateMachine.ChangeState(player.oneWayState);
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
