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
}

public enum DialogueTriggerType
{
    Nearby,
    Map_Bar,
    ClosePanel,
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