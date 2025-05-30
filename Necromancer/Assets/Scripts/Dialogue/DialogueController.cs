using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{

    private SingleDialogue dialogue;
    private Transform speaker;
    private IDialogueSpeaker dialogueSpeaker;
    private int line = 0;

    private List<DialogueBubble> bubbles;

    [SerializeField] private GameObject dialogueBubblePrefab;

    public void Initialize(SingleDialogue _dialogue, Transform _speakerTransform,IDialogueSpeaker _dialogueSpeaker)
    {
        this.dialogue = _dialogue;
        this.speaker = _speakerTransform;
        dialogueSpeaker = _dialogueSpeaker;
        dialogueSpeaker.RemoveCurrentDialogue();
        dialogueSpeaker.SetCurrentDialogue(this);
        bubbles = new List<DialogueBubble>(); // 初始化 bubbles 列表
        StartDisplaying();
    }

    private void StartDisplaying()
    {
        if (dialogue.triggerList.Contains(DialogueTriggerType.Single))
        {
            string id = DialogueManager.Instance.GetTextById(dialogue.dialogueLineList[line].localizationIdentifier);
            bool isLastLine = dialogue.dialogueLineList.Count > line + 1 ? false : true ;
            GenerateDialogueBubble(id, dialogue.dialogueLineList[0].localizationIdentifier, transform, isLastLine);
        }
        else
        {
            
        }

    }

    public void RemoveBubble(DialogueBubble bubble)
    {
        if (bubbles.Contains(bubble))
        {
            bubbles.Remove(bubble); // 从列表中移除气泡
        }
    }

    private void ClearBubbles()
    {
        for (int i = bubbles.Count - 1; i >= 0; i--)
        {
            bubbles[i].DestroySelf();
        }
        bubbles.Clear();
    }



    private void GenerateDialogueBubble(string dialogueText, string dialogueIdentifier, Transform speakerTransform
        , bool isLastLine)
    {
        ClearBubbles();
        GameObject obj = Instantiate(dialogueBubblePrefab, transform.position, Quaternion.identity, speakerTransform);
        DialogueBubble bubble = obj.GetComponent<DialogueBubble>();
        bubble.Initialized(dialogueText, dialogueIdentifier, speakerTransform ,this,isLastLine);
        bubbles.Add(bubble); // 将当前气泡添加到列表中

    }
    /// <summary>
    /// 先清空所有泡泡，再清空自己
    /// </summary>
    public void DestroyController()
    {
        ClearBubbles();
        Destroy(gameObject);
    }

}
