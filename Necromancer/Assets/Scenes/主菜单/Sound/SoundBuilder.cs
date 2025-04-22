using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBuilder
{
    readonly SoundManager soundManager;
    SoundData soundData;
    Vector3 position = Vector3.zero;
    bool randomPitch;

    public SoundBuilder(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    public SoundBuilder WithSoundData(SoundData soundData)
    {
        this.soundData = soundData;
        return this;
    }

    public SoundBuilder WithPosition(Vector3 position)
    {
        this.position = position;
        return this;
    }

    public SoundBuilder WithRandomPitch()
    {
        this.randomPitch = true;
        return this;
    }

    public void Play()
    {
        if (!soundManager.CanPlaySound(soundData)) return;

        SoundEmitter soundEmitter = soundManager.Get();
        soundEmitter.Initialize(soundData);
        soundEmitter.transform.position = position;
        soundEmitter.transform.parent = SoundManager.Instance.transform;

        if(randomPitch)
        {
            soundEmitter.WithRandomPitch();
        }

        soundManager.counts[soundData] = soundManager.counts.TryGetValue(soundData, out var count) ? count + 1 : 1;
        soundEmitter.Play();
    }
}
