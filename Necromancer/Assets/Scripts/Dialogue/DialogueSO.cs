using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Ohter/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public List<SingleDialogue> DialogueList;
}

[Serializable]
public struct SingleDialogue
{
    public int dialogueID;
    public List<DialogueTriggerType> triggerList;
    public List<DialogueLine> dialogueLineList;

    // 构造函数，初始化列表
    public SingleDialogue(int id)
    {
        dialogueID = id;
        triggerList = new List<DialogueTriggerType>();
        dialogueLineList = new List<DialogueLine>();
    }
}

public enum DialogueTriggerType
{
    Single,
    Paragraph,
    General,
    Near,
    M_Bar,M_Church,
    W_Medieval,
    ClosePanel,
    Talk,
    Timeline,
}
[Serializable]
public struct DialogueLine
{
    public string localizationIdentifier;
    public int lineIndex;
    public int nextLineIndex;
    public DialogueType dialogueType;
    public string text;
}


public enum DialogueType
{
    Dialogue,
    Selection,
    End
}