using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class DialogueManager : SingletonManagerBase<DialogueManager>
{

    [SerializeField] private GameObject dialogueControllerPrefab;
    [SerializeField] private LocalizedDialogueSO localizedDialogueSO;

    private Dictionary<string, LocalizedLine> localizedLines;

    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
    }

    private void Start()
    {
        if(localizedDialogueSO!= null)
        {
            localizedLines = new Dictionary<string, LocalizedLine>();
            foreach(var line in localizedDialogueSO.localizedLines)
            {
                localizedLines[line.identifier] = line;
            }
        }
    }

    public void StartDialogue(SingleDialogue dialogue,Transform speakerTransform,IDialogueSpeaker dialogueSpeaker)
    {
        GameObject obj = Instantiate(dialogueControllerPrefab, speakerTransform.position, Quaternion.identity, speakerTransform);
        DialogueController generator = obj.GetComponent<DialogueController>();
        generator.Initialize(dialogue,speakerTransform, dialogueSpeaker);
    }

    public List<SingleDialogue> GetProperDialogue(DialogueSO dialogueSO, List<DialogueTriggerType> dialogueTriggerTypeList)
    {
        List<SingleDialogue> properDialogues = new List<SingleDialogue>();
        if (dialogueSO == null)
            return properDialogues;
        foreach(var dialogue in dialogueSO.DialogueList)
        {
            // 检查 triggerList 是否包含在 dialogueTriggerTypeList 中
            if (dialogue.triggerList.All(trigger => dialogueTriggerTypeList.Contains(trigger)) &&
                dialogueTriggerTypeList.All(trigger => dialogue.triggerList.Contains(trigger)))
            {
                Debug.Log("可以添加这个SO");
                properDialogues.Add(dialogue);
            }
            else
            {
                if (dialogueTriggerTypeList.Contains(DialogueTriggerType.Single))
                {
                    if(dialogue.triggerList.Contains(DialogueTriggerType.Single) && dialogue.triggerList.Contains(DialogueTriggerType.General))
                    {
                        Debug.Log("作为默认语句，可以添加这个SO");
                        properDialogues.Add(dialogue);
                    }
                }
            }
        }
        return properDialogues;
    }

    public string GetTextById(string id)
    {
        // 检查字典是否为null
        if (localizedDialogueSO == null || localizedDialogueSO.localizedLines == null)
        {
            Debug.LogError("LocalizedDialogueSO 未初始化或 localizedLines 字典为空！");
            return string.Empty; // 返回空字符串或其他默认值
        }

        // 尝试从字典中获取LocalizedLine
        if (localizedLines.TryGetValue(id, out LocalizedLine localizedLine))
        {
            return localizedLine.zh; // 返回对应的中文字符串
        }
        else
        {
            Debug.LogWarning($"未找到ID为 {id} 的对话文本！");
            return string.Empty; // 返回空字符串或其他默认值
        }
    }

    
}
