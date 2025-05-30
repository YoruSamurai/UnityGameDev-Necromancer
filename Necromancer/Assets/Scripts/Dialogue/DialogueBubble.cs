using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshPro text;
    public SpriteRenderer background;
    private bool isLastLine;

    public SpriteRenderer arrow;

    [SerializeField] private Vector2 padding = new Vector2(0.5f, 0.3f);

    [SerializeField] private float destroyTimer;
    private float timer;

    private string dialogueText;
    private string dialogueIdentifier;
    private Transform speakerTransform;
    private DialogueController dialogueController;

    public void Initialized(string _dialogueText, string _dialogueIdentifier, Transform _speakerTransform,
        DialogueController controller,bool _isLastLine)
    {
        dialogueText = _dialogueText;
        dialogueIdentifier = _dialogueIdentifier;
        speakerTransform = _speakerTransform;
        dialogueController = controller;
        isLastLine = _isLastLine;
        RandomPitchPosition();
        timer = destroyTimer;
        SetText(dialogueText);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        // 通知 DialogueController 移除此气泡
        if(dialogueController != null)
        {
            dialogueController.RemoveBubble(this);
        }
        Destroy(gameObject);
    }

    private void RandomPitchPosition()
    {
        if (speakerTransform != null)
        {
            // 获取当前的位置
            Vector3 currentPosition = speakerTransform.position;

            // 随机生成 X 偏移量
            float randomXOffset;
            if (Random.value < 0.5f) // 50% 概率选择 [-2, -1] 区间
            {
                randomXOffset = Random.Range(-1f, -.2f);
            }
            else // 50% 概率选择 [1, 2] 区间
            {
                randomXOffset = Random.Range(.2f, 1f);
            }

            // 更新位置
            transform.position = new Vector3(currentPosition.x + randomXOffset, currentPosition.y + 1.5f, currentPosition.z);
        }
    }


    void LateUpdate()
    {
        UpdateBackgroundSize();
    }

    void UpdateBackgroundSize()
    {
        if (text == null || background == null || arrow == null) return;

        Vector2 textSize = GetTextSize();
        background.size = textSize + padding;

        // 计算背景的下边缘位置
        Vector3 backgroundBottomEdge = transform.position + new Vector3(0, -background.size.y / 2, 0);
        backgroundBottomEdge.x = transform.position.x - (transform.position.x - speakerTransform.position.x) * 0.1f;
        // 设置箭头位置为背景的下边缘
        arrow.transform.position = backgroundBottomEdge;

        // 根据位置决定箭头的翻转
        if (transform.position.x < speakerTransform.position.x)
        {
            // 在左侧，箭头朝右
            arrow.flipX = false;
        }
        else
        {
            // 在右侧，箭头朝左
            arrow.flipX = true;
        }
    }

    Vector2 GetTextSize()
    {
        text.ForceMeshUpdate();

        // 获取文本渲染区域的真实大小
        var textBounds = text.textBounds;
        return new Vector2(textBounds.size.x, textBounds.size.y);
    }

    public void SetText(string message)
    {
        text.text = message;
        UpdateBackgroundSize();
    }
}
