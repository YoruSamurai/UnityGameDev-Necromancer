public interface IDialogueSpeaker
{
    public DialogueController DialogueController { get; set; }

    public void SetCurrentDialogue(DialogueController dialogueController);

    public void RemoveCurrentDialogue();

}
