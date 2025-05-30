using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class 吟游诗人 : MonoBehaviour, IDialogueSpeaker
{
    [Header("音效设置")]
    [SerializeField] private SoundDataSO poetMusic;
    [SerializeField] private float triggerDistance = 8f;

    [SerializeField] private DialogueSO dialogueSO;
    [SerializeField] private List<int> hasUsedDialogueInScene = new List<int>();
    [SerializeField] private int usedSingleInScene = -1;

    [Header("背景音乐")]
    [SerializeField] private 背景音乐 bgMusic;

    private SoundBuilder soundBuilder;
    private Transform playerTransform;
    private bool isPlayerNear;

    #region 对话控制器

    // 私有字段来存储 DialogueController 实例
    private DialogueController dialogueController;

    // 实现接口的属性
    public DialogueController DialogueController
    {
        get => dialogueController; // 返回存储的 DialogueController 实例
        set => dialogueController = value; // 设置存储的 DialogueController 实例
    }

    public void SetCurrentDialogue(DialogueController dialogueController)
    {
        DialogueController = dialogueController;
        // 这里可以添加更多逻辑，例如开始对话
    }

    public void RemoveCurrentDialogue()
    {
        if (dialogueController != null)
        {
            dialogueController.DestroyController();
        }
        DialogueController = null; // 清除当前对话控制器
        // 这里可以添加更多逻辑，例如结束对话
    }

    #endregion

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        SoundData soundData = poetMusic.GetSoundData();
        soundBuilder = SoundManager.Instance.CreateSound()
            .WithSoundData(soundData)
            .WithPosition(gameObject.transform.position)
            .WithRandomPitch();

        // 然后可以继续操作或存储这个 builder
        soundBuilder.PrewarmPlay(); // 或者 PrewarmPlay() 如果你有这个方法
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool shouldBePlaying = distance <= triggerDistance;

        if (shouldBePlaying)
        {

        }


        if (shouldBePlaying != isPlayerNear)
        {
            isPlayerNear = shouldBePlaying;

            if (isPlayerNear)
            {
                /*string id = DialogueManager.Instance.GetTextById("smith_greeting_1_01");
                DialogueManager.Instance.GenerateDialogueBubble(id, "smith_greeting_1_01", transform);*/
                FindProperDialogueWithType(DialogueTriggerType.Single, DialogueTriggerType.Near);
                soundBuilder.JustPlay();
                bgMusic.StopBgMusic();
            }
            else
            {
                soundBuilder.JustStop();
                bgMusic.StartBgMusic();

            }
        }
    }

    private void FindProperDialogueWithType(params DialogueTriggerType[] triggerTypes)
    {
        List<DialogueTriggerType> triggerTypeList = triggerTypes.ToList();

        List<SingleDialogue> properDialogues = DialogueManager.Instance.GetProperDialogue(dialogueSO, triggerTypeList);
        SingleDialogue singleDialogue = new SingleDialogue(1);
        if (triggerTypeList.Contains(DialogueTriggerType.Single))
        {
            List<int> filteredDialogues = new List<int>();
            bool isGenaral = false;
            if(usedSingleInScene == -1)
            {
                Debug.Log(123);
                // 首先选择不包含 General 的对话
                foreach (var dialogue in properDialogues)
                {
                    Debug.Log(1232);

                    if (!dialogue.triggerList.Contains(DialogueTriggerType.General) 
                        && !hasUsedDialogueInScene.Contains(dialogue.dialogueID))
                    {
                        Debug.Log(1123);

                        filteredDialogues.Add(dialogue.dialogueID);
                    }
                }
            }

            // 如果没有找到不包含 General 的对话，则选择包含 General 的对话
            if (filteredDialogues.Count == 0)
            {
                isGenaral = true;
                foreach (var dialogue in properDialogues)
                {
                    if (dialogue.triggerList.Contains(DialogueTriggerType.General))
                    {
                        filteredDialogues.Add(dialogue.dialogueID);
                    }
                }
            }
            int index = filteredDialogues[UnityEngine.Random.Range(0, filteredDialogues.Count)];
            usedSingleInScene = isGenaral ? usedSingleInScene : index;
            //找到dialogueSO里对应的对话，开始进行显示
            foreach(var dialogue in properDialogues)
            {
                if(dialogue.dialogueID == index)
                {
                    singleDialogue = dialogue;
                }
            }
            
        }
        TryStartDialogue(singleDialogue);
    }

    private void TryStartDialogue(SingleDialogue singleDialogue)
    {
        //可能还要加入更多的判断
        //判断不是default 才继续
        if(singleDialogue.triggerList.Count > 0)
        {
            DialogueManager.Instance.StartDialogue(singleDialogue,transform,this);
        }

    }

    

    private void OnDestroy()
    {
        soundBuilder.Stop();
    }

    // 可选：在编辑器中显示触发范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }

}
