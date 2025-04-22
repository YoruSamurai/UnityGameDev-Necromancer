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

    private void Awake()
    {
        audioSource = gameObject.GetOrAdd<AudioSource>();
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
