using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/SoundDataSO")]
public class SoundDataSO : ScriptableObject
{
    [SerializeField] private SoundData soundData;
    /// <summary>
    /// 获取音频数据
    /// </summary>
    /// <returns></returns>
    public SoundData GetSoundData()
    {
        return soundData; 
    }


}
