using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBuilder
{
    readonly SoundManager soundManager;
    SoundData soundData;
    Vector3 position = Vector3.zero;
    bool randomPitch;
    SoundEmitter soundEmitter;

    public SoundBuilder(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    /// <summary>
    /// 把音频赋值过去
    /// </summary>
    /// <param name="soundData"></param>
    /// <returns></returns>
    public SoundBuilder WithSoundData(SoundData soundData)
    {
        this.soundData = soundData;
        return this;
    }
    
    //音频位置
    public SoundBuilder WithPosition(Vector3 position)
    {
        this.position = position;
        return this;
    }

    //随机振幅 暂时没用 不生效
    public SoundBuilder WithRandomPitch()
    {
        this.randomPitch = true;
        return this;
    }

    /// <summary>
    /// 立刻播放！适用于音效
    /// </summary>
    public void Play()
    {
        if (!soundManager.CanPlaySound(soundData)) return;

        //从对象池获取一个soundEmitter开始播放
        soundEmitter = soundManager.Get();
        soundEmitter.Initialize(soundData);
        soundEmitter.transform.position = position;
        soundEmitter.transform.parent = SoundManager.Instance.transform;

        if(randomPitch)
        {
            soundEmitter.WithRandomPitch();
        }

        //拿了记得告诉人家
        soundManager.counts[soundData] = soundManager.counts.TryGetValue(soundData, out var count) ? count + 1 : 1;
        soundEmitter.Play();
    }

    /// <summary>
    /// 不一定是立刻播放，但是会在对象池中预加载
    /// </summary>
    public void PrewarmPlay()
    {
        if (!soundManager.CanPlaySound(soundData)) return;

        soundEmitter = soundManager.Get();
        soundEmitter.Initialize(soundData);
        soundEmitter.transform.position = position;
        soundEmitter.transform.parent = SoundManager.Instance.transform;

        if (randomPitch)
        {
            soundEmitter.WithRandomPitch();
        }
        soundEmitter.Preload(); // 预加载音频数据
        soundManager.counts[soundData] = soundManager.counts.TryGetValue(soundData, out var count) ? count + 1 : 1;
    }

    public void JustPlay()
    {
        soundEmitter.JustPlay();
    }

    public void JustStop()
    {
        soundEmitter.JustStop();
    }
    /// <summary>
    /// 销毁音频对象返回对象池
    /// </summary>
    public void Stop()
    {
        soundEmitter.Stop();
    }
}
