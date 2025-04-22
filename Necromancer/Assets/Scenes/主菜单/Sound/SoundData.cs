using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



[Serializable]
public class SoundData
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public bool loop;
    public bool playOnAwake;
}
