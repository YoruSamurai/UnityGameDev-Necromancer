using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 背景音乐 : MonoBehaviour
{
    [Header("音效设置")]
    [SerializeField] private SoundDataSO bgMusic;
    [SerializeField] private float triggerDistance = 5f;


    private SoundBuilder soundBuilder;
    private Transform playerTransform;
    private bool isPlayerNear;

    private void Start()
    {
        SoundData soundData = bgMusic.GetSoundData();
        soundBuilder = SoundManager.Instance.CreateSound()
            .WithSoundData(soundData)
            .WithPosition(gameObject.transform.position)
            .WithRandomPitch();

        // 然后可以继续操作或存储这个 builder
        soundBuilder.PrewarmPlay(); // 或者 PrewarmPlay() 如果你有这个方法
        soundBuilder.JustPlay();

    }

    private void Update()
    {
        
    }

    public void StartBgMusic()
    {
        soundBuilder.JustPlay();
    }

    public void StopBgMusic()
    {
        soundBuilder.JustStop();

    }

    private void OnDestroy()
    {
        if (soundBuilder != null)
        {
            soundBuilder.Stop();
        }
    }

}
