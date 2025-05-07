using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yoru;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{

    public SoundData data {  get; private set; }
    private AudioSource audioSource;
    Coroutine playingCoroutine;

    private bool isPaused = false;

    private Coroutine fadeCoroutine;
    private float originalVolume;
    [SerializeField] private float fadeDuration = 1.0f; // 淡入淡出时长

    private void Awake()
    {
        audioSource = gameObject.GetOrAdd<AudioSource>();
        originalVolume = audioSource.volume;
    }

    public void Play()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }
        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    public void JustPlay()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 如果音频没有在播放，先设置音量为0再开始播放
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0;
            if (isPaused)
            {
                audioSource.UnPause();
                isPaused = false;
            }
            else
            {
                audioSource.Play();
            }
        }

        fadeCoroutine = StartCoroutine(FadeAudio(originalVolume));
    }

    public void JustStop()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeAudio(0, true));
    }

    private IEnumerator FadeAudio(float targetVolume, bool stopAfterFade = false)
    {
        float startVolume = audioSource.volume;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (stopAfterFade)
        {
            audioSource.Pause();
            isPaused = true;
        }
    }

    public void Stop()
    {
        if(playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }
        audioSource.Stop();
        SoundManager.Instance.ReturnToPool(this);
    }

    public void Preload()
    {
        if (audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Unloaded)
        {
            audioSource.clip.LoadAudioData(); // 强制预加载音频数据
        }
    }

    private IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        SoundManager.Instance.ReturnToPool(this);
    } 

    public void Initialize(SoundData data)
    {
        this.data = data;
        audioSource.clip = data.clip;
        audioSource.outputAudioMixerGroup = data.mixerGroup;
        audioSource.loop = data.loop;
        audioSource.playOnAwake = data.playOnAwake;
    }

    public void WithRandomPitch(float min = -.05f, float max = .05f)
    {
        audioSource.pitch += Random.Range(min, max);
    }
}
