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
            //在不爬的时候，我们攀登上去
            Debug.Log(player.rb.velocity.y);
            if (platformCollider.transform.position.y > transform.position.y && !player.isClimbing && player.rb.velocity.y >= 0)
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
            //Debug.Log("再见ELI" + player.stateMachine.currentState.yInput);
            // 如果玩家按下 S+空格，则下落——暂时忽略与该平台的碰撞
            if (player.dropTimer > 0)
            {
                //Debug.Log("1再见ELI");
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
        if (!player.isClimbing && !(player.stateMachine.currentState is PlayerDownDashState))
        {
            player.RemoveIgnoredPlatform(platformCollider);
        }
    }

    /*// **新增方法，进入攀爬状态时禁用碰撞**
    public void DisablePlatformCollision(Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        platformCollider.gameObject.layer = LayerMask.NameToLayer("OneWayPlatform");
    }

    // **新增方法，退出攀爬状态时恢复碰撞**
    public void EnablePlatformCollision(Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        platformCollider.gameObject.layer = LayerMask.NameToLayer("Ground");
    }*/
}
