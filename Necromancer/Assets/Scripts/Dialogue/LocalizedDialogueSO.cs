using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Other/Localization")]
public class LocalizedDialogueSO : ScriptableObject
{
    public Dictionary<string, LocalizedLine> localizedLines;
}

[System.Serializable]
public class LocalizedLine
{
    public string zh;
    public string zh_TW;
    public string en;
    public string jp;
}