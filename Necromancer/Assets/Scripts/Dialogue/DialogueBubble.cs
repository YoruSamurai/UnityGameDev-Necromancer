using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshPro text;
    public SpriteRenderer background;

    [SerializeField] private Vector2 padding = new Vector2(0.5f, 0.3f);

    void LateUpdate()
    {
        UpdateBackgroundSize();
    }

    void UpdateBackgroundSize()
    {
        if (text == null || background == null) return;

        Vector2 textSize = GetTextSize();

        background.size = textSize + padding;
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
