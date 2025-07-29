// AudioManager.cs (已升级BGM功能)
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音源组件")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("音效库")]
    [SerializeField] private AudioClip[] soundClips;
    private Dictionary<string, AudioClip> soundClipDict;

    [Header("BGM播放列表")] // 【新增】
    [SerializeField] private AudioClip[] bgmPlaylist;
    private int currentBgmIndex = -1;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        soundClipDict = new Dictionary<string, AudioClip>();
        foreach (var clip in soundClips)
        {
            if (clip != null && !soundClipDict.ContainsKey(clip.name))
            {
                soundClipDict[clip.name] = clip;
            }
        }
    }

    private void Start()
    {
        // 【新增】游戏一开始就播放背景音乐
        PlayNextBgmTrack();
    }

    private void Update()
    {
        // 【新增】当BGM播放完毕后，自动播放下一首，实现循环
        if (musicSource != null && !musicSource.isPlaying && bgmPlaylist.Length > 0)
        {
            PlayNextBgmTrack();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PlaySound(string clipName)
    {
        if (soundClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 找不到名为 {clipName} 的音效。");
        }
    }

    /// <summary>
    /// 【新增】播放BGM列表中的下一首歌曲
    /// </summary>
    private void PlayNextBgmTrack()
    {
        if (bgmPlaylist == null || bgmPlaylist.Length == 0) return;

        // 简单地顺序播放，到底后从头开始
        currentBgmIndex = (currentBgmIndex + 1) % bgmPlaylist.Length;
        musicSource.clip = bgmPlaylist[currentBgmIndex];
        musicSource.Play();
    }
}