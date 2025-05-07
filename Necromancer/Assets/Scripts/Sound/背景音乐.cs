using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 背景音乐 : MonoBehaviour
{
    [Header("音效设置")]
    [SerializeField] private SoundData bgMusic;
    [SerializeField] private float triggerDistance = 5f;


    private SoundEmitter currentEmitter;
    private Transform playerTransform;
    private bool isPlayerNear;

    private void Start()
    {
        SoundManager.Instance.CreateSound()
            .WithSoundData(bgMusic)
            .WithPosition(gameObject.transform.position)
            .WithRandomPitch()
            .Play();

    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (currentEmitter != null)
        {
            SoundManager.Instance.ReturnToPool(currentEmitter);
        }
    }

}
