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

    /// <summary>
    /// 这个方法适用于单次播放 播放完销毁的音效
    /// 如果要循环播放并且不销毁 请调用JustPlay
    /// </summary>
    public void Play()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }
        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }


    /// <summary>
    /// 循环播放且只是暂停不销毁 有渐进效果
    /// </summary>
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

    /// <summary>
    /// 循环播放且只是暂停不销毁 有一个渐出效果
    /// </summary>
    public void JustStop()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeAudio(0, true));
    }

    /// <summary>
    /// 淡入淡出音频效果
    /// </summary>
    /// <param name="targetVolume"></param>
    /// <param name="stopAfterFade"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 销毁音频对象返回到对象池
    /// </summary>
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

    /// <summary>
    /// 为了避免卡顿 预加载一些音频
    /// </summary>
    public void Preload()
    {
        if (audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Unloaded)
        {
            audioSource.clip.LoadAudioData(); // 强制预加载音频数据
        }
    }

    /// <summary>
    /// 在音频播放完之后返回对象池
    /// </summary>
    /// <returns></returns>
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
