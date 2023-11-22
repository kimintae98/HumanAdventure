using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    public AudioSource bgmPlayer;
    [Header("SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    int channelIndex;
    AudioSource[] sfxPlayers;
    public enum Bgm { Login, Town, Forest, Desert };
    public enum Sfx { Select, Cancle, Jump, Hit, Heal, Kill, Missile, Rock, Taunt };
    void Awake()
    {
        instance = this;
        Init();
    }
    void Init() //by.인태 / 초기화
    {
        // 배경음
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        // 효과음
        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }
    public void PlayBgm(Bgm bgm) // by.인태 / Bgm 플레이
    {
        bgmPlayer.clip = bgmClips[(int)bgm];
        bgmPlayer.Play();
    }
    public void PlaySfx(Sfx sfx) // by.인태 / 효과음 플레이
    {
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
                continue;
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}