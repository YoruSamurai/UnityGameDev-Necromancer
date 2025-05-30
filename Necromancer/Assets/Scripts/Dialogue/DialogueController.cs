using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{

    private SingleDialogue dialogue;
    private Transform speaker;
    private IDialogueSpeaker dialogueSpeaker;
    private int line = 0;
    public bool isSingle;
    private bool isLastLine;

    private DialogueBubble currentBubble;

    [SerializeField] private GameObject dialogueBubblePrefab;

    public void Initialize(SingleDialogue _dialogue, Transform _speakerTransform,IDialogueSpeaker _dialogueSpeaker)
    {
        this.dialogue = _dialogue;
        this.speaker = _speakerTransform;
        dialogueSpeaker = _dialogueSpeaker;
        dialogueSpeaker.RemoveCurrentDialogue();
        dialogueSpeaker.SetCurrentDialogue(this);
        currentBubble = null;
        isSingle = dialogue.triggerList.Contains(DialogueTriggerType.Single)? true: false;
        StartDisplaying();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && !isSingle)
        {
            StartDisplaying();
        }
    }

    private void StartDisplaying()
    {
        if(currentBubble == null)
        {
            string id = DialogueManager.Instance.GetTextById(dialogue.dialogueLineList[line].localizationIdentifier);
            isLastLine = dialogue.dialogueLineList.Count > line + 1 ? false : true;
            GenerateDialogueBubble(id, dialogue.dialogueLineList[line].localizationIdentifier, transform, isLastLine, isSingle);
            line = line + 1;
        }
        else if (!isLastLine)
        {
            if (currentBubble.isTyping)
            {
                currentBubble.SkipTyping();
            }
            else
            {
                string id = DialogueManager.Instance.GetTextById(dialogue.dialogueLineList[line].localizationIdentifier);
                isLastLine = dialogue.dialogueLineList.Count > line + 1 ? false : true;
                GenerateDialogueBubble(id, dialogue.dialogueLineList[line].localizationIdentifier, transform, isLastLine,isSingle);
                line = line + 1;
            }
        }
        else
        {
            Debug.LogWarning("没有对话了");
            if (currentBubble.isTyping)
            {
                currentBubble.SkipTyping();
            }
            else
            {
                dialogueSpeaker.RemoveCurrentDialogue();
            }
        }

    }

    public void RemoveBubble()
    {
        currentBubble = null;
    }

    private void ClearBubbles()
    {
        if(currentBubble != null)
        {
            currentBubble.DestroySelf();
        
            currentBubble = null;
        }
    }



    private void GenerateDialogueBubble(string dialogueText, string dialogueIdentifier, 
        Transform speakerTransform, bool isLastLine,bool isSingle)
    {
        ClearBubbles();
        GameObject obj = Instantiate(dialogueBubblePrefab, transform.position, Quaternion.identity, speakerTransform);
        DialogueBubble bubble = obj.GetComponent<DialogueBubble>();
        bubble.Initialized(dialogueText, dialogueIdentifier, speakerTransform ,this,isLastLine,isSingle);
        currentBubble = bubble;

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
