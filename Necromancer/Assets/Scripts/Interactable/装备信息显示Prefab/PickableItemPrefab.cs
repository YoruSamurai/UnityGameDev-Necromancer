using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickableItemPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshPro pickableName;
    [SerializeField] private TextMeshPro pickableMessage;
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private SpriteRenderer background;


    public void Initialize(string name,string message)
    {
        pickableName.text = name;
        pickableMessage.text = message;
    }

    public void DestroyPrefab()
    {
        StartCoroutine(FadeAndDestroy());
    }


    private IEnumerator FadeAndDestroy()
    {
        float timer = 0f;

        Color nameColor = pickableName.color;
        Color messageColor = pickableMessage.color;
        Color borderColor = border.color;
        Color backgroundColor = background.color;

        while (timer < .2f)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / .2f);
            pickableName.color = new Color(nameColor.r, nameColor.g, nameColor.b, alpha * .2f);
            pickableMessage.color = new Color(messageColor.r, messageColor.g, messageColor.b, alpha * .2f);
            border.color = new Color(borderColor.r, borderColor.g, borderColor.b, alpha);
            background.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a * alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        // 保证最终完全透明
        pickableName.color = new Color(nameColor.r, nameColor.g, nameColor.b, 0f);
        pickableMessage.color = new Color(messageColor.r, messageColor.g, messageColor.b, 0f);

        Destroy(gameObject);
    }

}
