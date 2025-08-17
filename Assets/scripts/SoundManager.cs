using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    PLAYER_FOOTSTEPS,
    ABILITY_USED,
    GROG_CONSUMED,
    PLAYER_KILLED,
    ORC_ALERTED,
    ORC_COMBAT,
}

public enum MusicType
{
    AMBIENT,
    COMBAT,
}


[System.Serializable]
public class SoundAudioClips
{
    public SoundType soundType;
    [SerializeField] private AudioClip[] clips;
    private int ptr = 0;

    public AudioClip GetCurrentSound()
    {
        if (clips.Length == 0)
        {
            return null;
        }
        else
        {
            if (ptr == clips.Length)
            {
                ptr = 0;
            }
            AudioClip currClip =  clips[ptr];
            ptr++;
            return currClip;
        }
    }
}

[System.Serializable]
public class MusicAudioClips
{
    public MusicType musicType;
    public AudioClip clip;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private SoundAudioClips[] sounds;
    [SerializeField] private MusicAudioClips[] music;
    private AudioSource audioSource;
    
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        PlayMusic(MusicType.AMBIENT);
    }

    public static void PlaySound(SoundType type)
    {
        foreach (SoundAudioClips sound in instance.sounds)
        {
            if (sound.soundType == type)
            {
                AudioClip clip = sound.GetCurrentSound();
                instance.audioSource.PlayOneShot(clip);
                break;
            }
        }
    }

    public static void PlayMusic(MusicType type)
    {
        foreach (MusicAudioClips track in instance.music)
        {
            if (track.musicType == type)
            {
                instance.audioSource.Stop();
                instance.audioSource.clip = track.clip;
                instance.audioSource.loop = true;
                instance.audioSource.Play();
            }
        }
    }

    public static void StopMusic()
    {
        instance.audioSource.Stop();
    }
}
