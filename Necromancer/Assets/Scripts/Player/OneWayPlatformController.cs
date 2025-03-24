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
            // 如果玩家按下 S+空格，则下落——暂时忽略与该平台的碰撞
            if (player.stateMachine.currentState.yInput < 0)
            {
                StartCoroutine(TemporarilyDisableCollision(collision.collider, dropDuration));
            }
            
        }
    }


    private IEnumerator TemporarilyDisableCollision(Collider2D platformCollider, float duration)
    {
        // 忽略碰撞
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        platformCollider.gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
        yield return new WaitForSeconds(duration);
        // 恢复碰撞
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        platformCollider.gameObject.layer = LayerMask.NameToLayer("Ground");

    }
}
