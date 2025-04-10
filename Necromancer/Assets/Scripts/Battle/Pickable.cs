using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] public IPickableItem pickableItem;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private PickableItemPrefab itemMessage;
    [SerializeField] private float duration;



    [Header("拾取提示 UI")]
    //public TextMeshPro pickupUIText;

    private bool isPlayerInRange = false;
    private Rigidbody2D rb;
    private bool isStopped = false;

    [SerializeField] private bool isPoping = false;
    private bool popToLeft = false;
    private bool isSetDir = false;
    int count = 1;

    public void SetPickable(IPickableItem item)
    {
        pickableItem = item;

        spriteRenderer.sprite = item.GetSprite();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 设置初始斜向上速度
        rb.velocity = new Vector2(Random.Range(-3f,3f), -3f); // 这里的速度可以根据需要调整
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 获取玩家离开触发器的位置
            Vector2 enterPos = other.transform.position;

            itemMessage = MessageShowUtil.Instance.ShowPickableMessage(true,pickableItem.GetItemName(),pickableItem.GetItemMessage(), enterPos,null);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickable") && !isPoping && rb.bodyType != RigidbodyType2D.Static)
        {
            Debug.Log("不要弹弹乐");
            Rigidbody2D rb2d = other.GetComponentInParent<Rigidbody2D>();
            if (rb2d.bodyType == RigidbodyType2D.Static)
            {
                isPoping = true;
                
                if (!isSetDir)
                {
                    isSetDir = true;
                    popToLeft = Random.value < 0.5f? true: false;
                }
                StartCoroutine(PopAway());
            }
        }
    }

    private IEnumerator PopAway()
    {
        Debug.Log("正在pop");
        float horizontalDirection = popToLeft? -1 : 1;
        rb.velocity = new Vector2 (horizontalDirection * 8f, 5f);
        rb.gravityScale = 5f;
        yield return new WaitForSeconds(duration);
        isPoping = false;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            MessageShowUtil.Instance.ShowPickableMessage(false, pickableItem.GetItemName(), pickableItem.GetItemMessage(),new Vector2(0,0), itemMessage);
        }
    }

    private void FixedUpdate()
    {
        // 检查刚体的 x 和 y 速度
        count += 1;
        if (count == 3)
        {
            count = 0;
            if (rb.velocity.x == 0 && rb.velocity.y == 0f && !isStopped && !isPoping)
            {
                isStopped = true;
                rb.bodyType = RigidbodyType2D.Static;
                MoveUpAndDown(); // 开始上下移动
            }
        }
        

    }

    private void MoveUpAndDown()
    {
        // 使用 DOTween 进行上下移动
        float moveDistance = 0.5f;
        float duration = 1f; // 上下移动的持续时间

        // 先移动到上方
        transform.DOMoveY(transform.position.y + moveDistance, duration)
            .SetEase(Ease.OutSine) // 使用缓和函数
            .OnComplete(() =>
            {
                // 移动到下方
                transform.DOMoveY(transform.position.y - moveDistance, duration)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() =>
                    {
                        // 递归调用以实现循环效果
                        MoveUpAndDown();
                    });
            });
    }

    public void Pickup()
    {
        if (pickableItem != null)
        {
            pickableItem.OnPickup(); // 触发自定义的拾取逻辑
        }

        Destroy(gameObject);
    }
}
