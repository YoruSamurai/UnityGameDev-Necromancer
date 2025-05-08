using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 吟游诗人 : MonoBehaviour
{
    [Header("音效设置")]
    [SerializeField] private SoundDataSO poetMusic;
    [SerializeField] private float triggerDistance = 15f;

    [Header("背景音乐")]
    [SerializeField] private 背景音乐 bgMusic;

    private SoundBuilder soundBuilder;
    private Transform playerTransform;
    private bool isPlayerNear;

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

        if (shouldBePlaying != isPlayerNear)
        {
            isPlayerNear = shouldBePlaying;

            if (isPlayerNear)
            {
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
